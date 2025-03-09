using UnityEngine;
using UnityEngine.InputSystem;

public class PirateCamera : MonoBehaviour
{
    public static PirateCamera instance;

    [SerializeField]
    Transform pirateTransform;
    [SerializeField]
    Camera currentPirateCamera;

    [SerializeField, Tooltip("The speed which the camera rotates around the pirate using keyboard")]
    float rotationAmount = 100;
    [SerializeField, Tooltip("The speed which the camera rotates around the pirate using mouse")]
    float sensitivity = 100;
    [SerializeField, Tooltip("The speed the camera follows pirate")]
    float movementSpeed = 5;
    [SerializeField, Tooltip("How far back the camera should be from the pirate")]
    float cameraOffset;

    Vector2 mouseDelta;

    float currentY;
    float cameraRate;

    Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (instance == null)
            instance = this;

        if (pirateTransform == null)
            Debug.LogError("Pirate is Null");

        currentY = transform.position.y;

    }
    private void OnEnable()
    {
        GameEventManager.instance.inputEvents.PirateCameraTurnEvent += HandlePirateCameraTurnInput;
        GameEventManager.instance.inputEvents.PirateMousePosEvent += HandlePirateMouseInput;
        
    }
    private void OnDisable()
    {
        GameEventManager.instance.inputEvents.PirateCameraTurnEvent -= HandlePirateCameraTurnInput;
        GameEventManager.instance.inputEvents.PirateMousePosEvent -= HandlePirateMouseInput;
        
    }

    void FixedUpdate()
    {

        RotateCamera();
        RotateCameraWithMouse();
        FollowPirate();
    }

    void FollowPirate()
    {
        Vector3 newLoc = Vector3.Lerp(transform.position, pirateTransform.position, movementSpeed * Time.deltaTime);
        newLoc.y = currentY;
        transform.position = newLoc;
    }

    void RotateCamera()
    {
        if (Mathf.Abs(cameraRate) > 0.1)
        {
            float rotationStep = -cameraRate * rotationAmount * Time.deltaTime;
            transform.RotateAround(pirateTransform.position, Vector3.up, rotationStep);
        }
    }

    void RotateCameraWithMouse()
    {
        if (Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            float rotationStep = mouseDelta.x * sensitivity * Time.deltaTime;
            transform.RotateAround(pirateTransform.position, Vector3.up, rotationStep);
        }
    }

    void HandlePirateCameraTurnInput(float val)
    {
        cameraRate = val;
    }

    void HandlePirateMouseInput(Vector2 val)
    {
        mouseDelta = val;
    }

}
