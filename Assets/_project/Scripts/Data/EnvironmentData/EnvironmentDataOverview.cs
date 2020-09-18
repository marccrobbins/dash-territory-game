using System;
using System.Collections.Generic;
using Framework.GameData;
using Sirenix.OdinInspector.Demos.RPGEditor;
using UnityEngine;

namespace DashTerritory
{
    [CreateAssetMenu(menuName = "GameData/EnvironmentDataOverview", fileName = "EnvironmentDataOverview")]
    public class EnvironmentDataOverview : GameDataManager
    {
        public override Type ChildType => typeof(EnvironmentItemData);
        public override string ChildrenDirectory => "Assets/_project/Data/GameData/Environment";
        public override bool IsDraggable => true;

        public List<EnvironmentItemData> environmentPieces;

        public override ScriptableObject CreateNew()
        {
            if (environmentPieces == null) environmentPieces = new List<EnvironmentItemData>();
            
            ScriptableObject newObject = null;
            ScriptableObjectCreator.ShowDialog<EnvironmentItemData>(ChildrenDirectory, obj =>
            {
                obj.name = ((UnityEngine.Object) obj).name;
                newObject = obj;
                environmentPieces.Add(obj);
            });
            
            return newObject;
        }
    }
}
