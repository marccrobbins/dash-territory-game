using UnityEngine;

namespace DashTerritory
{
    public class SpawnPoint : MonoBehaviour
    {
        public Transform spawnPoint;
        
        public Vector3 spawnPosition => spawnPoint.position;

        private void Awake()
        {
            PlayerManager.Instance.RegisterSpawnPoint(this);
        }

        private void OnDestroy()
        {
            PlayerManager.Instance.UnregisterSpawnPoint(this);
        }
    }
}
