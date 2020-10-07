using System.Collections;
using Framework.Pooling;
using Framework.Pooling.Generated;
using UnityEngine;

namespace DashTerritory
{
    public class PlayerDash : MonoBehaviour
    {
        public float dashDuration = 0.2f;
        public float dashDistance = 5;
        public float dashCooldown = 1;

        [Header("UI")]
        public GameObject cooldownBar;
        public Transform scaler;
        
        private Player player;
        private bool isDashCooldown;
        private Transform cameraTransform;
        
        public bool IsDashing { get; private set; }
        
        public void Initialize(Player player)
        {
            this.player = player;

            var inputActions = player.InputActions;
            inputActions.OnDashEvent += DashButton;
            
            cooldownBar.SetActive(false);
            
            var cam = Camera.main;
            if (cam == null) return;

            cameraTransform = cam.transform;
        }
      
        public void Uninitialize()
        {
            var inputActions = player.InputActions;
            inputActions.OnDashEvent -= DashButton;
        }

        private void Update()
        {
            if (!cameraTransform) return;
            if (!cooldownBar.activeInHierarchy) return;
            
            //Always look at camera on the Y only
            var containerTransform = cooldownBar.transform;
            var relative = cameraTransform.position;
            relative.y = containerTransform.position.y;
            
            containerTransform.rotation = Quaternion.LookRotation(relative, Vector3.up);
        }

        private void DashButton()
        {
            if (IsDashing ||
                isDashCooldown) return;
            
            PoolManager.Instance.Spawn(PoolNames.DASHEFFECT, transform.position, transform.rotation, Vector3.one * 2, transform);

            StartCoroutine(DashRoutine());
        }

        //Todo Calculate dash target position, check if going to hit a wall and dash to just before it
        //Todo only check against walls and geo, ignore players during check...https://www.youtube.com/watch?v=-_QIF1-x6XI&feature=emb_title...reference
        private IEnumerator DashRoutine()
        {
            IsDashing = true;
            
            var dashTarget = transform.forward * dashDistance;
            var timePassed = 0f;
            var fixedUpdateInstruction = new WaitForFixedUpdate();
            while (timePassed < dashDuration)
            {
                timePassed += Time.deltaTime;

                var startPosition = transform.position;
                var result = startPosition + timePassed / dashDuration * dashTarget;
                result.y = startPosition.y;
                transform.position = result;
                
                yield return fixedUpdateInstruction;
            }

            IsDashing = false;
            
            //Do cooldown
            isDashCooldown = true;
            cooldownBar.SetActive(true);
            
            timePassed = 0f;
            while (timePassed < dashCooldown)
            {
                timePassed += Time.deltaTime;

                var result = Mathf.Lerp(0, 1, timePassed / dashCooldown);
                var scale = Vector3.one;
                scale.x = result;
                scaler.localScale = scale;
                
                yield return fixedUpdateInstruction;
            }
            
            isDashCooldown = false;
            cooldownBar.SetActive(false);
        }
    }
}
