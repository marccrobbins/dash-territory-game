using System.Collections;
using Framework.Pooling;
using Framework.Pooling.Generated;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DashTerritory
{
    public class Movement : MonoBehaviour
    {
        [Header("References")] 
        public PlayerInput playerInput;
        public Collider collider;
        public Rigidbody rigidbody;
        public DashMeter dashMeter;

        [Header("Stats")]
        public LayerMask layerCheck;
        public float moveSpeed = 5;
        public float dashCooldown = 2;
        
        [Header("Force")]
        public float dashForce = 5;
        public float jumpForce = 2;
        public float groundSmashForce = 2;
        
        [Header("CheckDistances")]
        public float groundedCheckDistance = 0.1f;
        public float groundSmashCheckDistance = 5;

        private Vector3 axis;
        private Vector3 heading;
        private bool isDoingGroundSmash;
        private bool isGrounded;
        private bool canGroundSmash;
        private bool isDashCooldown;
        private Coroutine groundSmashRoutine;
        
        private void Start()
        {
            InputManager.RegisterMovementAxis(playerInput, Move);
            InputManager.RegisterDashButton(playerInput, DashButton);
            InputManager.RegisterJumpButton(playerInput, JumpButton);
        }

        private void FixedUpdate()
        {
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
            
            rigidbody.position += heading;
        }

        private void Update()
        {
            var transformPosition = transform.position;
            var halfHeight = collider.bounds.size.y * 0.5f;
            var checkStart = transformPosition;
            checkStart.y -= halfHeight - groundedCheckDistance * 0.5f;
            
            Debug.DrawRay(checkStart, -transform.up * groundedCheckDistance, Color.red);
            Debug.DrawRay(checkStart, -transform.up * groundSmashCheckDistance, Color.blue);

            isGrounded = CheckDistance(checkStart, groundedCheckDistance);
            canGroundSmash = !CheckDistance(checkStart, groundSmashCheckDistance);
        }

        private bool CheckDistance(Vector3 start, float distance)
        {
            var ray = new Ray(start, -transform.up);
            return Physics.Raycast(ray, distance, layerCheck);
        }

        #region InputManagerCalls

        private void Move(Vector2 value)
        {
            axis = new Vector3(value.x, 0, value.y);
        }

        private void DashButton()
        {
            if (isDashCooldown) return;
            
            PoolManager.Instance.Spawn(PoolNames.DASHEFFECT, transform.position, transform.rotation, Vector3.one * 2, transform);
            rigidbody.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            
            isDashCooldown = true;
            dashMeter.Fill(dashCooldown, () => isDashCooldown = false);
        }

        private void JumpButton()
        {
            if (isDoingGroundSmash) return;
            
            if (isGrounded)
            {
                Jump(jumpForce);
                return;
            }

            if (!canGroundSmash) return;
            
            if (groundSmashRoutine != null) StopCoroutine(groundSmashRoutine);
            groundSmashRoutine = StartCoroutine(GroundSmashRoutine());
        }
        
        #endregion InputManagerCalls

        private void Jump(float force)
        {
            Debug.Log("jump");
            rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
        }

        private IEnumerator GroundSmashRoutine()
        {
            Debug.Log("ground smash");
            isDoingGroundSmash = true;
            
            rigidbody.AddForce(Vector3.down * groundSmashForce, ForceMode.Impulse);
            
            yield return new WaitUntil(() => isGrounded);

            PoolManager.Instance.Spawn(PoolNames.BODYSLAMEFFECT, transform.position);
            Jump(jumpForce * 0.4f);
            
            isDoingGroundSmash = false;
        }
        
        private void OnDestroy()
        {
            InputManager.UnRegisterMovementAxis(playerInput, Move);
            InputManager.UnRegisterDashButton(playerInput, DashButton);
            InputManager.UnRegisterJumpButton(playerInput, JumpButton);
        }
    }
}
