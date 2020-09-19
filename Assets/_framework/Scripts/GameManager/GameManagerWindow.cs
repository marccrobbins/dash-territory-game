
using System;
using Sirenix.Utilities;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.GameData
{
    public class GameManagerWindow : OdinMenuEditorWindow
    {
        private GameManagerWindowInfo windowInfo;

        [MenuItem("Tools/GameManagerWindow")]
        private static void OpenWindow()
        {
            GetWindow<GameManagerWindow>().Show();
        }

        protected override void Initialize()
        {
            //Find GameManagerWindowData object
            windowInfo = GameManagerWindowInfo.Instance;
        }

        protected override void OnGUI()
        {
            if (windowInfo && MenuTree != null)
            {
                windowInfo.MenuWidth = MenuWidth;
                windowInfo.MenuSearchBarHeight = MenuTree.Config.SearchToolbarHeight;
            }

            base.OnGUI();
        }

        //I'm building multiple trees depending on what "state" is selected
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;
            tree.DefaultMenuStyle.Height = 40;
            tree.DefaultMenuStyle.IconSize = 36;
            tree.Config.DrawSearchToolbar = true;

            //Make sure we always have data
            if (!windowInfo) Initialize();

            foreach (var manager in windowInfo.gameDataManagers)
            {
                if (manager == null) continue;

                tree.Add(manager.name, manager);

                //check if we need to add children to the tree
                if (string.IsNullOrEmpty(manager.ChildrenDirectory)) continue;
                tree.AddAllAssetsAtPath(manager.name, manager.ChildrenDirectory, manager.ChildType, true);

                tree.EnumerateTree().Where(x => x.Value.GetType().IsSubclassOf(manager.ChildType)).ForEach(AddDragHandles);
            }

            return tree;
        }
        
        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }

        private GameDataManager activeManager;
        protected override void OnBeginDrawEditors()
        {
            if (MenuTree?.Selection == null) return;
            
            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected == null) return;

                GUILayout.Label(selected.Name);

                //Find current selected manager
                var isParent = selected.Parent == null;
                var objectValue = isParent ? selected.Value : selected.Parent.Value;
                activeManager = (GameDataManager) objectValue;
                
                if (activeManager == null) return;
                
                var hasChildren = !string.IsNullOrEmpty(activeManager.ChildrenDirectory) && activeManager.ChildType != null;
                if (hasChildren)
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create")))
                    {
                        TrySelectMenuItemWithObject(activeManager.CreateNew());
                    }

                    if (!isParent)
                    {
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent("Delete")))
                        {
                            var path = AssetDatabase.GetAssetPath((Object) selected.Value);
                            
                            if (AssetDatabase.DeleteAsset(path))
                            {
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            }
                        }
                    }
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
#endif
