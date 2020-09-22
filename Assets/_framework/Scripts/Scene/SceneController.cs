using System.Collections;
using System.Collections.Generic;
using DashTerritory;
using UnityEngine;

namespace Framework
{
    public class SceneController : MonoBehaviour, ISceneInitialization
    {
        private const string LogPrefix = "SCENECONTROLLER :: ";
        
        public void ContentLoaded()
        {
            Debug.Log($"{LogPrefix}Content loaded");
            TerritoryManager.Instance.BuildTerritory();
            GameManager.Instance.StartGame();
            InputManager.SetInputMap(InputMapType.Game);
        }

        public void UnloadingContent()
        {
            Debug.Log($"{LogPrefix}Content unloaded");
        }
    }
}
