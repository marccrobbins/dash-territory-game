using System.Collections;
using System.Collections.Generic;
using Framework.Pooling.Generated;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Pooling
{
    public class SpawnTester : MonoBehaviour
    {
        public Transform parent;
        public bool worldPositionStays;

        public GameObject instance;
        
        [Button]
        private void TestSpawn()
        {
            instance = PoolManager.Instance.Spawn(PoolNames.BODYSLAMEFFECT, worldPositionStays);
        }

        [Button]
        private void TestSpawnP()
        {
            PoolManager.Instance.Spawn(PoolNames.BODYSLAMEFFECT, transform.position, worldPositionStays);
        }
        
        [Button]
        private void TestSpawnPR()
        {
            PoolManager.Instance.Spawn(PoolNames.BODYSLAMEFFECT, transform.position, transform.rotation, worldPositionStays);
        }
        
        [Button]
        private void TestSpawnPRS()
        {
            PoolManager.Instance.Spawn(PoolNames.BODYSLAMEFFECT, transform.position, transform.rotation, transform.localScale, worldPositionStays);
        }
        
        [Button]
        private void TestSpawnPRSP()
        {
            PoolManager.Instance.Spawn(PoolNames.BODYSLAMEFFECT, transform.position, transform.rotation, transform.localScale, parent, worldPositionStays);
        }
        
        [Button]
        private void TestDeSpawn()
        {
            PoolManager.Instance.DeSpawn(instance);
        }

        [Button]
        private void TestDeSpawnAll()
        {
            PoolManager.Instance.DeSpawnAll(PoolNames.BODYSLAMEFFECT);
        }
    }
}
