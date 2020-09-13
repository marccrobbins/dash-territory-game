using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class SceneLoadManager : MonoBehaviour
    {
        private static SceneLoadManager _instance;

        private static List<SceneData> _queue = new List<SceneData>();
        private static SceneData _currentSceneData;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            
            DontDestroyOnLoad(gameObject);
        }

        public static void Enqueue(
            string sceneName,
            LoadType type,
            bool isAsync = false, 
            LoadSceneMode mode = LoadSceneMode.Single,
            Action<float, string> onProgressCallback = null,
            Action<string> onCompleteCallback = null)
        {
            if(string.IsNullOrEmpty(sceneName)) return;
            Enqueue(new List<string> { sceneName }, type, isAsync, mode, onProgressCallback, onCompleteCallback);
        }

        /// <summary>
        /// Enqueue for loading a scene with name.
        /// </summary>
        /// <param name="sceneNames">Names of the scene to load or unload.</param>
        /// <param name="type">The type of load from <see cref="LoadType"/>.</param>
        /// <param name="isAsync">Should the scene load asyncronously.</param>
        /// <param name="mode">The <see cref="LoadSceneMode"/>.</param>
        /// <param name="onProgressCallback">Callback to report progress of the scene.</param>
        /// <param name="onCompleteCallback">Callback to report when the process has complete.</param>
        public static void Enqueue(
            List<string> sceneNames, 
            LoadType type, 
            bool isAsync = false, 
            LoadSceneMode mode = LoadSceneMode.Single,
            Action<float, string> onProgressCallback = null, 
            Action<string> onCompleteCallback = null)
        {
            if (!IsValid() ||
                type == LoadType.None) return;

            //Force additive loading if there is more than one scene to load
            if (sceneNames.Count > 1 &&
                mode == LoadSceneMode.Single)
            {
                mode = LoadSceneMode.Additive;
            }
            
            _queue.Add(new SceneData
            {
                sceneNames = sceneNames,
                isAsync = isAsync,
                loadType = type,
                mode = mode,
                onProgressCallback = onProgressCallback,
                onCompleteCallback = onCompleteCallback
            });

            if (!_currentSceneData.inProgress)
            {
                QueueNext();
            }
        }

        /// <summary>
        /// Dequeue a queued up scene load or unload.
        /// </summary>
        /// <param name="sceneName">Scene name to remove from queue.</param>
        public static void Dequeue(string sceneName)
        {
            SceneData dataToRemove = new SceneData();
            foreach (var data in _queue)
            {
                if (data.sceneNames.Contains(sceneName))
                {
                    if (!data.inProgress)
                    {
                        dataToRemove = data;
                    }
                    break;
                }
            }

            if (dataToRemove.isValid)
            {
                _queue.Remove(dataToRemove);
            }
        }

        private static void QueueNext()
        {
            if (_queue.Count == 0)
            {
                Debug.Log("There is no more scenedata in queue.");
                return;
            }

            _currentSceneData = _queue[0];

            _instance.StartCoroutine(_instance.QueueRoutine());
        }

        private IEnumerator QueueRoutine()
        {
            var data = _currentSceneData;
            var count = data.sceneNames.Count;
            var progressIteration = (float)1 / count;
            
            if (data.isAsync)
            {
                AsyncOperation async = null;
                var progress = 0f;
                
                data.inProgress = true;

                for (var i = 0; i < count; i++)
                {
                    var sceneName = data.sceneNames[i];
                    
                    if (data.loadType == LoadType.Load)
                    {
                        async = SceneManager.LoadSceneAsync(sceneName, data.mode);
                    }
                    else
                    {
                        async = SceneManager.UnloadSceneAsync(sceneName);
                    }
                
                    if(async == null) yield break;

                    while (!async.isDone)
                    {
                        progress += async.progress * progressIteration;
                        Debug.LogFormat("async progress : {0} | iteration : {1}", async.progress, progressIteration);
                        data.onProgressCallback?.Invoke(progress, sceneName);
                        yield return null;
                    }
                
                    data.onProgressCallback?.Invoke(progress, sceneName);
                }

                data.inProgress = false;
                
                SceneProcessComplete(SceneManager.GetSceneByName(data.sceneNames[data.sceneNames.Count - 1]), data.mode);
            }
            else
            {
                if (data.loadType == LoadType.Load)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var name = data.sceneNames[i];
                        if (i == count - 1)
                        {
                            SceneManager.sceneLoaded += SceneProcessComplete;
                        }

                        SceneManager.LoadScene(name, data.mode);
                    }
                }
            }
        }

        private void SceneProcessComplete(Scene scene, LoadSceneMode mode)
        {
            _currentSceneData.onCompleteCallback?.Invoke(scene.name);
            SceneManager.sceneLoaded -= SceneProcessComplete;
            
            _queue.RemoveAt(0);
            QueueNext();
        }

        private static bool IsValid()
        {
            if (!_instance)
            {
                Debug.LogError("No reference to SceneLoadManager in scene, please put SceneLoadManager prefab in scene and play again.");
            }
            
            return _instance;
        }

        private void OnDisable()
        {
            _instance = null;
        }

        [Serializable]
        public struct SceneData
        {
            public bool isValid;
            public bool inProgress;

            public List<string> sceneNames;
            public bool isAsync;
            public LoadType loadType;
            public LoadSceneMode mode;
            public Action<float, string> onProgressCallback;
            public Action<string> onCompleteCallback;
        }
    }

    public enum LoadType
    {
        None = 0,
        Load,
        Unload
    }
}
