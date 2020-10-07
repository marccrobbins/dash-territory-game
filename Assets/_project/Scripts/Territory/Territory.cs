using System.Collections.Generic;
using UnityEngine;

namespace DashTerritory
{
    public class Territory : MonoBehaviour
    {
        public Renderer blockRenderer;
        
        [Header("Neighbours")] 
        public List<Neighbour> neighbours = new List<Neighbour>();
        
        public Player Owner { get; private set; }
        public bool IsOccupied { get; private set; }

        public void Pulse()
        {
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var player = other.GetComponentInParent<Player>();
            if (!player) return;

            if (IsOccupied||
                !player.IsGrounded ||
                player.playerDash.IsDashing) return; 
            
            IsOccupied = true;
            
            if (Owner == player) return;

            Owner = player;
            UpdateOwnership(player.Representation);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var player = other.GetComponentInParent<Player>();
            if (!player) return;

            IsOccupied = false;
        }

        public void UpdateOwnership(Color color)
        {
            blockRenderer.materials[0].SetColor("_Color", color);
            
            //Update the border visuals here
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
