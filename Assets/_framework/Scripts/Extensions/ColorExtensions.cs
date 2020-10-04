using UnityEngine;

namespace Framework
{
    public static class ColorExtensions
    {
        public static Color RandomColor(this Color color, bool hasAlpha = false)
        {
            return new Color(Random.value, Random.value, Random.value, hasAlpha ? Random.value : 0);
        }
        
        public static Color32 RandomColor32(this Color32 color, bool hasAlpha = false)
        {
            return new Color(
                Random.Range(0, 255), 
                Random.Range(0, 255),
                Random.Range(0, 255), 
                hasAlpha ? Random.Range(0, 255) : 0);
        }
    }
}
