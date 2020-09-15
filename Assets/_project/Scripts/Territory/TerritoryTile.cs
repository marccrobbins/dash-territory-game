using System.Collections.Generic;
using UnityEngine;

namespace DashTerritory
{
    public class TerritoryTile : MonoBehaviour
    {
        public List<TerritoryTile> neighbours = new List<TerritoryTile>();

        public void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void OnDrawGizmosSelected()
        {
            if (neighbours.Count == 0) return;

            Gizmos.color = Color.cyan;
            
            foreach (var neighbour in neighbours)
            {
                if (!neighbour) continue;
                Gizmos.DrawSphere(neighbour.transform.position, 0.5f);
            }
        }
    }
}
