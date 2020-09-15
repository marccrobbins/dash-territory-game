using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pooling
{
    public class Pool : MonoBehaviour
    {
        private PoolDataItem poolDataItem;
        public List<GameObject> consumed;
        public List<GameObject> unConsumed;

        public void Initialize(PoolDataItem dataItem, List<GameObject> instances)
        {
            poolDataItem = dataItem;
            unConsumed = instances;
            consumed = new List<GameObject>();
        }
        
        #region Spawn
        
        public GameObject Consume()
        {
            //Create new instance because all others are already consumed
            if (unConsumed.Count == 0)
            {
                var newInstance = Instantiate(poolDataItem.prefab, transform);
                newInstance.name = poolDataItem.poolName;
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
