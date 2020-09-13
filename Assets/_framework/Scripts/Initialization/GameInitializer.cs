using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class GameInitializer : MonoBehaviour
    {
        private const string LogPrefix = "<color=#9cff51>GAMEINITIALIZER :: </color>";

        [SerializeField] private SceneReference firstScene;
        [SerializeField] private GameObject[] managers;

        public event Action OnInitializationComplete;

        private void Awake()
        {
            StartCoroutine(BeginInitialization());
        }

        private IEnumerator BeginInitialization()
        {
            var instances = new List<GameObject>();

            Debug.Log($"{LogPrefix}Getting all instances of managers to initialize");
            foreach (var manager in managers)
            {
                if (!manager) continue;

                var isPrefab = manager.transform != null && string.IsNullOrEmpty(manager.scene.name);
                var instance = isPrefab ? Instantiate(manager, null, true) : manager;

                if (!instance) continue;

                DontDestroyOnLoad(instance);
                instances.Add(instance);
            }

            Debug.Log($"{LogPrefix}Getting all initializables");
            var initializables = GetInitializables(instances);
            
            if (initializables == null || 
                initializables.Count == 0) yield break;
            
            var initializedLookup = new Dictionary<IInitializable, bool>();
            foreach (var initializable in initializables)
            {
                initializedLookup[initializable] = false;
                initializable.Initialize();
            }

            //Wait for all managers to be initialized
            var initializedCount = 0;
            var initializedTarget = initializables.Count;
            var frameWait = new WaitForEndOfFrame();
            Debug.Log($"{LogPrefix}Starting to wait for all managers to initialize");
            while (initializedCount < initializedTarget)
            {
                foreach (var initializable in initializables)
                {
                    if (!initializable.IsInitialized || initializedLookup[initializable]) continue;
                    initializedLookup[initializable] = true;
                    initializedCount++;
                }
                
                yield return frameWait;
            }
            
            Debug.Log($"{LogPrefix}Initialization complete");
            
            //Initialization is complete
            OnInitializationComplete?.Invoke();
            
            //Load first scene
            //ToDo make a scene manager
            if (!string.IsNullOrEmpty(firstScene.SceneName) &&
                !string.IsNullOrEmpty(firstScene.ScenePath))
            {
                SceneManager.LoadScene(firstScene.SceneName);
            }
        }

        private List<IInitializable> GetInitializables(List<GameObject> instances)
        {
            var initializables = new List<IInitializable>();

            foreach (var component in from instance in instances
                select instance.GetComponentsInChildren<IInitializable>()
                into initComponents
                where initComponents != null && initComponents.Length != 0
                from component in initComponents
                where !initializables.Contains(component)
                select component)
            {
                initializables.Add(component);
            }

            return initializables;
        }
    }
}
