using System;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, PirateController.IShipControlActions, PirateController.IPirateControlActions, PirateController.IUIControlActions
{

    private PirateController _pController;

    public event Action<float> CameraTurnEvent;
    public event Action<float> ShipAccelerateEvent;
    public event Action<float> ShipTurnEvent;

    public event Action<Vector2> MousePosEvent;

    private void OnEnable()
    {
        if(_pController == null)
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

    public void OnCameraTurn(InputAction.CallbackContext context)
    {
        CameraTurnEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnMousePos(InputAction.CallbackContext context)
    {
        MousePosEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnNewaction(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnShipAccelerate(InputAction.CallbackContext context)
    {
        ShipAccelerateEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnShipTurn(InputAction.CallbackContext context)
    {
        ShipTurnEvent?.Invoke(context.ReadValue<float>());
    }
}
