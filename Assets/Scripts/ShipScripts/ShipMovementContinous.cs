using UnityEngine;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class ShipMovementContinous : MonoBehaviour
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
    [SerializeField]
    float changeDirectionDeceleration = 2f;
    [SerializeField]
    float sailLoweringSpeed = 0.25f;
    [SerializeField]
    TextMeshProUGUI sailText;
    [SerializeField]
    InputReader input;

    //Floats to set Input values
    float moveDir;
    float turnDir;

    float sailAmount = 0.0f;

    Vector3 velocity;

    Vector3 forwardVector;
    float turnVelocity;


    private void Start()
    {
        rigBod = GetComponent<Rigidbody>();

        if (rigBod == null)
        {
            Debug.LogError(this + " does not has a Rigidbody component");
        }


        input.ShipAccelerateEvent += HandleAccelerateInput;
        input.ShipTurnEvent += HandleTurnInput;
    }

    private void OnDestroy()
    {
        input.ShipAccelerateEvent -= HandleAccelerateInput;
        input.ShipTurnEvent -= HandleTurnInput;
    }

    private void FixedUpdate()
    {
        SetSail();
        MoveShip();
        TurnShip();
        TurnDrift();

        rigBod.linearVelocity = velocity;
        rigBod.rotation = rigBod.rotation * Quaternion.Euler(0, turnVelocity * Time.fixedDeltaTime, 0);
    }

    private void MoveShip()
    {

        if (Mathf.Abs(sailAmount) < 0.1f)
        {
            velocity += -velocity.normalized * moveAcceleration.y * Time.fixedDeltaTime;
        }
        else
        {
            velocity += transform.forward * moveAcceleration.x * sailAmount * Time.fixedDeltaTime;
        }


        velocity = Vector3.ClampMagnitude(velocity, maxMoveSpeed);
    }

    private void SetSail()
    {
        if(moveDir != 0.0f)
        {
            sailAmount += moveDir * sailLoweringSpeed * Time.deltaTime;
            sailAmount = Mathf.Clamp(sailAmount, 0, 1);
        }

        sailText.SetText(sailAmount.ToString());
    }

    private void TurnShip()
    {

        bool bTurningChange = Mathf.Sign(turnDir) != Mathf.Sign(turnVelocity) && Mathf.Abs(turnVelocity) > 0.1f;

        if (Mathf.Abs(turnDir) < 0.1f)
        {
            turnVelocity = Mathf.MoveTowards(turnVelocity, 0, turnAcceleration.y * Time.fixedDeltaTime);
        }
        else if (bTurningChange)
        {
            turnVelocity = Mathf.MoveTowards(turnVelocity, 0, turnAcceleration.y * changeDirectionDeceleration * Time.fixedDeltaTime);
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


    //Possible to move to a Contorller Class?
    void HandleAccelerateInput(float val)
    {
        moveDir = val;
    }
    void HandleTurnInput(float val)
    {
        turnDir = val;
    }
}
