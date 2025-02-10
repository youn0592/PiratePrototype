using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{

    //Things the move class need, Acceleration and Decleration. Turning happens exponently.

    Rigidbody rigBod;

    [SerializeField, Tooltip("X - Accelerate, Y - Decelerate")]
    Vector2 moveAcceleration, turnAcceleration;     //Acceleration and Deceleration for Turning and Moving. X is Acceleration, Y is for Deceleration
    [SerializeField]
    float maxMoveSpeed = 50f;   //The max speed for moving the ship
    [SerializeField]
    float maxTurnSpeed = 50f;
    [SerializeField]
    float driftFactor = 0.9f;

    //Floats to set Input values
    float moveDir;
    float turnDir;

    Vector3 velocity;

    Vector3 forwardVector;
    float turnVelocity;


    //Input Handles
    public InputActionReference moveHandle;
    public InputActionReference turnHandle;

    private void Start()
    {
        rigBod = GetComponent<Rigidbody>();

        if(rigBod == null)
        {
            Debug.LogError(this + " does not has a Rigidbody component");
        }
    }

    private void FixedUpdate()
    {
        moveDir = moveHandle.action.ReadValue<float>();
        turnDir = turnHandle.action.ReadValue<float>();

        MoveShip();
        TurnShip();
        TurnDrift();

        rigBod.linearVelocity = velocity;
        rigBod.rotation = rigBod.rotation * Quaternion.Euler(0, turnVelocity * Time.fixedDeltaTime, 0);
    }

    private void MoveShip()
    {

        if (Mathf.Abs(moveDir) < 0.1f)
        {
            velocity += -velocity.normalized * moveAcceleration.y * Time.fixedDeltaTime;
        }
        else
        {
            velocity += transform.forward * moveAcceleration.x * moveDir * Time.fixedDeltaTime;
        }


        velocity = Vector3.ClampMagnitude(velocity, maxMoveSpeed);
    }

    private void TurnShip()
    {
        if(Mathf.Abs(turnDir) < 0.1f)
        {
            turnVelocity = Mathf.MoveTowards(turnVelocity, 0, turnAcceleration.y * Time.fixedDeltaTime);
        }
        else
        {
            turnVelocity += turnDir * turnAcceleration.x * Time.fixedDeltaTime;
            turnVelocity = Mathf.Clamp(turnVelocity, -maxTurnSpeed, maxTurnSpeed);
        }
    }

    void TurnDrift()
    {
        forwardVector = Vector3.Project(rigBod.linearVelocity, transform.forward);
        Vector3 lateralVelocity = velocity - forwardVector;
        lateralVelocity *= driftFactor;


        velocity = forwardVector + lateralVelocity;
    }
}
