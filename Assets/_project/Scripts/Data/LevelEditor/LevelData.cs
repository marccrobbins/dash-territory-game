using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class LevelData : SerializedScriptableObject
    {
        [OnValueChanged("UpdateGrid")]
        public int width;
        [OnValueChanged("UpdateGrid")]
        public int height;
        public float tileSize = 2;
        public EnvironmentTile defaultTile;

        public LevelTileItem[,] grid;

        private void OnEnable()
        {
#if UNITY_EDITOR
            AssignDefaults();
#endif
        }

        #region Odin

#if UNITY_EDITOR

        private void UpdateGrid()
        {
            //Sanity
            if (width < 0) width = 0;
            if (height < 0) height = 0;

            grid = new LevelTileItem[width, height];
            AssignDefaults();
        }

        private void AssignDefaults()
        {
            if (grid == null) return;
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var item = grid[x, y];
                    if (item.tile == null) item.tile = defaultTile;
                    //if (item.modifiers == null) item.modifiers = new Dictionary<string, EnvironmentModifier>();
                    grid[x, y] = item;
                }
            }
        }

#endif
        #endregion Odin
    }
}
