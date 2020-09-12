using UnityEngine;

namespace DashTerritory
{
    [RequireComponent(typeof(Rigidbody))]
    public class Gravity : MonoBehaviour
    {
        public float gravityScale = 1;

        private Rigidbody rigidbody;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
        }

        private void FixedUpdate()
        {
            var gravity = Physics.gravity.y * gravityScale * Vector3.up;
            rigidbody.AddForce(gravity, ForceMode.Acceleration);
        }
    }
}
