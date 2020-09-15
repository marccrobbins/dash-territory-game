using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class LevelBuilder : MonoBehaviour
    {
        public LevelData levelData;
        public GameObject territoryTilePrefab;

        private TerritoryTile[,] neighbourGrid;
        [Button]
        private void Build()
        {
            neighbourGrid = new TerritoryTile[levelData.width, levelData.height];
            var scale = territoryTilePrefab.transform.localScale;
            var startingX = 0;//levelData.width * 0.5f * (scale.x * 0.5f);
            var startingY = 0;//levelData.height * 0.5f * (scale.z * 0.5f);

            for (var y = 0; y < levelData.height; y++)
            {
                for (var x = 0; x < levelData.width; x++)
                {
                    var newTile = Instantiate(territoryTilePrefab, transform);
                    var territoryTile = newTile.GetComponent<TerritoryTile>();
                    neighbourGrid[x, y] = territoryTile;
                    
                    var position = new Vector3
                    {
                        x = -startingX + scale.x * x,
                        z = startingY - scale.z * y
                    };

                    newTile.transform.position = position;
                    newTile.name = $"TerritoryTile [{position}]";
                    
                    if (levelData.grid[x, y].isNotPopulated)
                    {
                        territoryTile.SetVisibility(false);
                    }
                    
                    FindNeighbours(territoryTile, x, y);
                }
            }
            
            transform.position = new Vector3{x = -(scale.x * levelData.width * 0.5f - scale.x * 0.5f)};
        }

        private void FindNeighbours(TerritoryTile current, int xIndex, int yIndex)
        {
            var canFindLeftCorner = xIndex > 0 && yIndex > 0;
            var canFindRightCorner = xIndex < levelData.width - 1 && yIndex > 0;
            var canFindTop = yIndex > 0;
            var canFindLeft = xIndex > 0;
            
            if (canFindLeftCorner) AddNeighbour(current, neighbourGrid[xIndex - 1, yIndex - 1]);
            if (canFindRightCorner) AddNeighbour(current, neighbourGrid[xIndex + 1, yIndex - 1]);
            if (canFindTop) AddNeighbour(current, neighbourGrid[xIndex, yIndex - 1]);
            if (canFindLeft) AddNeighbour(current, neighbourGrid[xIndex - 1, yIndex]);
        }

        private void AddNeighbour(TerritoryTile current, TerritoryTile neighbour)
        {
            if (!current.neighbours.Contains(neighbour)) current.neighbours.Add(neighbour);
            if (!neighbour.neighbours.Contains(current)) neighbour.neighbours.Add(current);
        }
    }
}
