using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public static class ArrayExtensions
    {
        public static T GetRandomValue<T>(this T[,] array)
        {
            return array[Random.Range(0, array.GetLength(0)), Random.Range(0, array.GetLength(1))];
        }

        public static List<T> GetElementsAtIndex<T>(this T[,] array, int dimension, int index)
        {
            var results = new List<T>();
            for (var i = 0; i < array.GetLength(dimension); i++)
            {
                results.Add(dimension == 0 ? array[i, index] : array[index, i]);
            }

            return results;
        }

        public static Dictionary<int, List<T>> GetCollectionAlongDimension<T>(this T[,] array, int dimension, int staticDimension, bool isReversed = false)
        {
            var result = new Dictionary<int, List<T>>();
            var length = array.GetLength(dimension);

            for (var i = 0; i < length; i++)
            {
                result[i] = array.GetElementsAtIndex(staticDimension, i);
            }

            return result;
        }

        public static T First<T>(this T[,] array)
        {
            return array[0, 0];
        }
        
        public static T Last<T>(this T[,] array)
        {
            return array[array.GetLength(0) - 1, array.GetLength(1) - 1];
        }
    }
}
