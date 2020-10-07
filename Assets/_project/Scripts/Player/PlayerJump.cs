using System.Collections;
using Framework.Pooling;
using Framework.Pooling.Generated;
using UnityEngine;

namespace DashTerritory
{
   public class PlayerJump : MonoBehaviour
   {
      public float jumpForce = 2;
      public float groundSmashForce = 2;
      public float groundSmashCheckDistance = 5;

      private Player player;
      private bool isDoingGroundSmash;
      private bool canGroundSmash;
      private Coroutine groundSmashRoutine;
      
      public void Initialize(Player player)
      {
         this.player = player;

         var inputActions = player.InputActions;
         inputActions.OnJumpEvent += JumpButton;
      }
      
      public void Uninitialize()
      {
         var inputActions = player.InputActions;
         inputActions.OnJumpEvent -= JumpButton;
      }

      private void Update()
      {
         canGroundSmash = !player.CheckDistance(groundSmashCheckDistance);
      }
      
      private void JumpButton()
      {
         if (isDoingGroundSmash) return;
            
         if (player.IsGrounded)
         {
            Jump(jumpForce);
            return;
         }

         if (!canGroundSmash) return;
            
         if (groundSmashRoutine != null) StopCoroutine(groundSmashRoutine);
         groundSmashRoutine = StartCoroutine(GroundSmashRoutine());
      }

      private void Jump(float force)
      {
         Debug.Log("jump");
         player.rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
      }

      private IEnumerator GroundSmashRoutine()
      {
         Debug.Log("ground smash");
         isDoingGroundSmash = true;
            
         player.rigidbody.AddForce(Vector3.down * groundSmashForce, ForceMode.Impulse);
            
         yield return new WaitUntil(() => player.IsGrounded);

         PoolManager.Instance.Spawn(PoolNames.BODYSLAMEFFECT, transform.position);
         //This does a little jump up when you land but that can probably just be an animation
         //Jump(jumpForce * 0.4f);
            
         isDoingGroundSmash = false;
      }
   }
}
