using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace DashTerritory
{
    public class CameraManager : Manager<CameraManager>
    {
        public Camera camera;
        public Transform CameraTransform => camera.transform;

        public Transform[] players;

        public float scale;
        public int levelDepth;

        public float height;
        private float min;
        
        private void Start()
        {
            SetCameraSettings(levelDepth);
        }

        public void SetCameraSettings(int size)
        {
            height = size * 0.5f;
            min = levelDepth * scale * 0.5f;
        }

        private void Update()
        {
            CameraTransform.position = new Vector3
            {
                y = height,
                z = -min
            };
            
            CameraTransform.LookAt(Vector3.zero);
        }
    }
}
