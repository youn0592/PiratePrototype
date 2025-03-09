using System;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "ScriptableObjects/InputReader", order = 2)]
public class InputReader : ScriptableObject, PirateController.IShipControlActions, PirateController.IPirateControlActions, PirateController.IUIControlActions
{
    private PirateController _pController;

    //Ship Events
    public event Action<float> ShipCameraTurnEvent;
    public event Action<float> ShipAccelerateEvent;
    public event Action<float> ShipTurnEvent;
    public event Action<Vector2> ShipMousePosEvent;

    //Player Events
    public event Action<float> PirateForwardEvent;
    public event Action<float> PirateStrafeEvent;
    public event Action<float> PirateCameraTurnEvent;
    public event Action<float> PirateInteractEvent;
    public event Action<float> PirateSubmitEvent;
    public event Action<float> PirateMouseButton;
    public event Action<float> PirateTestEvent;
    public event Action<Vector2> PirateMousePosEvent;

    public event Action<float> SwapEvent;


    private void OnEnable()
    {
        if (_pController == null)
        {
            _pController = new PirateController();
            _pController.ShipControl.SetCallbacks(this);
            _pController.PirateControl.SetCallbacks(this);
            _pController.UIControl.SetCallbacks(this);
        }

        _pController.ShipControl.Enable();
        _pController.PirateControl.Disable();
        _pController.UIControl.Disable();
    }

    private void OnDisable()
    {
        _pController.ShipControl.Disable();
        _pController.PirateControl.Disable();
        _pController.UIControl.Disable();
    }

    public void SetShipController()
    {
        _pController.ShipControl.Enable();
        _pController.PirateControl.Disable();
        _pController.UIControl.Disable();
    }
    public void SetPirateController()
    {
        _pController.PirateControl.Enable();
        _pController.ShipControl.Disable();
        _pController.UIControl.Disable();
    }
    public void SetUIController()
    {
        _pController.UIControl.Enable();
        _pController.ShipControl.Disable();
        _pController.PirateControl.Disable();
    }

    public void OnShipCameraTurn(InputAction.CallbackContext context)
    {
        ShipCameraTurnEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnShipMousePos(InputAction.CallbackContext context)
    {
        ShipMousePosEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnShipAccelerate(InputAction.CallbackContext context)
    {
        ShipAccelerateEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnShipTurn(InputAction.CallbackContext context)
    {
        ShipTurnEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnShipSwap(InputAction.CallbackContext context)
    {
        SwapEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnPirateForward(InputAction.CallbackContext context)
    {
        PirateForwardEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnPirateCameraRotate(InputAction.CallbackContext context)
    {
        PirateCameraTurnEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnPirateMousePos(InputAction.CallbackContext context)
    {
        PirateMousePosEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPirateMouseClick(InputAction.CallbackContext context)
    {
        PirateMouseButton?.Invoke(context.ReadValue<float>());
    }

    public void OnPirateStrafe(InputAction.CallbackContext context)
    {
        PirateStrafeEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnPirateInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PirateInteractEvent?.Invoke(context.ReadValue<float>());
    }
    public void OnPirateSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PirateSubmitEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnPirateTest(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PirateTestEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnPirateSwap(InputAction.CallbackContext context)
    {
        SwapEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnUITest(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

}