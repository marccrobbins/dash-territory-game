using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pooling
{
    public class Pool : MonoBehaviour
    {
        private PoolData poolData;
        public List<GameObject> consumed;
        public List<GameObject> unConsumed;

        public void Initialize(PoolData data, List<GameObject> instances)
        {
            poolData = data;
            unConsumed = instances;
            consumed = new List<GameObject>();
        }
        
        #region Spawn
        
        public GameObject Consume()
        {
            //Create new instance because all others are already consumed
            if (unConsumed.Count == 0)
            {
                var newInstance = Instantiate(poolData.prefab, transform);
                newInstance.name = poolData.poolName;
                consumed.Add(newInstance);
                
                return newInstance;
            }
            
            //consume existing instance
            var instance = unConsumed[0];
            consumed.Add(instance);
            unConsumed.Remove(instance);

            return instance;
        }

        #endregion Spawn

        public void UnConsume(GameObject instance, bool resetTransform = true)
        {
            instance.SetActive(false);
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(transform);
            
            if (resetTransform)
            {
                instanceTransform.position = Vector3.zero;
                instanceTransform.rotation = Quaternion.Euler(Vector3.zero);
                instanceTransform.localScale = Vector3.one;
            }
            
            var consumedIndex = consumed.IndexOf(instance);
            var consumedInstance = consumed[consumedIndex];
            consumed.RemoveAt(consumedIndex);
            unConsumed.Add(consumedInstance);
            
            //IPoolable
            foreach (var poolable in instance.GetComponentsInChildren<IPoolable>())
            {
                poolable?.OnDeSpawn();
            }
        }

        public void UnConsumeAll()
        {
            foreach (var instance in new List<GameObject>(consumed))
            {
                UnConsume(instance);
            }
            
            consumed.Clear();
        }
    }
}
