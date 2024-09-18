using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITruck : MonoBehaviour
{
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    private float currentAcceleration = 0f;
    public float acceleration = 500f;
    public float maxSpeed = 60f;
    Rigidbody rb;

    public bool forward = true;

    public ChunkLoader chunkLoader;
    float offset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        offset = chunkLoader.chunkSize / 2 - 30;

        FloatingOriginController.OnFloatingOrigin += UpdateTruckPosition;
    }

    void OnDestroy()
    {
        FloatingOriginController.OnFloatingOrigin -= UpdateTruckPosition;    
    }

    private void FixedUpdate()
    {
        UpdateTruckWrap();

        currentAcceleration = acceleration;

        UpdateVerticalAcceleration();

        UpdateWheel(frontLeft, frontRightTransform);
        UpdateWheel(frontRight, frontLeftTransform);
        UpdateWheel(backLeft, backRightTransform);
        UpdateWheel(backRight, backLeftTransform);

        if (transform.position.y < -150)
        {
            transform.gameObject.SetActive(false);
        }
    }

    void UpdateTruckWrap()
    {
        if (forward && chunkLoader.GetFarthestForwardZ() - offset <= transform.position.z)
        {
            transform.position = new Vector3(transform.position.x, 2.1f, chunkLoader.GetFarthestBackwardZ() - offset);
        }
        else if (!forward && chunkLoader.GetFarthestBackwardZ() + offset >= transform.position.z)
        {
            transform.position = new Vector3(transform.position.x, 2.1f, chunkLoader.GetFarthestForwardZ() + offset);
        }
    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
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
    void UpdateTruckPosition(float reference)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - reference);
    }
}
