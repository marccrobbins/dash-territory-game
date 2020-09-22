using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace DashTerritory
{
    public class PlayerManager : Manager<PlayerManager>
    {
        public const string LogPrefix = "PLAYERMANAGER :: ";
        
        public GameObject playerPrefab;
        public float reviveTime = 3;

        public List<SpawnPoint> spawnPoints;

        protected override IEnumerator InitializeManager()
        {
            spawnPoints = new List<SpawnPoint>();
            yield break;
        }

        public void RegisterSpawnPoint(SpawnPoint spawn)
        {
            if (spawnPoints.Contains(spawn)) return;
            spawnPoints.Add(spawn);
        }

        public void UnregisterSpawnPoint(SpawnPoint spawn)
        {
            if (!spawnPoints.Contains(spawn)) return;
            spawnPoints.Remove(spawn);
        }

        public void SpawnPlayers()
        {
            Debug.Log($"{LogPrefix}Spawning players");

            var playersInputs = InputManager.Instance.PlayerInputObjects;
            foreach (var playerInput in playersInputs)
            {
                var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                var playerObject = Instantiate(playerPrefab, spawnPoint.spawnPosition, Quaternion.identity);
                var player = playerObject.GetComponent<Player>();
                player.Initialize(playerInput);
            }
        }

        public void PlayerDied(Player player)
        {
            StartCoroutine(WaitToRevive(player));
        }

        private IEnumerator WaitToRevive(Player player)
        {
            player.gameObject.SetActive(false);
            
            yield return new WaitForSeconds(reviveTime);

            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].spawnPosition;
            player.gameObject.SetActive(true);
        }
    }
}
