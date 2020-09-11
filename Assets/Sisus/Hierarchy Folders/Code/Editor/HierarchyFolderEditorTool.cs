// PreferencesApplier will make changes to this region based on preferences
#region ApplyPreferences
#define ENABLE_HIERARCHY_FOLDER_EDITOR_TOOL
#endregion

#if UNITY_2019_1_OR_NEWER
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
#endif

namespace Sisus.HierarchyFolders
{
	#if UNITY_2019_1_OR_NEWER
	#if ENABLE_HIERARCHY_FOLDER_EDITOR_TOOL
	[EditorTool("Hierarchy Folder")]
	#endif
	public class HierarchyFolderEditorTool : EditorTool
	{
		private GUIContent icon;

		private MethodInfo getActiveTool;
		private MethodInfo restorePreviousTool;

		public override GUIContent toolbarIcon
		{
			get { return icon; }
		}

		public override bool IsAvailable()
		{
			return true;
		}

		/// <summary>
		/// This is initialized on load due to the usage of the InitializeOnLoad attribute.
		/// </summary>
		static HierarchyFolderEditorTool()
		{
			EditorApplication.delayCall += ApplyPreferencesWhenAssetDatabaseReady;
		}

		private static void ApplyPreferencesWhenAssetDatabaseReady()
		{
			if(!PreferencesApplier.ReadyToApplyPreferences())
			{
				EditorApplication.delayCall += ApplyPreferencesWhenAssetDatabaseReady;
				return;
			}

			var classType = typeof(HierarchyFolderEditorTool);
			var preferences = HierarchyFolderPreferences.Get();
			bool enabled = preferences.enableToolbarIcon;

			PreferencesApplier.ApplyPreferences(classType,
			new[] { "#define ENABLE_HIERARCHY_FOLDER_EDITOR_TOOL" },
			new[] { enabled });

			preferences.onPreferencesChanged += (changedPreferences)=>
			{
				if(changedPreferences.enableToolbarIcon != enabled)
				{
					var script = PreferencesApplier.FindScriptFile(classType);
					if(script != null)
					{
						AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(script));
					}
					#if DEV_MODE
					else { Debug.LogWarning("Could not find script asset "+classType.Name+".cs"); }
					#endif
				}
			};
		}

		private void OnEnable()
		{
			icon = new GUIContent(HierarchyFolderPreferences.Get().GetDefaultIcon(18).closed, "Create Hierarchy Folder");
			EditorApplication.update += Update;
			var editorToolcontextType = typeof(EditorTool).Assembly.GetType("UnityEditor.EditorTools.EditorToolContext");
			getActiveTool = editorToolcontextType.GetMethod("GetActiveTool", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			restorePreviousTool = editorToolcontextType.GetMethod("RestorePreviousTool", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		}

		private void Update()
		{
			if(this == GetActiveTool())
			{
				OnActivate();
			}
		}

		private EditorTool GetActiveTool()
		{
			return getActiveTool.Invoke(null, null) as EditorTool;
		}

		// Called when an EditorTool is made the active tool.
		private void OnActivate()
		{
			RestorePreviousTool();

			var window = HierarchyWindowUtility.GetHierarchyWindow();
			if(window != null)
			{
				window.Focus();
				EditorApplication.delayCall += StartRenamingCreatedHierarchyFolder;
			}

			HierarchyFolderMenuItems.CreateHierarchyFolder();
		}

		private void StartRenamingCreatedHierarchyFolder()
		{
			var window = HierarchyWindowUtility.GetHierarchyWindow();
			if(EditorWindow.focusedWindow != window)
			{
				window.Focus();
				EditorApplication.delayCall += StartRenamingCreatedHierarchyFolder;
				return;
			}

			var sendEvent = Event.KeyboardEvent("F2");
			Event.current = sendEvent;
			window.SendEvent(sendEvent);

			EditorApplication.delayCall += EnsureRenamingCreatedHierarchyFolder;
		}

		private void EnsureRenamingCreatedHierarchyFolder()
		{
			var window = HierarchyWindowUtility.GetHierarchyWindow();
			if(EditorWindow.focusedWindow != window)
			{
				window.Focus();
				StartRenamingCreatedHierarchyFolder();
				return;
			}
		}

		private void RestorePreviousTool()
		{
			restorePreviousTool.Invoke(null, null);
		}
	}
	#endif
}