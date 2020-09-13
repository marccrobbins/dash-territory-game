using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using Context = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace DashTerritory
{
    public class InputManager : Manager<InputManager>
    {
        public const string LogPrefix = "INPUTMANAGER ::";
        
        private Dictionary<PlayerInput, PlayerInputActions> playerInputActionsLookup;

        public override void Initialize()
        {
            playerInputActionsLookup = new Dictionary<PlayerInput, PlayerInputActions>();
            base.Initialize();
        }
        
        /// <summary>
        /// Changes the map being used
        /// </summary>
        /// <param name="type"></param>
        public static void SetInputMap(InputMapType type)
        {
            //instance.playerInput.SwitchCurrentActionMap(type.ToString());
        }

        #region Registration

        /// <summary>
        /// Creates a <see cref="PlayerInputActions"/> object and adds listeners to playerInput.
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        public void RegisterPlayer(PlayerInput playerInput)
        {
            Debug.Log($"{LogPrefix}Registering player with id {playerInput.user.id}");
            if (playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}player with {playerInput.user.id} has already been registered.");
                return;
            }
            
            var playerInputActions = new PlayerInputActions();
            
            //Add listeners for calls. NOTE: This is in order that they appear in the editor
            //Submit
            playerInput.actionEvents[0].AddListener(playerInputActions.SubmitButton);
            //Back
            playerInput.actionEvents[1].AddListener(playerInputActions.BackButton);
            //Selection
            playerInput.actionEvents[2].AddListener(playerInputActions.Selection);
            //Start - Menu
            playerInput.actionEvents[3].AddListener(playerInputActions.StartButton);
            //Movement
            playerInput.actionEvents[4].AddListener(playerInputActions.Movement);
            //Start - Game
            playerInput.actionEvents[5].AddListener(playerInputActions.StartButton);
            //Jump
            playerInput.actionEvents[6].AddListener(playerInputActions.JumpButton);
            //Dash
            playerInput.actionEvents[7].AddListener(playerInputActions.DashButton);

            playerInputActionsLookup[playerInput] = playerInputActions;
        }

        /// <summary>
        /// Adds an action to the PlayerInput's <see cref="PlayerInputActions.OnStartPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the start button</param>
        public static void RegisterStartButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnStartPressed += action;
        }
        
        /// <summary>
        /// Adds an action to the PlayerInput's <see cref="PlayerInputActions.OnSubmitPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the submit button</param>
        public static void RegisterSubmitButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnSubmitPressed += action;
        }
        
        /// <summary>
        /// Adds an action to the PlayerInput's <see cref="PlayerInputActions.OnBackPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the back button</param>
        public static void RegisterBackButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnBackPressed += action;
        }
        
        /// <summary>
        /// Adds an action to the PlayerInput's <see cref="PlayerInputActions.OnSelection"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player is using the left joystick or dpad</param>
        public static void RegisterSelectionAxis(PlayerInput playerInput, Action<Vector2> action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnSelection += action;
        }
        
        /// <summary>
        /// Adds an action to the PlayerInput's <see cref="PlayerInputActions.OnMovement"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// /// <param name="action">The action called when this player is using the left joystick</param>
        public static void RegisterMovementAxis(PlayerInput playerInput, Action<Vector2> action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnMovement += action;
        }
        
        /// <summary>
        /// Adds an action to the PlayerInput's <see cref="PlayerInputActions.OnDashPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the dash button</param>
        public static void RegisterDashButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnDashPressed += action;
        }
        
        /// <summary>
        /// Adds an action to the PlayerInput's <see cref="PlayerInputActions.OnJumpPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// /// <param name="action">The action called when this player pressed the jump button</param>
        public static void RegisterJumpButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnJumpPressed += action;
        }
        
        #endregion Registration
        
        #region UnRegistration

        /// <summary>
        /// Removes PlayerInput from the system
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        public void UnRegisterPlayer(PlayerInput playerInput)
        {
            Debug.Log($"{LogPrefix}Unregistering player with id {playerInput.user.id}");
        }

        /// <summary>
        /// Removes an action from the PlayerInput's <see cref="PlayerInputActions.OnStartPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the start button</param>
        public static void UnRegisterStartButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnStartPressed -= action;
        }
        
        /// <summary>
        /// Removes an action from the PlayerInput's <see cref="PlayerInputActions.OnSubmitPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the submit button</param>
        public static void UnRegisterSubmitButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnSubmitPressed -= action;
        }
        
        /// <summary>
        /// Removes an action from the PlayerInput's <see cref="PlayerInputActions.OnBackPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the back button</param>
        public static void UnRegisterBackButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnBackPressed -= action;
        }
        
        /// <summary>
        /// Removes an action from the PlayerInput's <see cref="PlayerInputActions.OnSelection"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player is using the left joystick or dpad</param>
        public static void UnRegisterSelectionAxis(PlayerInput playerInput, Action<Vector2> action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnSelection -= action;
        }
        
        /// <summary>
        /// Removes an action from the PlayerInput's <see cref="PlayerInputActions.OnMovement"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// /// <param name="action">The action called when this player is using the left joystick</param>
        public static void UnRegisterMovementAxis(PlayerInput playerInput, Action<Vector2> action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnMovement -= action;
        }
        
        /// <summary>
        /// Removes an action from the PlayerInput's <see cref="PlayerInputActions.OnDashPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// <param name="action">The action called when this player pressed the dash button</param>
        public static void UnRegisterDashButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnDashPressed -= action;
        }
        
        /// <summary>
        /// Removes an action from the PlayerInput's <see cref="PlayerInputActions.OnJumpPressed"/>
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        /// /// <param name="action">The action called when this player pressed the jump button</param>
        public static void UnRegisterJumpButton(PlayerInput playerInput, Action action)
        {
            if (playerInput == null)
            {
                Debug.Log($"{LogPrefix}playerInput cannot be null.");
                return;
            }
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {playerInput.user.id} could be found.");
                return;
            }
            
            Instance.playerInputActionsLookup[playerInput].OnJumpPressed -= action;
        }
        
        #endregion UnRegistration
    }

    public sealed class PlayerInputActions
    {
        public event Action OnStartPressed;
        
        //Menu events
        public event Action OnSubmitPressed;
        public event Action OnBackPressed;
        public event Action<Vector2> OnSelection; 
        
        //Game events
        public event Action<Vector2> OnMovement;
        public event Action OnDashPressed;
        public event Action OnJumpPressed; 
        
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
