using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public static class DictionaryExtensions
    {
        public static Dictionary<T, U> Reverse<T, U>(this Dictionary<T, U> dictionary)
        {
            var keys = dictionary.Keys;
            var reversedKeys = keys.Reverse();

            return reversedKeys.ToDictionary(key => key, key => dictionary[key]);
        }
    }
}
