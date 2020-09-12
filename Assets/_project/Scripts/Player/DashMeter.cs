using System;
using System.Collections;
using UnityEngine;

namespace DashTerritory
{
    public class DashMeter : MonoBehaviour
    {
        public GameObject container;
        public Transform scaler;

        private Transform cameraTransform;
        private Coroutine fillRoutine;

        private void Start()
        {
            container.SetActive(false);
            
            var cam = Camera.main;
            if (cam == null) return;

            cameraTransform = cam.transform;
        }

        private void Update()
        {
            if (!cameraTransform) return;
            if (!container.activeInHierarchy) return;
            
            //Always look at camera on the Y only
            var containerTransform = container.transform;
            var relative = cameraTransform.position;
            relative.y = containerTransform.position.y;
            
            containerTransform.rotation = Quaternion.LookRotation(relative, Vector3.up);
        }

        public void Fill(float duration, Action onCompleteCallback = null)
        {
            if (fillRoutine != null) StopCoroutine(fillRoutine);
            fillRoutine = StartCoroutine(FillRoutine(duration, onCompleteCallback));
        }

        private IEnumerator FillRoutine(float duration, Action onCompleteCallback = null)
        {
            container.SetActive(true);

            var timePassed = 0f;
            var fixedUpdateInstruction = new WaitForFixedUpdate();
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;

                var result = Mathf.Lerp(0, 1, timePassed / duration);
                var scale = Vector3.one;
                scale.x = result;
                scaler.localScale = scale;
                
                yield return fixedUpdateInstruction;
            }
            
            onCompleteCallback?.Invoke();
            
            container.SetActive(false);
        }
    }
}
