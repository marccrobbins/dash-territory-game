using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Sisus.HierarchyFolders
{
	[InitializeOnLoad]
	public static class HierarchyFolderIconDrawer
	{
		private static readonly Texture folderIconOpen;
		private static readonly Texture folderIconClosed;

		private static List<int> expandedIDs = new List<int>();

		private static object treeViewState;
		private static PropertyInfo expandedIDsProperty;
		private static Type sceneHierarchyWindowType;

		static HierarchyFolderIconDrawer()
		{
			var preferences = HierarchyFolderPreferences.Get();

			var icon = preferences.Icon();
			folderIconOpen = icon.open;
			folderIconClosed = icon.closed;

			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

			if(!preferences.enableHierarchyIcons && !preferences.doubleClickSelectsChildrens)
			{
				return;
			}

			if(EditorApplication.isPlayingOrWillChangePlaymode)
			{
				switch(preferences.playModeBehaviour)
				{
					case StrippingType.RemoveComponent:
					case StrippingType.FlattenHierarchyAndRemoveComponent:
					case StrippingType.FlattenHierarchyAndRemoveGameObject:
						return;
				}
			}

			if(preferences.enableHierarchyIcons)
			{
				if(preferences.doubleClickSelectsChildrens)
				{
					EditorApplication.hierarchyWindowItemOnGUI += HandleDrawIconAndDoubleClickToSelectChildren;
				}
				else
				{
					EditorApplication.hierarchyWindowItemOnGUI += HandleDrawIcon;
				}
			}
			else
			{
				EditorApplication.hierarchyWindowItemOnGUI += HandleDoubleClickToSelectChildren;
			}

			EditorApplication.hierarchyChanged += OnHierarchyChanged;
			
			UpdateExpandedIDs();
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
		{
			if(playModeState == PlayModeStateChange.ExitingPlayMode)
			{
				ResubscribeToEvents();
			}
		}

		public static void ResubscribeToEvents()
		{
			var preferences = HierarchyFolderPreferences.Get();

			if(!preferences.enableHierarchyIcons && !preferences.doubleClickSelectsChildrens)
			{
				return;
			}

			if(preferences.enableHierarchyIcons)
			{
				if(preferences.doubleClickSelectsChildrens)
				{
					EditorApplication.hierarchyWindowItemOnGUI -= HandleDrawIconAndDoubleClickToSelectChildren;
					EditorApplication.hierarchyWindowItemOnGUI += HandleDrawIconAndDoubleClickToSelectChildren;
				}
				else
				{
					EditorApplication.hierarchyWindowItemOnGUI -= HandleDrawIcon;
					EditorApplication.hierarchyWindowItemOnGUI += HandleDrawIcon;
				}
				EditorApplication.hierarchyChanged -= OnHierarchyChanged;
				EditorApplication.hierarchyChanged += OnHierarchyChanged;
			}
			else if(preferences.doubleClickSelectsChildrens)
			{
				EditorApplication.hierarchyWindowItemOnGUI -= HandleDoubleClickToSelectChildren;
				EditorApplication.hierarchyWindowItemOnGUI += HandleDoubleClickToSelectChildren;

				EditorApplication.hierarchyChanged -= OnHierarchyChanged;
				EditorApplication.hierarchyChanged += OnHierarchyChanged;
			}
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		private static void UpdateExpandedIDs()
		{
			if(expandedIDsProperty == null)
			{
				if(sceneHierarchyWindowType == null)
				{
					var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
					sceneHierarchyWindowType = unityEditorAssembly.GetType("UnityEditor.SceneHierarchyWindow");
				}

				var hierarchyWindow = HierarchyWindowUtility.GetHierarchyWindow();
				if(hierarchyWindow == null)
				{
					Selection.selectionChanged -= OnSelectionChanged;
					Selection.selectionChanged += OnSelectionChanged;
					return;
				}

				var sceneHierarchyProperty = sceneHierarchyWindowType.GetProperty("sceneHierarchy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if(sceneHierarchyProperty == null)
				{
					return;
				}
				var sceneHierarchy = sceneHierarchyProperty.GetValue(hierarchyWindow, null);

				var treeViewStateProperty = sceneHierarchy.GetType().GetProperty("treeViewState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if(treeViewStateProperty == null)
				{
					return;
				}
				treeViewState = treeViewStateProperty.GetValue(sceneHierarchy, null);

				expandedIDsProperty = treeViewState.GetType().GetProperty("expandedIDs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}

			if(expandedIDsProperty != null)
			{
				expandedIDs = (List<int>)expandedIDsProperty.GetValue(treeViewState, null);
			}
		}

		private static void OnHierarchyChanged()
		{
			UpdateExpandedIDs();
		}

		private static void OnSelectionChanged()
		{
			Selection.selectionChanged -= OnSelectionChanged;
			UpdateExpandedIDs();
		}
		
		private static void HandleDrawIconAndDoubleClickToSelectChildren(int instanceId, Rect itemRect)
		{
			switch(Event.current.type)
			{
				case EventType.Repaint:
					DrawIcon(instanceId, itemRect);
					return;
				case EventType.MouseDown:
					if(Event.current.clickCount == 2 && itemRect.Contains(Event.current.mousePosition))
					{
						SelectChildrenIfIsHierarchyFolder(instanceId);
					}
					return;
			}
		}
		
		private static void HandleDrawIcon(int instanceId, Rect itemRect)
		{
			if(Event.current.type == EventType.Repaint)
			{
				DrawIcon(instanceId, itemRect);
			}
		}

		private static void HandleDoubleClickToSelectChildren(int instanceId, Rect itemRect)
		{
			if(Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
			{
				SelectChildrenIfIsHierarchyFolder(instanceId);
			}
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		private static void DrawIcon(int instanceId, Rect itemRect)
		{
			var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			if(gameObject != null && gameObject.IsHierarchyFolder())
			{
				var iconRect = itemRect;
				iconRect.width = itemRect.height;
				GUI.DrawTexture(iconRect, expandedIDs.IndexOf(instanceId) == -1 ? folderIconClosed : folderIconOpen);
			}
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		private static void SelectChildrenIfIsHierarchyFolder(int instanceId)
		{
			var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			if(gameObject != null && gameObject.transform.childCount > 0 && gameObject.IsHierarchyFolder())
			{
				Event.current.Use();
				var children = gameObject.GetChildren(false);

				expandedIDs.Add(instanceId);
				Expand(instanceId, children);

				Selection.objects = children;
			}
		}

		private static void Expand(int instanceId, GameObject[] children)
		{
			if(sceneHierarchyWindowType == null)
			{
				var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
				sceneHierarchyWindowType = unityEditorAssembly.GetType("UnityEditor.SceneHierarchyWindow");
			}
			var window = EditorWindow.focusedWindow;
			if(window == null || window.GetType() != sceneHierarchyWindowType)
			{
				return;
			}
			var setExpandedRecursiveMethod = sceneHierarchyWindowType.GetMethod("SetExpandedRecursive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(setExpandedRecursiveMethod == null)
			{
				return;
			}

			EditorApplication.hierarchyChanged -= OnHierarchyChanged;

			var parameters = new object[] { instanceId, true };
			setExpandedRecursiveMethod.Invoke(window, parameters);

			parameters[1] = false;
			for(int n = children.Length - 1; n >= 0; n--)
			{
				parameters[0] = children[n].GetInstanceID();
				setExpandedRecursiveMethod.Invoke(window, parameters);
			}

			EditorApplication.hierarchyChanged += OnHierarchyChanged;

			UpdateExpandedIDs();
		}
	}
}