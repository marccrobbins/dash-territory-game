using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Context = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace DashTerritory
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        
        public static event Action OnStartPressed;
        
        //Menu events
        public static event Action OnSubmitPressed;
        public static event Action OnBackPressed;
        public static event Action<Vector2> OnSelection; 
        
        //Game events
        public static event Action<Vector2> OnMovement;
        public static event Action OnDashPressed;
        public static event Action OnJumpPressed; 
        
        [SerializeField] private PlayerInput playerInput;

        #region MonoBehaviour

        private void Start()
        {
            instance = this;
        }

        #endregion MonoBehaviour

        public static void SetInputMap(InputMapType type)
        {
            instance.playerInput.SwitchCurrentActionMap(type.ToString());
        }

        public void StartButton(Context context)
        {
            if (!context.performed) return;
            
            OnStartPressed?.Invoke();
        }
        
        #region Menu

        public void SubmitButton(Context context)
        {
            if (!context.performed) return;
            
            OnSubmitPressed?.Invoke();
        }

        public void BackButton(Context context)
        {
            if (!context.performed) return;
            
            OnBackPressed?.Invoke();
        }

        public void Selection(Context context)
        {
            OnSelection?.Invoke(context.ReadValue<Vector2>());
        }

        #endregion Menu

        #region Game

        public void Movement(Context context)
        {
            OnMovement?.Invoke(context.ReadValue<Vector2>());
        }

        public void JumpButton(Context context)
        {
            if (!context.performed) return;
            
            OnJumpPressed?.Invoke();
        }

        public void DashButton(Context context)
        {
            if (!context.performed) return;
            
            OnDashPressed?.Invoke();
        }

        #endregion Game
    }
    
    public enum InputMapType
    {
        Menu,
        Game
    }
}
