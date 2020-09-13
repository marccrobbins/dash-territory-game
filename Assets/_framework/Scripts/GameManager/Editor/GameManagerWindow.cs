using System;
using System.Collections.Generic;
using Framework.Pooling;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.GameData.Editor
{
    public class GameManagerWindow : OdinMenuEditorWindow
    {
        [OnValueChanged("StateChange")]
        [LabelText("Manager View")]
        [LabelWidth(100f)]
        [EnumToggleButtons]
        [ShowInInspector]
        private ManagerState managerState; //used to control what is shown in the editor window
        private bool reBuildTree;
        
        [MenuItem("Tools/GameManagerWindow")]
        private static void OpenWindow()
        {
            GetWindow<GameManagerWindow>().Show();
        }

        protected override void Initialize()
        {
            //Find GameManagerWindowData object
            var dataInfo = GameManagerWindowInfo.Instance;

            //Debug.Log($"Init GameManagerWindow with {dataInfo.references.Length} managers");
        }

        //called when the enum "manager state" is changed
        //might need more in here for later additions?
        private void StateChange()
        {
            reBuildTree = true;
        }

        //used to place title and enum buttons (Target 0) above Odin Menu Tree
        //only used in some windows
        protected override void OnGUI()
        {
            //update menu tree on type change
            if (reBuildTree && Event.current.type == EventType.Layout)
            {
                ForceMenuTreeRebuild();
                reBuildTree = false;
            }

            SirenixEditorGUI.Title("Game Manager Window", "The manager that manages managers", TextAlignment.Center,
                true);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            switch (managerState)
            {
                case ManagerState.Pooling:
                    //DrawEditor(Enum.GetValues(typeof(ManagerState)).Length);
                    break;
            }

            EditorGUILayout.Space();
            base.OnGUI();
        }

        //targets are separate classes that wrap the main class
        //the idea here was to allow the addition of buttons and other functions
        //for use in the editor window that aren't needed in the class itself
        protected override IEnumerable<object> GetTargets()
        {
            var targets = new List<object>();
            //targets.Add(drawPooling); 
            targets.Add(base.GetTarget()); 

            return targets;
        }

        protected override void DrawEditors()
        {
            switch (managerState)
            {
                case ManagerState.Pooling:
                    //drawPooling.SetSelected(MenuTree.Selection.SelectedValue);
                    break;
            }

            //draw editor based on enum value
            DrawEditor((int) managerState);
        }

        //control over Odin Menu Tree
        protected override void DrawMenu()
        {
            switch (managerState)
            {
                case ManagerState.Pooling:
                    base.DrawMenu();
                    break;
            }
        }

        //I'm building multiple trees depending on what "state" is selected
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;

            switch (managerState)
            {
                case ManagerState.Pooling:
                    tree.AddAllAssetsAtPath("Module Data", "", typeof(PoolManager));
                    break;
            }

            return tree;
        }

        public enum ManagerState
        {
            Pooling,
            AnotherOne
        }
    }

    //Used to draw the current object that is selected in the Menu Tree
    //Look at me using generics ;)
    public class DrawSelected<T> where T : ScriptableObject
    {
        //[Title("@property.name")]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        public T selected;

        [LabelWidth(100)]
        [PropertyOrder(-1)]
        //[ColorGroupAttribute("CreateNew", 1f, 1f, 1f)]
        [HorizontalGroup("CreateNew/Horizontal")]
        public string nameForNew;

        private string path;

        [HorizontalGroup("CreateNew/Horizontal")]
        [GUIColor(0.7f, 0.7f, 1f)]
        [Button]
        public void CreateNew()
        {
            if (nameForNew == "")
                return;

            var newItem = ScriptableObject.CreateInstance<T>();
            newItem.name = "New " + typeof(T);

            if (path == "")
                path = "Assets/";

            AssetDatabase.CreateAsset(newItem, path + "\\" + nameForNew + ".asset");
            AssetDatabase.SaveAssets();

            nameForNew = "";
        }

        [HorizontalGroup("CreateNew/Horizontal")]
        [GUIColor(1f, 0.7f, 0.7f)]
        [Button]
        public void DeleteSelected()
        {
            if (selected == null) return;
            var path = AssetDatabase.GetAssetPath(selected);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }

        public void SetSelected(object item)
        {
            //ensure selection is of the correct type
            var attempt = item as T;
            if (attempt != null) selected = attempt;
        }

        public void SetPath(string path)
        {
            this.path = path;
        }
    }
}
