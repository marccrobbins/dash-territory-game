using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.GameData;
using Framework.Writer;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Framework.Pooling
{
    [CreateAssetMenu(menuName = "GameData/PoolData", fileName = "PoolData")]
    public class PoolData : GameDataManager
    {
        public override Type childType => null;
        public override string childrenDirectory => string.Empty;
        
        [TableList(AlwaysExpanded = true)]
        public PoolDataItem[] poolDataList;
        
        public override ScriptableObject CreateNew()
        {
            return null;
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
    public sealed class PoolDataItem
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
