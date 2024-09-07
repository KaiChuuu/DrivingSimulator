using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float minRotationAngle = -10f; 
    public float maxRotationAngle = 10f;

    [SerializeField] Camera mirrorCamera;

    private float currentRotation;

    void Start()
    {
        currentRotation = transform.localEulerAngles.y;
    }

    public void UpdateMirrorRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float newRotation = currentRotation + mouseX * rotationSpeed;

        newRotation = Mathf.Clamp(newRotation, minRotationAngle, maxRotationAngle);

        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, -newRotation, transform.localEulerAngles.z);
        currentRotation = newRotation;
    }
}
