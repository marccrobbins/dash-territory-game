using System;
using System.Collections.Generic;
using Framework.GameData;
using Sirenix.OdinInspector.Demos.RPGEditor;
using UnityEngine;

namespace DashTerritory
{
    [CreateAssetMenu(menuName = "GameData/LevelDataOverview", fileName = "LevelEditorOverview")]
    public class LevelDataOverview : GameDataManager
    {
        public override Type ChildType => typeof(LevelData);
        public override string ChildrenDirectory => "Assets/_project/Data/GameData/Levels";
        public override bool IsDraggable => false;

        public List<LevelData> levels;

        public override ScriptableObject CreateNew()
        {
            if (levels == null) levels = new List<LevelData>();
            
            ScriptableObject newObject = null;
            ScriptableObjectCreator.ShowDialog<LevelData>(ChildrenDirectory, obj =>
            {
                newObject = obj;
                levels.Add(obj);
            });

            return newObject;
        }
    }
}
