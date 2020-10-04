using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class TerritoryManager : Manager<TerritoryManager>
    {
        private const string LogPrefix = "TERRITORYMANAGER :: ";
        
        //ToDo this will need to be different, not sure if there will be "level" loading or not
        public LevelData levelData;
        public Territory[,] grid;

        public Dictionary<Player, List<Territory>> territoryOwnershipLookup;

        public float pulseDelay = 0.1f;

        protected override IEnumerator WaitForDependencies()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);
            
            territoryOwnershipLookup = new Dictionary<Player, List<Territory>>();
        }

        #region Building

        public void BuildTerritory(Transform parent = null)
        {
            grid = new Territory[levelData.width, levelData.height];
            var size = levelData.tileSize;
            var startingX = size * levelData.width * 0.5f - size * 0.5f;
            var startingY = size * levelData.height * 0.5f - size * 0.5f;

            for (var y = 0; y < levelData.height; y++)
            {
                for (var x = 0; x < levelData.width; x++)
                {
                    var data = levelData.grid[x, y];
                    if (!data.tile || data.isNotPopulated) continue;
                    var tileObject = Instantiate(data.tile.prefab, parent);
                    tileObject.transform.localScale = Vector3.one * size;
                    
                    var territory = tileObject.GetComponent<Territory>();
                    if (territory)
                    {
                        grid[x, y] = territory;
                        FindNeighbours(territory, x, y);
                    }

                    if (data.modifier)
                    {
                        Instantiate(data.modifier.prefab, tileObject.transform);
                    }

                    var position = new Vector3
                    {
                        x = -startingX + size * x,
                        z = startingY - size * y
                    };

                    tileObject.transform.position = position;
                    tileObject.name = $"TerritoryTile [{position}]";
                }
            }
            
            Debug.Log($"{LogPrefix}Territory built");
        }

        private void FindNeighbours(Territory current, int xIndex, int yIndex)
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

        private void AddNeighbour(Territory current, Territory neighbour, GridLocation location)
        {
            if (!neighbour) return;
            
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

        private IEnumerator CenterPulseRoutine(Territory source)
        {
            var collection = new Dictionary<int, List<Territory>>();
            var found = new List<Territory>{source};
            var last = new List<Territory>{source};
            
            do
            {
                if (!collection.ContainsKey(collection.Count)) collection.Add(collection.Count, new List<Territory>());

                var currentIndex = collection.Count - 1;
                
                foreach (var lastTerritory in last)
                {
                    var results = new List<Territory>();
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
            var collection = new Dictionary<int, List<Territory>>();
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

        private IEnumerator PulseRoutine(Dictionary<int, List<Territory>> collection)
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
        public Territory northNeighbour;
        public Territory eastNeighbour;
        public Territory southNeighbour;
        public Territory westNeighbour;
        public Territory northEastNeighbour;
        public Territory southEastNeighbour;
        public Territory northWestNeighbour;
        public Territory southWestNeighbour;
        
        public List<Territory> AllNeighbours => new List<Territory>
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

        public Territory GetNeighbourByType(GridLocation location)
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
        public Territory territory;
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
