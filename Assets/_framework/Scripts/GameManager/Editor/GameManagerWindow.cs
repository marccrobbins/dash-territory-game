using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.GameData.Editor
{
    public class GameManagerWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/GameManagerWindow")]
        private static void OpenWindow()
        {
            GetWindow<GameManagerWindow>().Show();
        }
        
        protected override void Initialize()
        {
            //Find GameManagerWindowData object
            var dataInfo = GameManagerWindowInfo.Instance;
            
            Debug.Log($"Init GameManagerWindow with {dataInfo.references.Length} managers");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            return tree;
        }
    }
}
