using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6;
    public float jumpSpeed = 8;
    public float gravity = 20;
    
    private CharacterController _controller;
    private Vector3 _targetHeading;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    { 
        if (_controller.isGrounded)
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            
            _targetHeading = new Vector3(h, 0, v);
            _targetHeading *= moveSpeed;

            if (Input.GetButtonUp("Fire1"))
            {
                _targetHeading.y = jumpSpeed;
            }
        }
        
        //Apply rotation
        if (_targetHeading != Vector3.zero)
        {
            var turn = Quaternion.LookRotation(_targetHeading);
            transform.rotation = turn;
        }

        //Apply gravity
        _targetHeading.y -= gravity * Time.deltaTime;
        
        //Move character
        _controller.Move(_targetHeading * Time.deltaTime);
    }
}
