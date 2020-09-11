// GENERATED AUTOMATICALLY FROM 'Assets/_project/Data/Input/GameInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace DashTerritory
{
    public class @GameInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @GameInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInput"",
    ""maps"": [
        {
            ""name"": ""Menu"",
            ""id"": ""751a1e6f-b2ab-4a21-8bc4-7d153bc46999"",
            ""actions"": [
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""49e01106-f44f-454f-ab13-23a8b7c153ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""f5b12324-14de-40db-b245-4af51151c627"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                },
                {
                    ""name"": ""Selection"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fbdb53a4-0755-46d4-8062-27a8109f6d3c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""310806a7-bf15-4dcd-8dc4-d878d3d98166"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""512f2952-fc7f-4ff9-91d7-a02225a4a994"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""DPad"",
                    ""id"": ""c0d6f28d-e526-4876-b849-be458b1f7cc4"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0124303a-9410-450d-a75f-6a3d17c1c194"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8b56b7a9-e060-4986-bedb-56ea2207a5c7"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""966028ed-7aa3-4e57-9323-7f0aa2098b5d"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3f2e4dfa-c275-469e-aeb7-372b4c693b1e"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftJoystick"",
                    ""id"": ""9ef95e2e-d22e-4181-8494-d9104ae934f9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b4d54556-3349-4de7-b8cd-532737cddeb8"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bc79f15e-7c24-4d17-ae36-44965de4b737"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bd89de64-cbf5-469e-816c-aade13cc2765"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2cf603c8-865b-488e-a5ee-b273c000ea03"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5ed68848-f170-43fb-bd71-5249d5e700f4"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7804cc89-d1c7-4955-adbf-a8bfa9b90847"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Game"",
            ""id"": ""0a721952-7535-41ec-9f9f-125710cd3e9c"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""58c3a63e-4ade-495d-bff8-5ff0af0b1e27"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""5404092b-5943-4a14-9497-af887d4754f1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""b7ad4a39-c7db-4910-b025-7121444c7b86"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""7aeaa257-e95f-4fbe-909a-01e5242fa7ae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""LeftJoystick"",
                    ""id"": ""f54d2e5b-2981-48a0-8eb9-6494290c726a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b5cd5771-0294-4a0c-b3eb-6fbdf538ea54"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""becfb6d0-57c6-49d9-87a9-91f48dae2121"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""37fd2ec9-65dd-4887-85a2-dde7513c2e7a"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6decdb56-96a0-485b-9258-5838e3873d5d"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6654846e-b023-427d-a3c9-54b607c5e9bf"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79a26712-34ac-45ca-958b-984abeeca151"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ded6fb6a-39b6-4500-8113-d7cfd902a976"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Menu
            m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
            m_Menu_Submit = m_Menu.FindAction("Submit", throwIfNotFound: true);
            m_Menu_Back = m_Menu.FindAction("Back", throwIfNotFound: true);
            m_Menu_Selection = m_Menu.FindAction("Selection", throwIfNotFound: true);
            m_Menu_Start = m_Menu.FindAction("Start", throwIfNotFound: true);
            // Game
            m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
            m_Game_Movement = m_Game.FindAction("Movement", throwIfNotFound: true);
            m_Game_Start = m_Game.FindAction("Start", throwIfNotFound: true);
            m_Game_Jump = m_Game.FindAction("Jump", throwIfNotFound: true);
            m_Game_Dash = m_Game.FindAction("Dash", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Menu
        private readonly InputActionMap m_Menu;
        private IMenuActions m_MenuActionsCallbackInterface;
        private readonly InputAction m_Menu_Submit;
        private readonly InputAction m_Menu_Back;
        private readonly InputAction m_Menu_Selection;
        private readonly InputAction m_Menu_Start;
        public struct MenuActions
        {
            private @GameInput m_Wrapper;
            public MenuActions(@GameInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Submit => m_Wrapper.m_Menu_Submit;
            public InputAction @Back => m_Wrapper.m_Menu_Back;
            public InputAction @Selection => m_Wrapper.m_Menu_Selection;
            public InputAction @Start => m_Wrapper.m_Menu_Start;
            public InputActionMap Get() { return m_Wrapper.m_Menu; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
            public void SetCallbacks(IMenuActions instance)
            {
                if (m_Wrapper.m_MenuActionsCallbackInterface != null)
                {
                    @Submit.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                    @Submit.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                    @Submit.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                    @Back.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                    @Back.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                    @Back.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                    @Selection.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSelection;
                    @Selection.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSelection;
                    @Selection.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSelection;
                    @Start.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnStart;
                    @Start.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnStart;
                    @Start.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnStart;
                }
                m_Wrapper.m_MenuActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Submit.started += instance.OnSubmit;
                    @Submit.performed += instance.OnSubmit;
                    @Submit.canceled += instance.OnSubmit;
                    @Back.started += instance.OnBack;
                    @Back.performed += instance.OnBack;
                    @Back.canceled += instance.OnBack;
                    @Selection.started += instance.OnSelection;
                    @Selection.performed += instance.OnSelection;
                    @Selection.canceled += instance.OnSelection;
                    @Start.started += instance.OnStart;
                    @Start.performed += instance.OnStart;
                    @Start.canceled += instance.OnStart;
                }
            }
        }
        public MenuActions @Menu => new MenuActions(this);

        // Game
        private readonly InputActionMap m_Game;
        private IGameActions m_GameActionsCallbackInterface;
        private readonly InputAction m_Game_Movement;
        private readonly InputAction m_Game_Start;
        private readonly InputAction m_Game_Jump;
        private readonly InputAction m_Game_Dash;
        public struct GameActions
        {
            private @GameInput m_Wrapper;
            public GameActions(@GameInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Game_Movement;
            public InputAction @Start => m_Wrapper.m_Game_Start;
            public InputAction @Jump => m_Wrapper.m_Game_Jump;
            public InputAction @Dash => m_Wrapper.m_Game_Dash;
            public InputActionMap Get() { return m_Wrapper.m_Game; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
            public void SetCallbacks(IGameActions instance)
            {
                if (m_Wrapper.m_GameActionsCallbackInterface != null)
                {
                    @Movement.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMovement;
                    @Movement.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMovement;
                    @Movement.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMovement;
                    @Start.started -= m_Wrapper.m_GameActionsCallbackInterface.OnStart;
                    @Start.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnStart;
                    @Start.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnStart;
                    @Jump.started -= m_Wrapper.m_GameActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnJump;
                    @Dash.started -= m_Wrapper.m_GameActionsCallbackInterface.OnDash;
                    @Dash.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnDash;
                    @Dash.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnDash;
                }
                m_Wrapper.m_GameActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Movement.started += instance.OnMovement;
                    @Movement.performed += instance.OnMovement;
                    @Movement.canceled += instance.OnMovement;
                    @Start.started += instance.OnStart;
                    @Start.performed += instance.OnStart;
                    @Start.canceled += instance.OnStart;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Dash.started += instance.OnDash;
                    @Dash.performed += instance.OnDash;
                    @Dash.canceled += instance.OnDash;
                }
            }
        }
        public GameActions @Game => new GameActions(this);
        public interface IMenuActions
        {
            void OnSubmit(InputAction.CallbackContext context);
            void OnBack(InputAction.CallbackContext context);
            void OnSelection(InputAction.CallbackContext context);
            void OnStart(InputAction.CallbackContext context);
        }
        public interface IGameActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnStart(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnDash(InputAction.CallbackContext context);
        }
    }
}
