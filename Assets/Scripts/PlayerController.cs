using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float inputBuffer = 0.1f;
    public float moveSpeed;
    public float turnSpeed;
    public float gravity = 9.18f;
    [Tooltip("Is grounded check starts from the bottom of the CharacterController volume.")]
    public float groundedCheckDistance = 0.1f;
    public bool isGrounded;
    
    private CharacterController _controller;
    private Vector3 _targetHeading;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CalculateGrounded();
        if (!isGrounded)
        {
            var position = transform.position;
            position.y -= gravity * Time.deltaTime;
            transform.position = position;
        }

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        _targetHeading = new Vector3(h, 0, v);
        if (_targetHeading == Vector3.zero) return;
        var heading = Quaternion.LookRotation(_targetHeading);
        transform.rotation = heading;
        _controller.Move(_targetHeading * moveSpeed);
    }

    private void CalculateGrounded()
    {
        var playerTransform = transform;
        var playerPosition = playerTransform.position;
        var height = _controller.height <= 1 ? 1 : _controller.height;
        var rayPosition = new Vector3(playerPosition.x, playerPosition.y - height * 0.5f, playerPosition.z);
        var ray = new Ray(rayPosition, -playerTransform.up);
        
        isGrounded = Physics.Raycast(ray, groundedCheckDistance);
    }
}
