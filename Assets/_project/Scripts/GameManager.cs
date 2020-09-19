using System.Collections;
using System.Collections.Generic;
using Framework;
using Framework.GameData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class GameManager : Manager<GameManager>
    {
        [Button]
        private void StartGame()
        {
            PlayerManager.Instance.SpawnPlayers();
        }
    }
}
