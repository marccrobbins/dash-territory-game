using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashTerritory
{
    public struct LevelTileItem
    {
        public bool isNotPopulated;
        public int elevationLevel;
        public EnvironmentTile tile;
        public EnvironmentModifier modifier;
        //public Dictionary<string, EnvironmentModifier> modifiers;
    }
}
