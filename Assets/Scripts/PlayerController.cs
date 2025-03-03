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

    [SerializeField]
    GameObject pirateObj;
    [SerializeField]
    GameObject shipObj;

    ECurrentController E_Controller = ECurrentController.Ship;

    [SerializeField]
    InputReader input;

    void Start()
    {
        if (instance == null)
            instance = this;

        if(pirateObj == null || shipObj == null)
        {
            Debug.LogError("Pirate Obj or Ship Obj is null in " + this);
        }

        pirateObj.SetActive(false);
        shipObj.SetActive(true);
        
        input.SetShipController();
        input.SwapEvent += HandleSwapInput;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDestroy()
    {
        input.SwapEvent -= HandleSwapInput;
    }

    void HandleSwapInput(float val)
    {
        switch(E_Controller)
        {
            case ECurrentController.Pirate:
                pirateObj.SetActive(false);
                shipObj.SetActive(true);
                input.SetShipController();
                E_Controller = ECurrentController.Ship;
                break;
            case ECurrentController.Ship:
                shipObj.SetActive(false);
                pirateObj.SetActive(true);
                input.SetPirateController();
                E_Controller = ECurrentController.Pirate;
                break;
        }
    }
}
