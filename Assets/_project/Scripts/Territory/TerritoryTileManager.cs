using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace DashTerritory
{
    public class TerritoryTileManager : Manager<TerritoryTileManager>
    {
        public GameObject territoryTilePrefab;
        private Dictionary<TerritoryGrid, List<TerritoryTile>> territoryTileLookup;
        
        protected override IEnumerator InitializeManager()
        {
            territoryTileLookup = new Dictionary<TerritoryGrid, List<TerritoryTile>>();
            
            return base.InitializeManager();
        }

        public void RegisterTerritoryGrid(TerritoryGrid grid)
        {
            territoryTileLookup[grid] = new List<TerritoryTile>();
            
            //instantiate all tiles
            foreach (var subdivision in grid.Subdivisions)
            {
                var tileObject = Instantiate(territoryTilePrefab, subdivision, grid.transform.rotation, transform);
                var territoryTile = tileObject.GetComponent<TerritoryTile>();
                if (!territoryTile)
                {
                    Destroy(tileObject);
                    continue;
                }
                
                territoryTileLookup[grid].Add(territoryTile);
            }
        }
    }
}
