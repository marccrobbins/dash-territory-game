using UnityEngine;

namespace DashTerritory
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 5;
        
        private Player player;
        private Vector3 axis;
        private Vector3 heading;
        
        public void Initialize(Player player)
        {
            this.player = player;
            
            var inputActions = player.InputActions;
            inputActions.OnMovementEvent += Move;
        }

        public void Uninitialize()
        {
            var inputActions = player.InputActions;
            inputActions.OnMovementEvent -= Move;
        }

        private void FixedUpdate()
        {
            if (player.playerDash.IsDashing) return;
            
            heading = axis;

            //Apply speed
            heading *= moveSpeed;
            
            //Rotate player
            if (heading != Vector3.zero)
            {
                var lookRotation = Quaternion.LookRotation(heading, Vector3.up);
                transform.rotation = lookRotation;
            }
            
            //Move
            heading *= Time.deltaTime;
            
            player.rigidbody.position += heading;
        }

        private void Move(Vector2 value)
        {
            axis = new Vector3(value.x, 0, value.y);
        }
    }
}
