//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Inputs/GolfControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GolfControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GolfControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GolfControls"",
    ""maps"": [
        {
            ""name"": ""Golf"",
            ""id"": ""f796ab32-a74c-4452-aeb9-d9fb87b0e468"",
            ""actions"": [
                {
                    ""name"": ""Toggle Backswing"",
                    ""type"": ""Button"",
                    ""id"": ""fb231cca-8858-4734-9caa-6e24466bb854"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Backswinging"",
                    ""type"": ""Value"",
                    ""id"": ""cbaed72e-8d7b-4186-a18c-01c8997fcaa3"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""a2e931ef-724e-4772-a1f7-09bee7a6ad0d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ee274bc0-464c-428a-a5e1-9bb582ec9fa5"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Backswinging"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc03f19d-f317-456b-b5f7-c0789e8fda14"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""986ea948-70ed-412f-8c49-7ae96722d595"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6bc0daf-765c-4db5-ac8c-8f2dcbf09f64"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Toggle Backswing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c7bd8b9d-eafb-4972-92ec-b42b45ba93aa"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Toggle Backswing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Golf
        m_Golf = asset.FindActionMap("Golf", throwIfNotFound: true);
        m_Golf_ToggleBackswing = m_Golf.FindAction("Toggle Backswing", throwIfNotFound: true);
        m_Golf_Backswinging = m_Golf.FindAction("Backswinging", throwIfNotFound: true);
        m_Golf_Look = m_Golf.FindAction("Look", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Golf
    private readonly InputActionMap m_Golf;
    private List<IGolfActions> m_GolfActionsCallbackInterfaces = new List<IGolfActions>();
    private readonly InputAction m_Golf_ToggleBackswing;
    private readonly InputAction m_Golf_Backswinging;
    private readonly InputAction m_Golf_Look;
    public struct GolfActions
    {
        private @GolfControls m_Wrapper;
        public GolfActions(@GolfControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleBackswing => m_Wrapper.m_Golf_ToggleBackswing;
        public InputAction @Backswinging => m_Wrapper.m_Golf_Backswinging;
        public InputAction @Look => m_Wrapper.m_Golf_Look;
        public InputActionMap Get() { return m_Wrapper.m_Golf; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GolfActions set) { return set.Get(); }
        public void AddCallbacks(IGolfActions instance)
        {
            if (instance == null || m_Wrapper.m_GolfActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GolfActionsCallbackInterfaces.Add(instance);
            @ToggleBackswing.started += instance.OnToggleBackswing;
            @ToggleBackswing.performed += instance.OnToggleBackswing;
            @ToggleBackswing.canceled += instance.OnToggleBackswing;
            @Backswinging.started += instance.OnBackswinging;
            @Backswinging.performed += instance.OnBackswinging;
            @Backswinging.canceled += instance.OnBackswinging;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
        }

        private void UnregisterCallbacks(IGolfActions instance)
        {
            @ToggleBackswing.started -= instance.OnToggleBackswing;
            @ToggleBackswing.performed -= instance.OnToggleBackswing;
            @ToggleBackswing.canceled -= instance.OnToggleBackswing;
            @Backswinging.started -= instance.OnBackswinging;
            @Backswinging.performed -= instance.OnBackswinging;
            @Backswinging.canceled -= instance.OnBackswinging;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
        }

        public void RemoveCallbacks(IGolfActions instance)
        {
            if (m_Wrapper.m_GolfActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGolfActions instance)
        {
            foreach (var item in m_Wrapper.m_GolfActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GolfActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GolfActions @Golf => new GolfActions(this);
    public interface IGolfActions
    {
        void OnToggleBackswing(InputAction.CallbackContext context);
        void OnBackswinging(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
    }
}
