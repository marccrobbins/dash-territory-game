using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class TerritoryTile : MonoBehaviour
    {
        public Animator animator;
        public Transform container;
        
        [Header("Neighbours")] 
        public List<Neighbour> neighbours = new List<Neighbour>();

        public void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
        
        [Button]
        public void Pulse()
        {
            //Don't activate another pulse if one is still running
            animator.SetTrigger("Pulse");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            if (neighbours == null || neighbours.Count == 0) return;

            foreach (var neighbour in neighbours)
            {
                Gizmos.DrawSphere(neighbour.territory.transform.position, 0.5f);
            }
        }
    }
}
