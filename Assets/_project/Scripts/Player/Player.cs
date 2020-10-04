using System;
using Framework;
using Framework.Pooling;
using Framework.Pooling.Generated;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class Player : MonoBehaviour
    {
        public Movement playerMovement;
        
        private PlayerInputActions inputActions;

        public Color Representation { get; private set; }
        
        public void Initialize(PlayerInputActions playerInputActions)
        {
            inputActions = playerInputActions;
            
            playerMovement.Initialize(inputActions);

            Representation = Representation.RandomColor();
        }

        [Button]
        private void Death()
        {
            PoolManager.Instance.Spawn(PoolNames.DEATHEFFECT, transform.position);
            PlayerManager.Instance.PlayerDied(this);
        }

        private void OnDestroy()
        {
            playerMovement.Uninitialize(inputActions);
        }
    }
}
