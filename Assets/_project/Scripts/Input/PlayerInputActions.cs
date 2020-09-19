using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Context = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace DashTerritory
{
    public class PlayerInputActions : MonoBehaviour, GameInput.IMenuActions, GameInput.IGameActions
    {
        public const string LogPrefix = "PLAYERINPUTACTIONS :: ";
        
        public event Action OnStartEvent;

        //Menu events
        public event Action OnSubmitEvent;
        public event Action OnBackEvent;
        public event Action<Vector2> OnSelectionEvent;

        //Game events
        public event Action<Vector2> OnMovementEvent;
        public event Action OnDashEvent;
        public event Action OnJumpEvent;

        [Header("Debug")] 
        [SerializeField] private bool isDebug;

        public void Initialize(PlayerInput input)
        {
            if (!input) return;
            input.actionEvents[0].AddListener(OnSubmit);
            input.actionEvents[1].AddListener(OnBack);
            input.actionEvents[2].AddListener(OnSelection);
            input.actionEvents[3].AddListener(OnStart);
            input.actionEvents[4].AddListener(OnMovement);
            input.actionEvents[5].AddListener(OnStart);
            input.actionEvents[6].AddListener(OnJump);
            input.actionEvents[7].AddListener(OnDash);
        }

        public void OnStart(Context context)
        {
            if (!context.performed) return;
            if (isDebug) Debug.Log($"{LogPrefix}OnStart is pressed.");
            OnStartEvent?.Invoke();
        }

        #region IMenuActions

        public void OnSubmit(Context context)
        {
            if (!context.performed) return;
            if (isDebug) Debug.Log($"{LogPrefix}OnSubmit is pressed.");
            OnSubmitEvent?.Invoke();
        }

        public void OnBack(Context context)
        {
            if (!context.performed) return;
            if (isDebug) Debug.Log($"{LogPrefix}OnBack is pressed.");
            OnBackEvent?.Invoke();
        }

        public void OnSelection(Context context)
        {
            if (isDebug) Debug.Log($"{LogPrefix}OnSelection.");
            OnSelectionEvent?.Invoke(context.ReadValue<Vector2>());
        }
        
        #endregion IMenuActions

        #region IGameActions

        public void OnMovement(Context context)
        {
            if (isDebug) Debug.Log($"{LogPrefix}OnMovement.");
            OnMovementEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(Context context)
        {
            if (!context.performed) return;
            if (isDebug) Debug.Log($"{LogPrefix}OnJump is pressed.");
            OnJumpEvent?.Invoke();
        }

        public void OnDash(Context context)
        {
            if (!context.performed) return;
            if (isDebug) Debug.Log($"{LogPrefix}OnDash is pressed.");
            OnDashEvent?.Invoke();
        }
        
        #endregion IGameActions
    }
}
