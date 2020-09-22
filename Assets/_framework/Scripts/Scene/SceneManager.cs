using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class SceneManager : Manager<SceneManager>
    {
        private const string LogPrefix = "SCENEMANAGER :: ";
        
        public SceneController ActiveSceneController { get; private set; }
        
        #region Load

        public void Load(SceneReference[] sceneRefs)
        {

        }

        public void Load(SceneReference sceneRef)
        {
            Unload();
            StartCoroutine(SceneLoadRoutine(sceneRef));
        }

        private IEnumerator SceneLoadRoutine(SceneReference sceneReference)
        {
            var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneReference.SceneName, LoadSceneMode.Single);
            
            while (!async.isDone)
            {
                yield return null;
            }

            //Initialize all ISceneInitialization objects
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var rootGameObjects = activeScene.GetRootGameObjects();
            foreach (var rootObject in rootGameObjects)
            {
                if (!rootObject) continue;
                var sceneObjects = rootObject.GetComponentsInChildren<ISceneInitialization>().ToList();
                var sceneControllers = rootObject.GetComponentsInChildren<SceneController>().ToList();

                if (sceneControllers.Count > 0)
                {
                    if (sceneControllers.Count > 1)
                    {
                        Debug.LogWarning($"{LogPrefix}There are more than one SceneController in {activeScene.name}. Will only be using the first one.");
                    }

                    foreach (var controller in sceneControllers)
                    {
                        if(!controller) continue;
                        sceneObjects.Remove(controller);
                    }
                    
                    ActiveSceneController = sceneControllers[0];
                    ActiveSceneController.ContentLoaded();
                }
                
                foreach (var sceneObject in sceneObjects)
                {
                    sceneObject?.ContentLoaded();
                }
            }
        }

        #endregion Load

        #region Unload

        public void Unload()
        {
            StartCoroutine(UnloadRoutine());
        }

        private IEnumerator UnloadRoutine()
        {
            var unloadingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            
            var rootGameObjects = unloadingScene.GetRootGameObjects();
            foreach (var rootObject in rootGameObjects)
            {
                if (!rootObject) continue;
                var sceneObjects = rootObject.GetComponentsInChildren<ISceneInitialization>();
                foreach (var sceneObject in sceneObjects)
                {
                    sceneObject?.UnloadingContent();
                }
            }

            var async = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(unloadingScene);
            if (async == null) yield break;
            if (!async.isDone) yield return null;
        }

        #endregion Unload
        
    }
}
