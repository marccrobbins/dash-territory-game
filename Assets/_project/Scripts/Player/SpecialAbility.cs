using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashTerritory
{
    public class SpecialAbility : MonoBehaviour
    {
        private Player player;

        public void Initialize(Player player)
        {
            this.player = player;

            var inputActions = player.InputActions;
        }

        public void Uninitialize()
        {
            
        }

        private void AbilityButton()
        {
            
        }
    }
}
