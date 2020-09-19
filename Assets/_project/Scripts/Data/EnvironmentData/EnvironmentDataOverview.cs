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
        public override Type ChildType => typeof(EnvironmentData);
        public override string ChildrenDirectory => "Assets/_project/Data/GameData/Environment";
        public override bool IsDraggable => true;

        public List<EnvironmentData> environmentPieces;

        public override ScriptableObject CreateNew()
        {
            if (environmentPieces == null) environmentPieces = new List<EnvironmentData>();
            
            ScriptableObject newObject = null;
            ScriptableObjectCreator.ShowDialog<EnvironmentData>(ChildrenDirectory, obj =>
            {
                obj.name = ((UnityEngine.Object) obj).name;
                newObject = obj;
                environmentPieces.Add(obj);
            });
            
            return newObject;
        }

        public override void Delete(ScriptableObject obj)
        {
            var data = (EnvironmentData) obj;
            environmentPieces.Remove(data);
        }
    }
}
