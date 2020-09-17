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
        public LevelTileItem[,] grid;

        #region Odin

#if UNITY_EDITOR

        private void UpdateGrid()
        {
            //Sanity
            if (width < 0) width = 0;
            if (height < 0) height = 0;

            grid = new LevelTileItem[width, height];
            
            //Assign index
//            for (var x = 0; x < width; x++)
//            {
//                for (var y = 0; y < height; y++)
//                {
//                    grid[x, y].index = new Index
//                    {
//                        x = x, y = y
//                    };
//                }
//            }

            //Find neighbours
            for (var nx = Math.Max(0, width - 1); nx <= Math.Min(width + 1, 0); nx++)
            {
                for (var ny = Math.Max(0, height - 1); ny <= Math.Min(height + 1, 0); ny++)
                {
                    if (nx != width || ny != height)
                    {
                        
                    }
                }
            }
        }

#endif
        #endregion Odin
    }
}
