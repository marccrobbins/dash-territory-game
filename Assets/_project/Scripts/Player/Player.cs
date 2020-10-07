using Framework;
using Framework.Pooling;
using Framework.Pooling.Generated;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class Player : MonoBehaviour
    {
        public Collider collider;
        public Rigidbody rigidbody;
        
        public LayerMask layerCheck;
        public float groundedCheckDistance = 0.1f;
        
        public PlayerMovement playerMovement;
        public PlayerDash playerDash;
        public PlayerJump playerJump;
        public SpecialAbility playerAbility;
        
        public PlayerInputActions InputActions { get; private set; }
        public bool IsGrounded { get; private set; }
        public Color Representation { get; private set; }
        
        public void Initialize(PlayerInputActions playerInputActions)
        {
            InputActions = playerInputActions;
            
            playerMovement.Initialize(this);
            playerDash.Initialize(this);
            playerJump.Initialize(this);
            playerAbility.Initialize(this);

            Representation = Representation.RandomColor();
        }

        private void Update()
        {
            var transformPosition = transform.position;
            var halfHeight = collider.bounds.size.y * 0.5f;
            var checkStart = transformPosition;
            checkStart.y -= halfHeight - 0.1f * 0.5f;
            
            IsGrounded = CheckDistance(groundedCheckDistance);
        }

        [Button]
        private void Death()
        {
            PoolManager.Instance.Spawn(PoolNames.DEATHEFFECT, transform.position);
            PlayerManager.Instance.PlayerDied(this);
        }

        #region Utility

        public bool CheckDistance(float distance)
        {
            var transformPosition = transform.position;
            var halfHeight = collider.bounds.size.y * 0.5f;
            var checkStart = transformPosition;
            checkStart.y -= halfHeight - groundedCheckDistance * 0.5f;
            
            var ray = new Ray(checkStart, -transform.up);
            return Physics.Raycast(ray, distance, layerCheck);
        }
        
        #endregion Utility

        private void OnDestroy()
        {
            playerMovement.Uninitialize();
            playerDash.Uninitialize();
            playerJump.Uninitialize();
            playerAbility.Uninitialize();
        }
    }
}
