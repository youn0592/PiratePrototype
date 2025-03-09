using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PirateMovement : MonoBehaviour
{

    Rigidbody rigBod;
    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float acceleration = 5;
    [SerializeField]
    float rotationSpeed = 5;
    [SerializeField]
    Transform cameraRig;

    float forwardDir;
    float strafeDir;

    Vector3 moveDir;
    Vector3 velocity;


    void Start()
    {
        rigBod = GetComponent<Rigidbody>();

        if(rigBod == null)
        {
            Debug.LogError(this + " does not has a Rigidbody component");
        }

        if (cameraRig == null)
            Debug.LogError(this + " does not have CameraRig Hooked up and will cause errors");

        rigBod.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    }
    private void OnEnable()
    {
        GameEventManager.instance.inputEvents.SetPirateController();
        GameEventManager.instance.inputEvents.PirateForwardEvent += HandleForwardInput;
        GameEventManager.instance.inputEvents.PirateStrafeEvent += HandleStrafeInput;
        
    }
    private void OnDisable()
    {
        GameEventManager.instance.inputEvents.PirateForwardEvent -= HandleForwardInput;
        GameEventManager.instance.inputEvents.PirateStrafeEvent -= HandleStrafeInput;
    }

    void FixedUpdate()
    {
        MovePirate();
        RotatePirate();
    }

    void MovePirate()
    {
        Vector3 forwardCam = cameraRig.forward;
        Vector3 rightCam = cameraRig.right;

        forwardCam.y = 0;
        rightCam.y = 0;

        forwardCam.Normalize();
        rightCam.Normalize();

        moveDir = (rightCam * strafeDir + forwardCam * forwardDir).normalized;
        velocity = moveDir * moveSpeed;
        rigBod.linearVelocity = Vector3.Lerp(rigBod.linearVelocity, new Vector3(velocity.x, rigBod.linearVelocity.y, velocity.z), acceleration * Time.fixedDeltaTime) ;
    }

    void RotatePirate()
    {
        if(moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void HandleForwardInput(float val)
    {
        forwardDir = val;
    }
    void HandleStrafeInput(float val)
    {
        strafeDir = val;
    }
}
