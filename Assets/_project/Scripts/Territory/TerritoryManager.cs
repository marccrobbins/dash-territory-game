using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DashTerritory
{
    public class TerritoryManager : Manager<TerritoryManager>
    {
        //ToDo this will need to be different, not sure if there will be "level" loading or not
        public LevelData levelData;
        public GameObject territoryTilePrefab;
        public TerritoryTile[,] grid;

        public float pulseDelay = 0.1f;

        #region Building

        public void BuildTerritory(Transform parent)
        {
            grid = new TerritoryTile[levelData.width, levelData.height];
            var scale = territoryTilePrefab.transform.localScale;
            var startingX = scale.x * levelData.width * 0.5f - scale.x * 0.5f;
            var startingY = scale.z * levelData.height * 0.5f - scale.z * 0.5f;

            for (var y = 0; y < levelData.height; y++)
            {
                for (var x = 0; x < levelData.width; x++)
                {
                    var newTile = Instantiate(territoryTilePrefab, parent);
                    var territoryTile = newTile.GetComponent<TerritoryTile>();
                    grid[x, y] = territoryTile;
                    
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
        }

        private void FindNeighbours(TerritoryTile current, int xIndex, int yIndex)
        {
            var canFindLeftCorner = xIndex > 0 && yIndex > 0;
            var canFindRightCorner = xIndex < levelData.width - 1 && yIndex > 0;
            var canFindTop = yIndex > 0;
            var canFindLeft = xIndex > 0;
            
            if (canFindLeftCorner) AddNeighbour(current, grid[xIndex - 1, yIndex - 1], GridLocation.NorthWest);
            if (canFindRightCorner) AddNeighbour(current, grid[xIndex + 1, yIndex - 1], GridLocation.NorthEast);
            if (canFindTop) AddNeighbour(current, grid[xIndex, yIndex - 1], GridLocation.North);
            if (canFindLeft) AddNeighbour(current, grid[xIndex - 1, yIndex], GridLocation.West);
        }

        private void AddNeighbour(TerritoryTile current, TerritoryTile neighbour, GridLocation location)
        {
            current.neighbours.Add(new Neighbour
            {
                territory = neighbour,
                location = location
            });
            
            switch (location)
            {
                case GridLocation.NorthWest:
                    neighbour.neighbours.Add(new Neighbour
                    {
                        territory = current,
                        location = GridLocation.SouthEast
                    });
                    break;
                case GridLocation.NorthEast:
                    neighbour.neighbours.Add(new Neighbour
                    {
                        territory = current,
                        location = GridLocation.SouthWest
                    });
                    break;
                case GridLocation.North:
                    neighbour.neighbours.Add(new Neighbour
                    {
                        territory = current,
                        location = GridLocation.South
                    });
                    break;
                case GridLocation.West:
                    neighbour.neighbours.Add(new Neighbour
                    {
                        territory = current,
                        location = GridLocation.East
                    });
                    break;
            }
        }

        #endregion Building

        #region PulseTerritory

        /// <summary>
        /// Create a wave pulse from a random territory tile
        /// </summary>
        [Button]
        public void CenterPulse()
        {
            var sourceTerritory = grid.GetRandomValue();

            StartCoroutine(CenterPulseRoutine(sourceTerritory));
        }

        private IEnumerator CenterPulseRoutine(TerritoryTile source)
        {
            var collection = new Dictionary<int, List<TerritoryTile>>();
            var found = new List<TerritoryTile>{source};
            var last = new List<TerritoryTile>{source};
            
            do
            {
                if (!collection.ContainsKey(collection.Count)) collection.Add(collection.Count, new List<TerritoryTile>());

                var currentIndex = collection.Count - 1;
                
                foreach (var lastTerritory in last)
                {
                    var results = new List<TerritoryTile>();
                    foreach (var neighbour in lastTerritory.neighbours)
                    {
                        if (neighbour == null) continue;
                        var territory = neighbour.territory;
                        if (!territory) continue;
                        if (found.Contains(territory)) continue;
                        results.Add(territory);
                    }

                    found.AddRange(results);
                    collection[currentIndex].AddRange(results);
                }

                last = collection[currentIndex];
            } while (found.Count != grid.Length);

            StartCoroutine(PulseRoutine(collection));
            yield break;
        }

        [Button]
        public void PulseFromLocation(GridLocation location)
        {
            var collection = new Dictionary<int, List<TerritoryTile>>();
            switch (location)
            {
                case GridLocation.North:
                    collection = grid.GetCollectionAlongDimension(1, 0);
                    break;
                case GridLocation.East:
                    collection = grid.GetCollectionAlongDimension(0, 1).Reverse();
                    break;
                case GridLocation.South:
                    collection = grid.GetCollectionAlongDimension(1, 0).Reverse();
                    break;
                case GridLocation.West:
                    collection = grid.GetCollectionAlongDimension(0, 1);
                    break;
                case GridLocation.NorthEast:
                    StartCoroutine(CenterPulseRoutine(grid[grid.GetLength(0) - 1, 0]));
                    break;
                case GridLocation.SouthEast:
                    StartCoroutine(CenterPulseRoutine(grid.Last()));
                    break;
                case GridLocation.NorthWest:
                    StartCoroutine(CenterPulseRoutine(grid.First()));
                    break;
                case GridLocation.SouthWest:
                    StartCoroutine(CenterPulseRoutine(grid[0, grid.GetLength(1) - 1]));
                    break;
            }

            StartCoroutine(PulseRoutine(collection));
        }

        private IEnumerator PulseRoutine(Dictionary<int, List<TerritoryTile>> collection)
        {
            foreach (var elementLists in collection)
            {
                foreach (var element in elementLists.Value)
                {
                    element.Pulse();
                }
                
                yield return new WaitForSeconds(pulseDelay);
            }
        }

        #endregion PulseTerritory
    }
    
    [Serializable]
    public class Neighbours
    {
        public TerritoryTile northNeighbour;
        public TerritoryTile eastNeighbour;
        public TerritoryTile southNeighbour;
        public TerritoryTile westNeighbour;
        public TerritoryTile northEastNeighbour;
        public TerritoryTile southEastNeighbour;
        public TerritoryTile northWestNeighbour;
        public TerritoryTile southWestNeighbour;
        
        public List<TerritoryTile> AllNeighbours => new List<TerritoryTile>
        {
            northNeighbour, 
            eastNeighbour, 
            southNeighbour, 
            westNeighbour, 
            northEastNeighbour, 
            southEastNeighbour, 
            northWestNeighbour, 
            southWestNeighbour
        };

        public TerritoryTile GetNeighbourByType(GridLocation location)
        {
            switch (location)
            {
                case GridLocation.North:
                    return northNeighbour;
                case GridLocation.East:
                    return eastNeighbour;
                case GridLocation.South:
                    return southNeighbour;
                case GridLocation.West:
                    return westNeighbour;
                case GridLocation.NorthEast:
                    return northEastNeighbour;
                case GridLocation.SouthEast:
                    return southEastNeighbour;
                case GridLocation.NorthWest:
                    return northEastNeighbour;
                case GridLocation.SouthWest:
                    return southWestNeighbour;
            }

            return default;
        }
    }

    [Serializable]
    public class Neighbour
    {
        public TerritoryTile territory;
        public GridLocation location;
    }

    public enum GridLocation
    {
        North, 
        East, 
        South,
        West,
        NorthEast,
        SouthEast,
        NorthWest,
        SouthWest
    }
}
