// GENERATED AUTOMATICALLY FROM 'Assets/Input/TouchInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @TouchInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @TouchInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TouchInput"",
    ""maps"": [
        {
            ""name"": ""Touch"",
            ""id"": ""db4f195f-e8d0-419c-8a6e-ae85c9de84b9"",
            ""actions"": [
                {
                    ""name"": ""Press"",
                    ""type"": ""Button"",
                    ""id"": ""0215c363-719d-4bbb-bd7c-25e29109adc8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""PassThrough"",
                    ""id"": ""61bbc2f6-32ba-4359-8fbf-2ae3b6d1ed0f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Radius"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e3fd5bdd-0c5a-4266-885a-8a6111a3d9ed"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6dc53e10-13c0-4d02-bdb1-c6e4367c5087"",
                    ""path"": ""<Touchscreen>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Press"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9081c207-8e5b-4c30-aef8-f5124ccf7822"",
                    ""path"": ""<Touchscreen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7184a1dc-b6f0-457d-8a9f-c50fa92fb94d"",
                    ""path"": ""<Touchscreen>/primaryTouch/radius"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Radius"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Touch
        m_Touch = asset.FindActionMap("Touch", throwIfNotFound: true);
        m_Touch_Press = m_Touch.FindAction("Press", throwIfNotFound: true);
        m_Touch_Position = m_Touch.FindAction("Position", throwIfNotFound: true);
        m_Touch_Radius = m_Touch.FindAction("Radius", throwIfNotFound: true);
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

    // Touch
    private readonly InputActionMap m_Touch;
    private ITouchActions m_TouchActionsCallbackInterface;
    private readonly InputAction m_Touch_Press;
    private readonly InputAction m_Touch_Position;
    private readonly InputAction m_Touch_Radius;
    public struct TouchActions
    {
        private @TouchInput m_Wrapper;
        public TouchActions(@TouchInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Press => m_Wrapper.m_Touch_Press;
        public InputAction @Position => m_Wrapper.m_Touch_Position;
        public InputAction @Radius => m_Wrapper.m_Touch_Radius;
        public InputActionMap Get() { return m_Wrapper.m_Touch; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchActions set) { return set.Get(); }
        public void SetCallbacks(ITouchActions instance)
        {
            if (m_Wrapper.m_TouchActionsCallbackInterface != null)
            {
                @Press.started -= m_Wrapper.m_TouchActionsCallbackInterface.OnPress;
                @Press.performed -= m_Wrapper.m_TouchActionsCallbackInterface.OnPress;
                @Press.canceled -= m_Wrapper.m_TouchActionsCallbackInterface.OnPress;
                @Position.started -= m_Wrapper.m_TouchActionsCallbackInterface.OnPosition;
                @Position.performed -= m_Wrapper.m_TouchActionsCallbackInterface.OnPosition;
                @Position.canceled -= m_Wrapper.m_TouchActionsCallbackInterface.OnPosition;
                @Radius.started -= m_Wrapper.m_TouchActionsCallbackInterface.OnRadius;
                @Radius.performed -= m_Wrapper.m_TouchActionsCallbackInterface.OnRadius;
                @Radius.canceled -= m_Wrapper.m_TouchActionsCallbackInterface.OnRadius;
            }
            m_Wrapper.m_TouchActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Press.started += instance.OnPress;
                @Press.performed += instance.OnPress;
                @Press.canceled += instance.OnPress;
                @Position.started += instance.OnPosition;
                @Position.performed += instance.OnPosition;
                @Position.canceled += instance.OnPosition;
                @Radius.started += instance.OnRadius;
                @Radius.performed += instance.OnRadius;
                @Radius.canceled += instance.OnRadius;
            }
        }
    }
    public TouchActions @Touch => new TouchActions(this);
    public interface ITouchActions
    {
        void OnPress(InputAction.CallbackContext context);
        void OnPosition(InputAction.CallbackContext context);
        void OnRadius(InputAction.CallbackContext context);
    }
}
