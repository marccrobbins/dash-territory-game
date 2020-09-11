//#define DEBUG_AWAKE_STRIPPING
//#define DEBUG_RESET_STATE
//#define DEBUG_STRIP_SCENE

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Sisus.HierarchyFolders
{
	public class PlayModeStripper
	{
		private static PlayModeStripper instance = null;

		private readonly HashSet<Scene> playModeStrippingHandledForScenes = new HashSet<Scene>();
		private readonly Dictionary<Scene, HashSet<Transform>> playModeStrippingHandledForSceneRootObjects = new Dictionary<Scene, HashSet<Transform>>(1);
		private readonly StrippingType playModeStripping = StrippingType.None;
		private readonly PlayModeStrippingMethod playModeStrippingMethod = PlayModeStrippingMethod.EntireSceneImmediate;

		public PlayModeStripper(StrippingType setStrippingType, PlayModeStrippingMethod setStrippingMethod)
		{
			#if DEV_MODE && DEBUG_RESET_STATE
			Debug.Log("PlayModeStripper("+ setStrippingType + ", "+ setStrippingMethod + ").");
			#endif

			playModeStripping = setStrippingType;
			playModeStrippingMethod = setStrippingMethod;

			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			
		}

		private void OnPlayModeStateChanged(PlayModeStateChange playModeState)
		{
			#if DEV_MODE && DEBUG_RESET_STATE
			Debug.Log("PlayModeStripper.OnPlayModeStateChanged: "+ playModeState);
			#endif

			switch(playModeState)
			{
				case PlayModeStateChange.ExitingPlayMode:
					ResetState();
					break;
			}
		}

		private void ResetState()
		{
			playModeStrippingHandledForScenes.Clear();
			playModeStrippingHandledForSceneRootObjects.Clear();
		}

		private static PlayModeStripper Instance()
		{
			if(instance == null)
			{
				var preferences = HierarchyFolderPreferences.Get();
				instance = new PlayModeStripper(preferences.playModeBehaviour, preferences.playModeStrippingMethod);
			}
			return instance;
		}

		public static void OnSceneObjectAwake(GameObject gameObject)
		{
			Instance().HandleOnSceneObjectAwake(gameObject);
		}

		private void HandleOnSceneObjectAwake(GameObject gameObject)
		{
			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			if(playModeStripping == StrippingType.None)
			{
				#if DEV_MODE && DEBUG_AWAKE_STRIPPING
				Debug.Log("Aborting stripping for gameObject because playModeStripping was None.");
				#endif
				return;
			}

			#if DEV_MODE
			Debug.Assert(EditorApplication.isPlayingOrWillChangePlaymode);
			#endif

			var scene = gameObject.scene;

			// Strip hierarchy from all other scenes which are fully loaded at this time (it should be safe since they are loaded, so we'll do this no matter which stripping method is used).
			int sceneCount = SceneManager.sceneCount;
			if(sceneCount > 1)
			{
				for(int s = 0; s < sceneCount; s++)
				{
					var otherScene = SceneManager.GetSceneAt(s);
					if(otherScene != scene && otherScene.isLoaded && playModeStrippingHandledForScenes.Add(otherScene))
					{
						HierarchyFolderUtility.ApplyStrippingType(otherScene, playModeStripping);
					}
				}
			}

			if(playModeStrippingHandledForScenes.Contains(scene))
			{
				#if DEV_MODE && DEBUG_AWAKE_STRIPPING
				Debug.Log("Aborting stripping for gameObject because stripping already handled for scene "+scene.name);
				#endif
				return;
			}

			switch(playModeStrippingMethod)
			{
				case PlayModeStrippingMethod.EntireSceneWhenLoaded:
					if(!scene.isLoaded)
					{
						#if DEV_MODE && DEBUG_AWAKE_STRIPPING
						Debug.Log("Aborting stripping for "+gameObject.name+" because scene "+scene.name + " not loaded and using stripping method EntireSceneWhenLoaded.");
						#endif
						return;
					}
					break;
				case PlayModeStrippingMethod.IndividuallyDuringAwake:
					// If entire scene is not yet loaded, then only strip GameObjects nested under the root transform of this hierarchy folder (might be safer this way, so won't try to Destroy uninitialized objects).
					if(!scene.isLoaded)
					{
						var rootTransform = gameObject.transform.root;
						HashSet<Transform> handledRootObjects;
						if(!playModeStrippingHandledForSceneRootObjects.TryGetValue(gameObject.scene, out handledRootObjects))
						{
							playModeStrippingHandledForSceneRootObjects.Add(gameObject.scene, new HashSet<Transform>(){ rootTransform });
						}
						else if(!handledRootObjects.Add(rootTransform))
						{
							#if DEV_MODE && DEBUG_AWAKE_STRIPPING
							Debug.Log("Aborting stripping for gameObject because scene " + scene.name + " not loaded and handledRootObjects already contained transform.root "+ rootTransform.name + ".");
							#endif
							return;
						}

						#if DEV_MODE && DEBUG_STRIP_SCENE
						Debug.Log("CheckForAndRemoveHierarchyFoldersInChildren("+gameObject.transform.root.name+").");
						#endif

						HierarchyFolderUtility.CheckForAndRemoveHierarchyFoldersInChildren(gameObject.transform.root, playModeStripping);
						return;
					}

					#if DEV_MODE && DEBUG_STRIP_SCENE
					Debug.Log("Scene "+scene.name+" was not loaded yet...");
					#endif
					
					// If entire scene is loaded, then can just strip the entire scene fully, as it should be safe to do so.
					break;
			}

			playModeStrippingHandledForScenes.Add(scene);

			try
			{
				HierarchyFolderUtility.ApplyStrippingType(scene, playModeStripping);
			}
			catch(System.ArgumentException e) // catch "ArgumentException: The scene is not loaded." which can occur when using PlayModeStrippingMethod.EntireSceneImmediate.
			{
				Debug.LogError("Exception encountered while stripping Hierarchy Folders from scene " + scene.name + " for play mode using method "+ playModeStripping + ". You may need to switch to a different play mode stripping method in preferences.\n" + e);
				playModeStrippingHandledForScenes.Remove(scene);

				#if DEV_MODE
				Debug.Assert(!HierarchyFolderUtility.NowStripping);
				#endif
			}
		}

		/// <summary>
		/// Called once a scene has fully finished loading. This happens AFTER Awake methods have been called.
		/// This methods serves to purposes:
		/// 1. When Play Mode Stripping Method is set to Entire Scene When Loaded, this is the main way that scenes are stripped.
		/// 2. When Play Mode Stripping Method is set to Individually During Awake, this might handles stripping some inactive HierarchyFolders Awake methods were not fired for handling them.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="mode"></param>
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			#if DEV_MODE
			Debug.Assert(scene.isLoaded);
			#endif

			if(playModeStripping == StrippingType.None)
			{
				return;
			}

			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			if(!playModeStrippingHandledForScenes.Add(scene))
			{
				return;
			}

			HierarchyFolderUtility.ApplyStrippingType(scene, playModeStripping);
		}

		private void OnSceneUnloaded(Scene scene)
		{
			playModeStrippingHandledForScenes.Remove(scene);
			playModeStrippingHandledForSceneRootObjects.Remove(scene);
		}
	}
}
#endif