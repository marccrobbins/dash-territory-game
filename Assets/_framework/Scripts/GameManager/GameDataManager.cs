using System;
using UnityEngine;

namespace Framework.GameData
{
    public abstract class GameDataManager : ScriptableObject 
    {
        public abstract Type ChildType { get; } 
        public abstract string ChildrenDirectory { get; }
        public abstract bool IsDraggable { get; }

        public abstract ScriptableObject CreateNew();
    }
}
