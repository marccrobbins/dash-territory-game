//#define DEBUG_ON_VALIDATE
//#define DEBUG_HIERARCHY_CHANGED
//#define DEBUG_AWAKE
//#define DEBUG_RESET_STATE

using UnityEngine;
using Sisus.Attributes;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using JetBrains.Annotations;
#endif

namespace Sisus.HierarchyFolders
{
	[AddComponentMenu("Hierarchy/Hierarchy Folder")]
	[HideTransformInInspector, HideComponentInInspector, OnlyComponent]
	#if UNITY_2018_3_OR_NEWER
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	public class HierarchyFolder : MonoBehaviour
	{
		#if UNITY_EDITOR
		private static readonly List<Component> ReusableComponentsList = new List<Component>(2);

		[UsedImplicitly]
		private void Reset()
		{
			if(HasSupernumeraryComponents())
			{
				Debug.LogWarning("Can't convert GameObject with extraneous components into a Hierarchy Folder.");
				TurnIntoNormalGameObject();
				return;
			}

			#if UNITY_2018_3_OR_NEWER
			if(PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab)
			#else
			var prefabType = PrefabUtility.GetPrefabType(gameObject);
			if(prefabType == PrefabType.Prefab)
			#endif
			{
				Debug.LogWarning("Can't convert prefabs into a Hierarchy Folder. Hierarchy Folders can only exist in the scene.");
				TurnIntoNormalGameObject();
				return;
			}

			#if UNITY_2018_3_OR_NEWER
			if(PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected)
			#else
			if(prefabType == PrefabType.PrefabInstance)
			#endif
			{
				Debug.LogWarning("Can't add HierarchyFolder component to a prefab instance. You need to unpack the prefab instance first.");
				TurnIntoNormalGameObject();
				return;
			}

			ResetTransformStateWithoutAffectingChildren();

			transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;

			hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
			EditorUtility.SetDirty(transform);
			gameObject.isStatic = true;
			EditorUtility.SetDirty(this);
			var preferences = HierarchyFolderPreferences.Get();
			if(preferences.autoNameOnAdd)
			{
				if(gameObject.name.Equals("GameObject", StringComparison.Ordinal) || gameObject.name.StartsWith("GameObject (", StringComparison.Ordinal))
				{
					gameObject.name = preferences.defaultName;
				}
				else
				{
					ApplyNamingPattern();
				}
			}

			EditorUtility.SetDirty(gameObject);
		}

		[UsedImplicitly]
		private void Awake()
		{
			#if DEV_MODE && DEBUG_AWAKE
			Debug.Log(name + ".Awake");
			#endif

			PlayModeStripper.OnSceneObjectAwake(gameObject);
		}

		private void ResubscribeToHierarchyChanged(HierarchyFolderPreferences preferences)
		{
			#if DEV_MODE && DEBUG_HIERARCHY_CHANGED
			Debug.Log(name + ".ResubscribeToHierarchyChanged");
			#endif

			UnsubscribeToHierarchyChanged(preferences);
			
			preferences.onPreferencesChanged += ResubscribeToHierarchyChanged;

			#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui += OnSceneGUI;
			#else
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			#endif

			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.hierarchyChanged += OnHierarchyChangedInEditMode;
			}
			else
			{
				if(preferences.playModeBehaviour == StrippingType.FlattenHierarchy)
				{
					EditorApplication.hierarchyChanged += OnHierarchyChangedInPlayModeFlattened;
				}
				else
				{
					EditorApplication.hierarchyChanged += OnHierarchyChangedInPlayModeGrouped;
				}
			}
		}

		private void OnSceneGUI(SceneView sceneView)
		{
			if(this == null)
			{
				#if UNITY_2019_1_OR_NEWER
				SceneView.duringSceneGui -= OnSceneGUI;
				#else
				SceneView.onSceneGUIDelegate -= OnSceneGUI;
				#endif
				return;
			}

			switch(Event.current.type)
			{
				case EventType.MouseUp:
				case EventType.DragExited:
				case EventType.DragPerform:
				case EventType.MouseLeaveWindow:
				case EventType.Ignore:
				case EventType.Used:
					// Skip prefab instances to avoid exceptions from SetParent.
					#if UNITY_2018_3_OR_NEWER
					if(PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected)
					#else
					if(PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance)
					#endif
					{
						return;
					}

					ResetTransformStateWithoutAffectingChildren();
					return;
			}
		}

		private void UnsubscribeToHierarchyChanged(HierarchyFolderPreferences preferences)
		{
			preferences.onPreferencesChanged -= ResubscribeToHierarchyChanged;
			EditorApplication.hierarchyChanged -= OnHierarchyChangedInEditMode;
			EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeFlattened;
			EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeGrouped;

			#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui -= OnSceneGUI;
			#else
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			#endif
		}

		[UsedImplicitly]
		private void OnValidate()
		{
			#if DEV_MODE && DEBUG_ON_VALIDATE
			Debug.Log(name + ".OnValidate");
			#endif

			if(this == null)
			{
				return;
			}

			ResubscribeToHierarchyChanged(HierarchyFolderPreferences.Get());
		}

		private void ResetTransformStateWithoutAffectingChildren()
		{
			#if DEV_MODE && DEBUG_RESET_STATE
			Debug.Log(name + ".ResetTransformStateWithoutAffectingChildren");
			#endif

			var transform = this.transform;

			if(!gameObject.activeSelf)
			{
				var preferences = HierarchyFolderPreferences.Get();

				if(preferences.askAboutAllowInactiveHierarchyFolders)
				{
					if(EditorUtility.DisplayDialog("Hierarchy Folder Active Flag Behaviour", "What would you like to happen when the active flag of a Hierarchy Folder is modified?\n\nTarget Hierarchy Folder:\nAdjust the active state of the Hierarchy Folder itself. This will have no effect in the final build since all Hierarchy Folders will be stripped even if inactive.\n\nTarget Children:\nModify active state of all child Objects and keep the Hierarchy Folder itself always active.\nThis is the recommended behaviour.\n\n(You can change your choice at any time in the preferences.)", "Target Hierarchy Folder", "Target Children"))
					{
						preferences.allowInactiveHierarchyFolders = true;
					}
					else
					{
						preferences.allowInactiveHierarchyFolders = false;
					}
					preferences.askAboutAllowInactiveHierarchyFolders = false;
					preferences.SaveState();
				}

				if(!preferences.allowInactiveHierarchyFolders)
				{
					Undo.RegisterFullObjectHierarchyUndo(gameObject, "Toggle Hierarchy Folder Children Active");

					gameObject.SetActive(true);

					int childCount = transform.childCount;
					if(childCount > 0)
					{
						var children = gameObject.GetChildren(true);
						childCount = children.Length;
						if(childCount > 0)
						{
							var firstChild = children[0];
							bool setActive = !firstChild.activeSelf;
							firstChild.SetActive(setActive);

							for(int n = 1; n < childCount; n++)
							{
								children[n].SetActive(setActive);
							}
						}
					}
				}
			}

			var rectTransform = transform as RectTransform;
			if(transform.localPosition != Vector3.zero || transform.localEulerAngles != Vector3.zero || transform.localScale != Vector3.one || (rectTransform != null && (rectTransform.anchorMin != Vector2.zero || rectTransform.anchorMax != Vector2.one || rectTransform.pivot != new Vector2(0.5f, 0.5f) || rectTransform.offsetMin != Vector2.zero || rectTransform.offsetMax != Vector2.zero)))
			{
				Undo.RegisterFullObjectHierarchyUndo(gameObject, "Reset Hierarchy Folder Transform");
				ForceResetTransformStateWithoutAffectingChildren(false);
			}
		}

		private void ForceResetTransformStateWithoutAffectingChildren(bool alsoConvertToRectTransform)
		{
			var transform = this.transform;

			// For non-prefab instances we can use a method where children are unparented temporarily.
			// This has the benefit the the world position of all children remains unchanged throughout the whole process.
			var parent = transform.parent;
			int childCount = transform.childCount;
			var children = new Transform[childCount];
			for(int n = childCount - 1; n >= 0; n--)
			{
				children[n] = transform.GetChild(n);

				// NOTE: Using SetParent with worldPositionStays true is very important (even with RectTransforms).
				children[n].SetParent(parent, true);
			}

			RectTransform rectTransform;
			if(alsoConvertToRectTransform)
			{
				rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
				transform = rectTransform;
			}
			else
			{
				rectTransform = transform as RectTransform;
			}

			if(rectTransform != null)
			{
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.offsetMin = Vector2.zero;
				rectTransform.offsetMax = Vector2.zero;
			}

			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
			transform.localScale = Vector3.one;

			for(int n = 0; n < childCount; n++)
			{
				children[n].SetParent(transform, true);
				children[n].SetAsLastSibling();
			}

			EditorUtility.SetDirty(transform);
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			UnsubscribeToHierarchyChanged(HierarchyFolderPreferences.Get());
		}

		private void OnHierarchyChangedInEditMode()
		{
			if(this == null)
			{
				EditorApplication.hierarchyChanged -= OnHierarchyChangedInEditMode;
				return;
			}

			// Make sure that the user hasn't converted a hierarchy folder into a prefab instance.
			#if UNITY_2018_3_OR_NEWER
			if(PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected)
			#else
			if(PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance)
			#endif
			{
				Debug.LogWarning("Prefab instances can't be Hierarchy Folders. Converting into a normal GameObject.", gameObject);
				TurnIntoNormalGameObject();
				return;
			}

			// If has RectTransform child convert Transform component into RectTransform 
			// to avoid child RectTransform values being affected by the parent hierarchy folders.
			// For performance reasons only first child is checked.
			if(transform.GetFirstChild(true) is RectTransform && !(transform is RectTransform))
			{
				#if DEV_MODE
				Debug.LogWarning("Converting Hierarchy Folder " + name + " Transform into RectTransform because it had a RectTransform child.", gameObject);
				#endif

				ForceResetTransformStateWithoutAffectingChildren(true);
			}

			OnHierarchyChangedShared();

			ApplyNamingPattern();
		}

		private void OnHierarchyChangedInPlayModeFlattened()
		{
			if(this == null)
			{
				EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeFlattened;
				return;
			}

			OnHierarchyChangedShared();

			#if DEV_MODE
			if(transform.childCount > 0)
			{
				if(HierarchyFolderUtility.NowStripping) { Debug.LogWarning(name + " child count is "+ transform.childCount+" but won't flatten because HierarchyFolderUtility.NowStripping already true.", gameObject); }
				else { Debug.Log(name + " child count " + transform.childCount+". Flattening now...", gameObject); }
			}
			#endif

			if(transform.childCount > 0 && !HierarchyFolderUtility.NowStripping)
			{
				int moveToIndex = HierarchyFolderUtility.GetLastChildIndexInFlatMode(gameObject);
				for(int n = transform.childCount - 1; n >= 0; n--)
				{
					var child = transform.GetChild(n);
					child.SetParent(null, true);
					child.SetSiblingIndex(moveToIndex);
				}
			}
		}

		private void OnHierarchyChangedInPlayModeGrouped()
		{
			if(this == null)
			{
				EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeGrouped;
				return;
			}

			OnHierarchyChangedShared();
		}

		private void OnHierarchyChangedShared()
		{
			#if DEV_MODE && DEBUG_HIERARCHY_CHANGED
			Debug.Log(name + ".OnHierarchyChangedShared");
			#endif

			if(HasSupernumeraryComponents())
			{
				Debug.LogWarning("Hierarchy Folder \"" + name + "\" contained extraneous components.\nThis is not supported since Hierarchy Folders are stripped from builds. Converting into a normal GameObject now.", gameObject);

				#if DEV_MODE
				foreach(var component in gameObject.GetComponents<Component>())
				{
					Debug.Log(component.GetType().Name);
				}
				#endif

				TurnIntoNormalGameObject();
				return;
			}

			ResetTransformStateWithoutAffectingChildren();
		}

		private bool HasSupernumeraryComponents()
		{
			GetComponents(ReusableComponentsList);
			// A hierarchy folder GameObject should only have Transform (or RectTransform) and HierarchyFolder components.
			bool tooManyComponents = ReusableComponentsList.Count > 2;
			ReusableComponentsList.Clear();
			return tooManyComponents;
		}

		[ContextMenu("Turn Into Normal GameObject")]
		private void TurnIntoNormalGameObject()
		{
			UnsubscribeToHierarchyChanged(HierarchyFolderPreferences.Get());

			// Can help avoid NullReferenceExceptions via hierarchyChanged callback
			// by adding a delay between the unsubscribing and the destroying of the HierarchyFolder component
			EditorApplication.delayCall += UnmakeHierarchyFolder;
		}

		private void UnmakeHierarchyFolder()
		{
			HierarchyFolderUtility.UnmakeHierarchyFolder(gameObject, this);
		}

		private void ApplyNamingPattern()
		{
			var preferences = HierarchyFolderPreferences.Get();
			if(!preferences.enableNamingRules)
			{
				return;
			}

			string setName = gameObject.name;
			bool possiblyChanged = false;

			if(preferences.forceNamesUpperCase)
			{
				setName = setName.ToUpper();
				possiblyChanged = true;
			}

			string prefix = preferences.namePrefix;
			if(!setName.StartsWith(prefix, StringComparison.Ordinal))
			{
				possiblyChanged = true;

				if(setName.StartsWith(preferences.previousNamePrefix, StringComparison.Ordinal))
				{
					setName = setName.Substring(preferences.previousNamePrefix.Length);
				}

				for(int c = prefix.Length - 1; c >= 0 && !setName.StartsWith(prefix, StringComparison.Ordinal); c--)
				{
					setName = prefix[c] + setName;
				}
			}

			string suffix = preferences.nameSuffix;
			if(!setName.EndsWith(suffix, StringComparison.Ordinal))
			{
				possiblyChanged = true;

				// Handle situation where a hierarchy folder has been duplicated and a string like "(1)"
				// has been added to the end of the name.
				if(setName.EndsWith(")", StringComparison.Ordinal))
				{
					int openParenthesis = setName.LastIndexOf(" (", StringComparison.Ordinal);
					if(openParenthesis != -1)
					{
						string ending = setName.Substring(openParenthesis);
						if(ending.Length <= 5 && setName.EndsWith(suffix + ending, StringComparison.Ordinal))
						{
							int from = openParenthesis + 2;
							int to = setName.Length - 1;
							string nthString = setName.Substring(from, to - from);
							int nthInt;
							if(int.TryParse(nthString, out nthInt))
							{
								setName = setName.Substring(0, openParenthesis - suffix.Length) + suffix;
							}
						}
					}
				}

				if(setName.EndsWith(preferences.previousNameSuffix, StringComparison.Ordinal))
				{
					setName = setName.Substring(0, setName.Length - preferences.previousNameSuffix.Length);
				}

				for(int c = 0, count = suffix.Length; c < count && !setName.EndsWith(suffix, StringComparison.Ordinal); c++)
				{
					setName += suffix[c];
				}
			}

			if(possiblyChanged && !string.Equals(setName, gameObject.name))
			{
				gameObject.name = setName;
			}
		}
		#endif
	}
}