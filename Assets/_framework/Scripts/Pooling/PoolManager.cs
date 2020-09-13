using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.GameData;
using Framework.Writer;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Pooling
{
    public class PoolManager : Manager<PoolManager>
    {
        public GameObject poolPrefab;
        
        [TableList(AlwaysExpanded = true)]
        public PoolData[] poolDataList;

        private Dictionary<string, Pool> poolLookUp; 
        
        protected override IEnumerator InitializeManager()
        {
            poolLookUp = new Dictionary<string, Pool>();

            foreach (var data in poolDataList)
            {
                var poolObject = Instantiate(poolPrefab, transform);
                poolObject.name = data.poolName + "_Pool";
                var pool = poolObject.GetComponent<Pool>();
                if (!pool)
                {
                    Destroy(poolObject);
                    continue;
                }

                var instances = new List<GameObject>();
                var frameYield = new WaitForEndOfFrame();
                for (var i = 0; i < data.warmCount; i++)
                {
                    var instance = Instantiate(data.prefab, Vector3.zero, Quaternion.identity,
                        poolObject.transform);

                    instance.name = data.poolName;
                    
                    instance.SetActive(false);
                    instances.Add(instance);
                    
                    //Yield for x frames before spawning a new one
                    for (var f = 0; f < data.framesCount; f++)
                    {
                        yield return frameYield;
                    }
                }
                
                pool.Initialize(data, instances);
                poolLookUp.Add(data.poolName, pool);
            }
        }

        #region Spawn

        public GameObject Spawn(string poolName, bool worldPositionStays = true)
        {
            return Spawn(poolName, Vector3.zero, Quaternion.identity, Vector3.one, null, worldPositionStays);
        }
        
        public GameObject Spawn(string poolName, Transform parent, bool worldPositionStays = true)
        {
            return Spawn(poolName, Vector3.zero, Quaternion.identity, Vector3.one, parent, worldPositionStays);
        }
        
        public GameObject Spawn(string poolName, Vector3 position, bool worldPositionStays = true)
        {
            return Spawn(poolName, position, Quaternion.identity, Vector3.one, null, worldPositionStays);
        }
        
        public GameObject Spawn(string poolName, Vector3 position, Transform parent, bool worldPositionStays = true)
        {
            return Spawn(poolName, position, Quaternion.identity, Vector3.one, parent, worldPositionStays);
        }
        
        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation, bool worldPositionStays = true)
        {
            return Spawn(poolName, position, rotation, Vector3.one, null, worldPositionStays);
        }
        
        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStays = true)
        {
            return Spawn(poolName, position, rotation, Vector3.one, parent, worldPositionStays);
        }
        
        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation, Vector3 scale, bool worldPositionStays = true)
        {
            return Spawn(poolName, position, rotation, scale, null, worldPositionStays);
        }
        
        public GameObject Spawn(
            string poolName, 
            Vector3 position, 
            Quaternion rotation, 
            Vector3 scale, 
            Transform parent, 
            bool worldPositionStays = true)
        {
            if (!poolLookUp.ContainsKey(poolName))
            {   
                Debug.Log($"POOLMANAGER :: No such pool exists named {poolName}");
                return null;
            }

            var pool = poolLookUp[poolName];
            var instance = pool.Consume();
            var instanceTransform = instance.transform;
            instanceTransform.position = position;
            instanceTransform.rotation = rotation;
            instanceTransform.localScale = scale;
            instanceTransform.SetParent(parent, worldPositionStays);
            instance.SetActive(true);
            
            //IPoolable
            foreach (var poolable in instance.GetComponentsInChildren<IPoolable>())
            {
                poolable?.OnSpawn();
            }

            return instance;
        }

        #endregion Spawn

        public void DeSpawn(GameObject instance)
        {
            var poolName = instance.name;
            if (!poolLookUp.ContainsKey(poolName))
            {   
                Debug.Log($"POOLMANAGER :: No such pool exists named {poolName}");
                return;
            }
            
            poolLookUp[poolName].UnConsume(instance);
        }

        public void DeSpawnAll(string poolName)
        {
            if (!poolLookUp.ContainsKey(poolName))
            {   
                Debug.Log($"POOLMANAGER :: No such pool exists named {poolName}");
                return;
            }
            
            poolLookUp[poolName].UnConsumeAll();
        }

        #region Odin

        [Button("Compile")]
        private void CompileEnumValues()
        {
            var templateAsset = Resources.Load<TextAsset>("PoolNamesTemplate");
            var template = new ScriptWriter(templateAsset.text);
            
            template.AddVariable("namespace", "Framework.Pooling.Generated");
            template.AddVariable("class", "PoolNames");
            template.AddVariable("constants", GenerateCode().ToArray());

            var fileContent = template.Parse();
            
            if (string.IsNullOrEmpty(fileContent))
            {
                Debug.LogError("File Content is empty.");
                return;
            }

            var path = Path.Combine("Assets/_framework/Scripts/Generated/", "PoolNames.cs");
            if (File.Exists(path))
            {
                var reader = new StreamReader(File.Open(path, FileMode.Open));
                var content = reader.ReadToEnd();
                reader.Dispose();
                reader.Close();
                
                //If contents are the same do nothing
                if (content == fileContent) return;
            }
            
            using (var writer = File.CreateText(path))
            {
                writer.Write(fileContent);
                writer.Dispose();
                writer.Close();
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Code generation successful. PoolNames.cs added to Assets/_framework/Scripts/Generated/");
        }
        
        private List<object[]> GenerateCode()
        {
            var variables = poolDataList.Select(poolItem => poolItem.poolName).ToArray();
            var variablesList = new List<object[]>();
            foreach (var variable in variables)
            {
                var variableName = variable;
                if (variableName.Contains(" "))
                {
                    variableName = variableName.Replace(" ", string.Empty);
                }
                
                variablesList.Add(new []
                {
                    variableName.ToUpper(), //Upper case the variable
                    variable //Value of the variable
                });
            }

            return variablesList;
        }

        #endregion Odin
    }

    [Serializable]
    public sealed class PoolData
    {
        public string poolName;
        [OnValueChanged("ValidatePoolPrefab")]
        public GameObject prefab;
        public int warmCount;
        public int framesCount;

        #region Odin

        private void ValidatePoolPrefab()
        {
            if (!prefab) return;
            
            var poolable = prefab.GetComponent<IPoolable>();
            if (poolable != null) return;
            
            Debug.LogError($"POOLMANAGER :: {prefab.name} is not an IPoolable object, cannot be pooled");
            prefab = null;
        }

        #endregion Odin
    }
}
