using System.Collections;
using Framework.Pooling;
using Framework.Pooling.Generated;
using UnityEngine;

namespace DashTerritory
{
    public class PlayerDash : MonoBehaviour
    {
        public float dashForce = 2;
        public float dashCooldown = 1;

        [Header("UI")]
        public GameObject cooldownBar;
        public Transform scaler;
        
        private Player player;
        private bool isDashCooldown;
        private Transform cameraTransform;
        private Coroutine fillRoutine;
        
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
            if (isDashCooldown) return;
            
            PoolManager.Instance.Spawn(PoolNames.DASHEFFECT, transform.position, transform.rotation, Vector3.one * 2, transform);
            player.rigidbody.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            
            if (fillRoutine != null) StopCoroutine(fillRoutine);
            fillRoutine = StartCoroutine(FillRoutine());
        }
        
        private IEnumerator FillRoutine()
        {
            isDashCooldown = true;
            cooldownBar.SetActive(true);

            var timePassed = 0f;
            var fixedUpdateInstruction = new WaitForFixedUpdate();
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
