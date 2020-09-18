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
        public EnvironmentItemData defaultTile;

        public LevelTileItem[,] grid;

        #region Odin

#if UNITY_EDITOR

        private void UpdateGrid()
        {
            //Sanity
            if (width < 0) width = 0;
            if (height < 0) height = 0;

            grid = new LevelTileItem[width, height];
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    grid[x, y].environmentItem = defaultTile;
                }
            }
        }

#endif
        #endregion Odin
    }
}
