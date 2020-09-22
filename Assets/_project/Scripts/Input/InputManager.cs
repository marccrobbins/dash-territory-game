using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DashTerritory
{
    public class InputManager : Manager<InputManager>
    {
        private const string LogPrefix = "INPUTMANAGER ::";
        
        //Events
        public static event Action<PlayerInputActions> OnPlayerJoined;
        
        //Private variables
        private Dictionary<PlayerInput, PlayerInputActions> playerInputActionsLookup;

        //Properties
        public List<PlayerInputActions> PlayerInputObjects => playerInputActionsLookup.Values.ToList();

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
            foreach (var playerInput in Instance.playerInputActionsLookup.Keys)
            {
                if (!playerInput) continue;
                playerInput.SwitchCurrentActionMap(type.ToString());
            }
        }

        /// <summary>
        /// Creates a <see cref="PlayerInputActions"/> object and adds listeners to playerInput.
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        public void RegisterPlayer(PlayerInput playerInput)
        {
            //Parent input under InputManager so it will persist across scenes
            playerInput.transform.SetParent(transform);
            
            var id = playerInput.user.id;
            Debug.Log($"{LogPrefix}Registering player with id {id}");
            if (playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}player with {id} has already been registered.");
                return;
            }
            
            var playerInputActions = playerInput.GetComponent<PlayerInputActions>();
            if (!playerInputActions)
            {
                Debug.LogError($"{LogPrefix}Could not find PlayerInputAction on {playerInput.gameObject.name}");
                return;
            }
            
            playerInputActions.Initialize(playerInput);
            playerInputActionsLookup[playerInput] = playerInputActions;
            
            OnPlayerJoined?.Invoke(playerInputActions);
        }
        
        /// <summary>
        /// Removes PlayerInput from the system
        /// </summary>
        /// <param name="playerInput">PlayerInput component attached to a specific character</param>
        public void UnRegisterPlayer(PlayerInput playerInput)
        {
            var id = playerInput.user.id;
            
            if (!Instance.playerInputActionsLookup.ContainsKey(playerInput))
            {
                Debug.Log($"{LogPrefix}No player with id {id} could be found.");
                return;
            }

            Debug.Log($"{LogPrefix}Unregistering player with id {playerInput.user.id}");

            Instance.playerInputActionsLookup.Remove(playerInput);
        }
    }

    public enum InputMapType
    {
        Menu,
        Game
    }
}
