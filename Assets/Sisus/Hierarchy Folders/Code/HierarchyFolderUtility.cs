//#define DEBUG_REMOVE_HIERARCHY_FOLDER
#define DEBUG_STRIP_SCENE
//#define DEBUG_STRIP_FOLDER
#define DEBUG_UNMAKE_HIERARCHY_FOLDER

#define ASSERT_COMPONENT_COUNT
//#define ASSERT_CHILD_COUNT

#if UNITY_EDITOR
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Sisus.HierarchyFolders
{
	public static class HierarchyFolderUtility
	{
		public static bool NowStripping
		{
			get;
			private set;
		}

		private static readonly List<GameObject> RootGameObjects = new List<GameObject>(100);

		public static void ApplyStrippingTypeToAllLoadedScenes(StrippingType strippingType)
		{
			for(int s = 0, scount = SceneManager.sceneCount; s < scount; s++)
			{
				ApplyStrippingType(SceneManager.GetSceneAt(s), strippingType);
			}
		}

		public static void ApplyStrippingType(Scene scene, StrippingType strippingType)
		{
			#if DEV_MODE && DEBUG_STRIP_SCENE
			Debug.Assert(RootGameObjects.Count == 0);
			#endif

			scene.GetRootGameObjects(RootGameObjects);

			#if DEV_MODE && DEBUG_STRIP_SCENE
			Debug.Log("Stripping " + RootGameObjects.Count + " root objects in scene " + scene.name + "...");
			#endif

			for(int n = 0, count = RootGameObjects.Count; n < count; n++)
			{
				CheckForAndRemoveHierarchyFoldersInChildren(RootGameObjects[n].transform, strippingType);
			}
			RootGameObjects.Clear();

			#if DEV_MODE && DEBUG_STRIP_SCENE
			Debug.Log("Stripping scene "+scene.name+" done.");
			#endif
		}

		public static void CheckForAndRemoveHierarchyFoldersInChildren([NotNull]Transform transform, StrippingType strippingType)
		{
			bool wasStripping = NowStripping;
			NowStripping = true;

			int childCount = transform.childCount;
			var children = new Transform[childCount];
			for(int n = 0; n < childCount; n++)
			{
				children[n] = transform.GetChild(n);
			}

			var hierarchyFolder = transform.GetComponent<HierarchyFolder>();
			if(hierarchyFolder != null)
			{
				var hierarchyFolderParent = transform.parent;
				int setSiblingIndex = transform.GetSiblingIndex() + 1;

				#if DEV_MODE && DEBUG_STRIP_FOLDER
				Debug.Log("Stripping "+transform.name+" with "+childCount+" children...");
				#endif

				List<GameObject> setChildrenActiveDelayed = null;

				switch(strippingType)
				{
					case StrippingType.FlattenHierarchyAndRemoveGameObject:
					case StrippingType.FlattenHierarchy:
					case StrippingType.FlattenHierarchyAndRemoveComponent:
					case StrippingType.FlattenHierarchyAndDisableComponent:
						for(int n = 0; n < childCount; n++)
						{
							var child = children[n];

							if(!transform.gameObject.activeSelf && child.gameObject.activeSelf && Application.isPlaying)
							{
								if(setChildrenActiveDelayed == null)
								{
									setChildrenActiveDelayed = new List<GameObject>();
								}
								child.gameObject.SetActive(false);
								setChildrenActiveDelayed.Add(child.gameObject);
							}

							child.SetParent(hierarchyFolderParent, true);

							child.SetSiblingIndex(setSiblingIndex);
							setSiblingIndex++;
						}
						for(int n = 0; n < childCount; n++)
						{
							CheckForAndRemoveHierarchyFoldersInChildren(children[n], strippingType);
						}
						break;
					default:
						for(int n = 0; n < childCount; n++)
						{
							var child = children[n];
							CheckForAndRemoveHierarchyFoldersInChildren(child, strippingType);
						}
						break;
				}

				#if ASSERT_COMPONENT_COUNT
				var componentCount = transform.GetComponents<Component>().Length;
				if(componentCount != 2)
				{
					Debug.LogError("HierarchyFolder " + transform.name + " contained " + componentCount + " components! All components will be destroyed along with the Hierarchy Folder.");
				}
				#endif

				#if ASSERT_CHILD_COUNT
				Debug.Assert(transform.childCount == 0);
				#endif

				#if DEBUG_REMOVE_HIERARCHY_FOLDER
				Debug.Log("Destroying HierarchyFolder: "+ transform.name);
				#endif

				switch(strippingType)
				{
					case StrippingType.FlattenHierarchyAndRemoveGameObject:
						// Using DestroyImmediate instead of Destroy even in play mode so that other scripts won't be aware of the hierarchy folders ever having existed.
						Object.DestroyImmediate(transform.gameObject, true);
						break;
					case StrippingType.FlattenHierarchyAndRemoveComponent:
					case StrippingType.RemoveComponent:
						Object.DestroyImmediate(hierarchyFolder, true);
						break;
					case StrippingType.FlattenHierarchyAndDisableComponent:
					case StrippingType.DisableComponent:
						hierarchyFolder.enabled = false;
						transform.hideFlags = HideFlags.None;
						hierarchyFolder.hideFlags = HideFlags.None;
						break;
				}

				if(setChildrenActiveDelayed != null)
				{
					for(int n = 0, count = setChildrenActiveDelayed.Count; n < count; n++)
					{
						setChildrenActiveDelayed[n].SetActive(true);
					}
				}
			}
			else
			{
				for(int n = 0, count = children.Length; n < count; n++)
				{
					CheckForAndRemoveHierarchyFoldersInChildren(children[n], strippingType);
				}
			}

			NowStripping = wasStripping;
		}

		private static void Destroy(Object obj)
		{
			#if UNITY_EDITOR
			#if UNITY_2018_3_OR_NEWER
			if(!Application.isPlaying || UnityEditor.PrefabUtility.GetPrefabAssetType(obj) != UnityEditor.PrefabAssetType.NotAPrefab)
			#else
			if(!Application.isPlaying || UnityEditor.PrefabUtility.GetPrefabType(obj) == UnityEditor.PrefabType.Prefab)
			#endif
			{
				Object.DestroyImmediate(obj, true);
			}
			else
			#endif
			{
				Object.Destroy(obj);
			}
		}

		public static void UnmakeHierarchyFolder([NotNull]GameObject gameObject, [CanBeNull]HierarchyFolder hierarchyFolder)
		{
			#if DEV_MODE && DEBUG_UNMAKE_HIERARCHY_FOLDER
			Debug.Log("UnmakeHierarchyFolder("+gameObject.name+")");
			#endif

			if(hierarchyFolder != null)
			{
				#if UNITY_EDITOR
				#if UNITY_2018_3_OR_NEWER
				if(!Application.isPlaying || UnityEditor.PrefabUtility.GetPrefabAssetType(hierarchyFolder) != UnityEditor.PrefabAssetType.NotAPrefab)
				#else
				if(!Application.isPlaying || UnityEditor.PrefabUtility.GetPrefabType(hierarchyFolder) == UnityEditor.PrefabType.Prefab)
				#endif
				{
					Object.DestroyImmediate(hierarchyFolder, true);
				}
				else
				#endif
				{
					Object.Destroy(hierarchyFolder);
				}
			}

			var preferences = HierarchyFolderPreferences.Get();
			string setName = gameObject.name;
			string prefix = preferences.namePrefix;
			if(prefix.Length > 0 && setName.StartsWith(prefix, StringComparison.Ordinal))
			{
				setName = setName.Substring(0);
			}
			string suffix = preferences.nameSuffix;
			if(suffix.Length > 0 && setName.EndsWith(setName, StringComparison.Ordinal))
			{
				setName = setName.Substring(0, setName.Length - suffix.Length);
			}
			if(preferences.forceNamesUpperCase && setName.Length > 1)
			{
				setName = setName[0] + setName.Substring(1).ToLower();
			}

			if(!string.Equals(setName, gameObject.name))
			{
				gameObject.name = setName;
				#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(gameObject);
				#endif
			}

			var transform = gameObject.transform;
			transform.hideFlags = HideFlags.None;

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(transform);
			#endif
		}

		public static int GetLastChildIndexInFlatMode(GameObject hierarchyFolder)
		{
			var transform = hierarchyFolder.transform;
			int myIndex = transform.GetSiblingIndex();
			#if DEV_MODE
			Debug.Assert(myIndex >= 0, myIndex.ToString());
			#endif

			var parent = transform.parent;
			if(parent == null)
			{
				var scene = hierarchyFolder.scene;
				var root = scene.GetRootGameObjects();
				int rootCount = root.Length;

				for(int n = myIndex + 1; n < rootCount; n++)
				{
					if(root[n].IsHierarchyFolder())
					{
						#if DEV_MODE
						Debug.Assert(n > 0, n.ToString());
						#endif
						return n;
					}
				}
				return rootCount;
			}

			for(int n = myIndex + 1; n < parent.childCount; n++)
			{
				if(parent.GetChild(n).gameObject.IsHierarchyFolder())
				{
					return n;
				}
			}
			return parent.childCount;
		}

		public static void SetParent([NotNull]GameObject child, [CanBeNull]GameObject parent, bool worldPositionStays)
		{
			if(parent == null)
			{ 
				child.transform.SetParent(null, worldPositionStays);
				return;
			}

			#if UNITY_EDITOR
			if(HierarchyFolderPreferences.FlattenHierarchy && parent.IsHierarchyFolder())
			{
				int moveToIndex = GetLastChildIndexInFlatMode(parent);
				var parentOfFolder = parent.transform.parent;
				child.transform.SetParent(parentOfFolder, worldPositionStays);
				if(moveToIndex < 0)
				{
					#if DEV_MODE
					Debug.LogWarning("GetLastChildIndexInFlatMode result < 0");
					#endif
					return;
				}
				child.transform.SetSiblingIndex(moveToIndex);
				return;
			}
			#endif

			child.transform.SetParent(parent.transform, worldPositionStays);
		}
	}
}
#endif