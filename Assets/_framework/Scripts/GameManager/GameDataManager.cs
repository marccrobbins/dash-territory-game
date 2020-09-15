using System;
using UnityEngine;
using Object = System.Object;

namespace Framework.GameData
{
    public abstract class GameDataManager : ScriptableObject 
    {
        public abstract Type childType { get; } 
        public abstract string childrenDirectory { get; }

        public abstract ScriptableObject CreateNew();
    }
}
