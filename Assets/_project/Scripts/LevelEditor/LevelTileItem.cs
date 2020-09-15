using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashTerritory
{
    public struct LevelTileItem
    {
        public Index index;
        public bool isNotPopulated;
        public int elevationLevel;
        public List<Index> neighbours;

        //Add drag and droppable item for power up
    }

    public struct Index
    {
        public int x;
        public int y;
    }
}
