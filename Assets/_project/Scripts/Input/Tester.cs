using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashTerritory
{
    public class Tester : MonoBehaviour
    {
        public Vector2 movement;
        public Vector2 selection;
        public bool isMenuOpen;
        
        #region MonoBehaviour

        private void Start()
        {
            InputManager.OnStartPressed += StartPressed;
            InputManager.OnJumpPressed += JumpPressed;
            InputManager.OnDashPressed += DashPressed;
            InputManager.OnSubmitPressed += SubmitPressed;
            InputManager.OnSelection += Selection;
            InputManager.OnMovement += Movement;
        }

        private void OnDestroy()
        {
            InputManager.OnStartPressed -= StartPressed;
            InputManager.OnJumpPressed -= JumpPressed;
            InputManager.OnDashPressed -= DashPressed;
            InputManager.OnSubmitPressed -= SubmitPressed;
            InputManager.OnSelection -= Selection;
            InputManager.OnMovement -= Movement;
        }
        
        #endregion MonoBehaviour

        private void Movement(Vector2 value)
        {
            movement = value;
        }

        private void Selection(Vector2 value)
        {
            selection = value;
        }

        private void SubmitPressed()
        {
            Debug.Log("Submit pressed");
        }

        private void DashPressed()
        {
            Debug.Log("Dash pressed");
        }

        private void JumpPressed()
        {
            Debug.Log("Jump pressed");
        }

        private void StartPressed()
        {
            isMenuOpen = !isMenuOpen;
            Debug.Log($"Start pressed, setting to {isMenuOpen}");
            
            InputManager.SetInputMap(isMenuOpen ? InputMapType.Menu : InputMapType.Game);
        }
    }
}
