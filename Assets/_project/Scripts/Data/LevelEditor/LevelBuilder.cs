using UnityEngine;

namespace DashTerritory
{
    public class LevelBuilder : MonoBehaviour
    {
        private void Start()
        {
            TerritoryManager.Instance.BuildTerritory(transform);
        }
    }
}
