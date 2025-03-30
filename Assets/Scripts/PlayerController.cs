using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ECurrentController
{
    Pirate = 0,
    Ship = 1,
    UI = 2
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }

    public event Action PirateControllerEvent;
    public event Action ShipControllerEvent;
    public event Action UIControllerEvent;

    InputReader controllerInputs;
    ECurrentController E_Controller = ECurrentController.Pirate;

    void Start()
    {
        if (instance != null)
            Debug.LogError("Multiple instances of PlayerController exist");

        instance = this;
        controllerInputs.SetPirateController();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnEnable()
    {
        controllerInputs = GameEventManager.instance.inputEvents;
        GameEventManager.instance.dialougeEvents.onDialogueStarted += SetUIController;
        GameEventManager.instance.dialougeEvents.onDialogueFinished += SetPirateController;
        controllerInputs.SwapEvent += SetPirateController;
    }

    void OnDisable()
    {
        GameEventManager.instance.dialougeEvents.onDialogueStarted -= SetUIController;
        GameEventManager.instance.dialougeEvents.onDialogueFinished -= SetPirateController;
        controllerInputs.SwapEvent -= HandleSwapInput;
    }

    public void SetUIController()
    {
        if (E_Controller == ECurrentController.UI)
        {
            Debug.LogWarning("SetUIController was called even though Pirate Controller is active");
            return;
        }

        controllerInputs.SetUIController();
        E_Controller = ECurrentController.UI;
        UIControllerEvent?.Invoke();
    }

    public void SetPirateController()
    {
        if (E_Controller == ECurrentController.Pirate)
        {
            Debug.LogWarning("SetPirateController was called even though Pirate Controller is active");
            return;
        }
        controllerInputs.SetPirateController();
        E_Controller = ECurrentController.Pirate;
    }

    public void SetPirateController(float val)
    {
        if(E_Controller == ECurrentController.Pirate)
        {
            Debug.LogWarning("SetPirateController was called even though Pirate Controller is active");
            return;
        }
        controllerInputs.SetPirateController();
        E_Controller = ECurrentController.Pirate;
        PirateControllerEvent?.Invoke();
    }

    public void SetShipController()
    {
        if (E_Controller == ECurrentController.Ship)
        {
            Debug.LogWarning("SetShipController was called even though Pirate Controller is active");
            return;
        }
        controllerInputs.SetShipController();
        E_Controller = ECurrentController.Ship;
        ShipControllerEvent?.Invoke();
    }

    void HandleSwapInput(float val)
    {
        switch (E_Controller)
        {
            case ECurrentController.Pirate:
                controllerInputs.SetShipController();
                E_Controller = ECurrentController.Ship;
                ShipControllerEvent?.Invoke();
                break;
            case ECurrentController.Ship:
                controllerInputs.SetPirateController();
                E_Controller = ECurrentController.Pirate;
                PirateControllerEvent?.Invoke();
                break;
        }
    }
}
