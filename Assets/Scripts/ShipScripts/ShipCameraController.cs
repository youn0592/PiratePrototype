using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipCameraController : MonoBehaviour
{
    public static ShipCameraController instance;

    [SerializeField]
    Transform shipTransform;
    [SerializeField]
    Camera currentShipCamera;

    [SerializeField, Tooltip("The speed which the camera rotates around the ship using keyboard")]
    float rotationAmount = 100;
    [SerializeField, Tooltip("The speed which the camera rotates around the ship using mouse")]
    float sensitivity = 100;
    [SerializeField, Tooltip("The speed the camera follows ship")]
    float movementSpeed = 5;
    [SerializeField, Tooltip("How far back the camera should be from the ship")]
    float cameraOffset;
    [SerializeField, Tooltip("How long after a player let go of key before reseting camera")]
    float cameraReset = 5f;

    Vector2 mouseDelta;

    float currentY;
    float cameraRate;
    float cameraTimer = 0f;

    [SerializeField]
    InputReader input;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
            instance = this;

        if (shipTransform == null)
            Debug.LogError("Ship is Null");

        currentY = transform.position.y;

        input.ShipCameraTurnEvent += HandleShipCameraTurnInput;
        input.ShipMousePosEvent += HandleShipMousePosInput;
    }

    private void OnDestroy()
    {
        input.ShipCameraTurnEvent -= HandleShipCameraTurnInput;
        input.PirateMousePosEvent -= HandleShipMousePosInput;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowShip();

        if(Mathf.Abs(cameraRate) > 0.1f)
        {
            cameraTimer = 0f;
            RotateCamera();
        }
        else if(Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            cameraTimer = 0f;
            RotateCameraWithMouse();
        }
        else
        {
            cameraTimer += Time.deltaTime;
        }

        if(cameraTimer > cameraReset)
        {
            ResetCamera();
        }
        
    }

    void FollowShip()
    {
        Vector3 newLoc = Vector3.Lerp(transform.position, shipTransform.position, movementSpeed * Time.deltaTime);
        newLoc.y = currentY;
        transform.position = newLoc;
    }

    void RotateCamera()
    {
        if(Mathf.Abs(cameraRate) > 0.1f)
        {
            float rotationStep = cameraRate * rotationAmount * Time.deltaTime;
            transform.RotateAround(shipTransform.position, Vector3.up, rotationStep);
        }
    }

    void RotateCameraWithMouse()
    {
        float rotationStep = mouseDelta.x * sensitivity * Time.deltaTime;
        transform.RotateAround(shipTransform.position, Vector3.up, rotationStep);
    }

    void ResetCamera()
    {
        Vector3 targetPosition = transform.position - transform.forward * cameraOffset;
        targetPosition.y = currentY;

        transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(shipTransform.forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, movementSpeed * Time.deltaTime);
    }    

    void HandleShipMousePosInput(Vector2 val)
    {
        mouseDelta = val;
    }
    void HandleShipCameraTurnInput(float val)
    {
        cameraRate = val;
    }


}
