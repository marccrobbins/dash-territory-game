using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashTerritory
{
    public class SpawnPoint : MonoBehaviour
    {
        public Transform spawnPoint;
        
        public Vector3 spawnPosition => spawnPoint.position;

        private void Start()
        {
            PlayerManager.Instance.RegisterSpawnPoint(this);
        }
    }
}
