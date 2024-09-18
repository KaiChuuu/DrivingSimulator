using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    [SerializeField] Transform steeringWheelTransform;

    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0f;

   
    public float maxWheelTurnAngle = 110f;
    public float wheelTurnSpeed = 15f;
    public float wheelReturnSpeed = 250f;

    private float currentWheelRotation = 0f; // Keeping track of steering wheel rotation

    public float maxSpeed = 50f;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        currentAcceleration = acceleration * Input.GetAxis("Vertical");
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");

        UpdateVerticalAcceleration();

        UpdateBrake();

        UpdateHorizontalTurn();

        UpdateWheel(frontLeft, frontRightTransform);
        UpdateWheel(frontRight, frontLeftTransform);
        UpdateWheel(backLeft, backRightTransform);
        UpdateWheel(backRight, backLeftTransform);

        UpdateWheelTransform();
    }

    void UpdateHorizontalTurn()
    {
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
    }

    void UpdateVerticalAcceleration()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            //2 wheel drive (i.e. only front wheels exert movement)
            frontRight.motorTorque = currentAcceleration;
            frontLeft.motorTorque = currentAcceleration;
        } else if (rb.velocity.magnitude > maxSpeed + 5) //Hysteresis
        {
            frontRight.motorTorque = 0;
            frontLeft.motorTorque = 0;
        }
    }

    void UpdateBrake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentBreakForce = breakingForce;
        }
        else
        {
            currentBreakForce = 0f;
        }

        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;
    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);
        
        trans.position = position;
        trans.rotation = rotation;
    }

    void UpdateWheelTransform()
    {
        float wheelAngle = wheelTurnSpeed * currentTurnAngle * Time.deltaTime;

        float newWheelRotation;
        float clampedRotation;

        // if player released turn keys
        if (currentTurnAngle == 0){
            //re-center wheel
            newWheelRotation = Mathf.MoveTowards(currentWheelRotation, 0f, wheelReturnSpeed * Time.deltaTime);
        } 
        else
        {
            newWheelRotation = Mathf.Clamp(currentWheelRotation + wheelAngle, -maxWheelTurnAngle, maxWheelTurnAngle);
        }

        // new rotation amount applied to the wheel
        clampedRotation = newWheelRotation - currentWheelRotation;

        steeringWheelTransform.Rotate(0f, 0f, clampedRotation);

        currentWheelRotation = newWheelRotation;
    }
}
