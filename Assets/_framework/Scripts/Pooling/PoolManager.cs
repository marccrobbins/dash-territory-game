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
        public PoolData poolData;

        private Dictionary<string, Pool> poolLookUp; 
        
        protected override IEnumerator InitializeManager()
        {
            poolLookUp = new Dictionary<string, Pool>();

            foreach (var item in poolData.poolDataList)
            {
                var poolObject = Instantiate(poolPrefab, transform);
                poolObject.name = item.poolName + "_Pool";
                var pool = poolObject.GetComponent<Pool>();
                if (!pool)
                {
                    Destroy(poolObject);
                    continue;
                }

                var instances = new List<GameObject>();
                var frameYield = new WaitForEndOfFrame();
                for (var i = 0; i < item.warmCount; i++)
                {
                    var instance = Instantiate(item.prefab, Vector3.zero, Quaternion.identity,
                        poolObject.transform);

                    instance.name = item.poolName;
                    
                    instance.SetActive(false);
                    instances.Add(instance);
                    
                    //Yield for x frames before spawning a new one
                    for (var f = 0; f < item.framesCount; f++)
                    {
                        yield return frameYield;
                    }
                }
                
                pool.Initialize(item, instances);
                poolLookUp.Add(item.poolName, pool);
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
    }
}
