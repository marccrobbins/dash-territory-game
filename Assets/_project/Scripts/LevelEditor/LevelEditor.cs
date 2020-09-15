using System;
using Framework.GameData;
using Sirenix.OdinInspector.Demos.RPGEditor;
using UnityEngine;

namespace DashTerritory
{
    [CreateAssetMenu(menuName = "GameData/LevelEditor", fileName = "LevelEditor")]
    public class LevelEditor : GameDataManager
    {
        public override Type childType => typeof(LevelData);
        public override string childrenDirectory => "Assets/_project/Data/GameData/Levels";

        public override ScriptableObject CreateNew()
        {
            ScriptableObject newObject = null;
            ScriptableObjectCreator.ShowDialog<LevelData>(childrenDirectory, obj => newObject = obj);

            return newObject;
        }
    }
}
