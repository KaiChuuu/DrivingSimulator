using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float speed = 500f;

    public float minX = -10f;
    public float maxX = 25f;
    public float minY = -50f;
    public float maxY = 45f;

    private Vector3 currentRotation;

    //Other interactive components
    [SerializeField] Collider leftMirrorCollider;
    [SerializeField] Collider rightMirrorCollider;

    private int cameraColliderMask;
    private HashSet<Collider> draggable = new HashSet<Collider>();

    private bool isDragging = false;
    private Collider currentInteraction;

    void Start()
    {
        currentRotation = transform.eulerAngles;

        cameraColliderMask = 1 << 6; // Layer 6

        draggable.Add(leftMirrorCollider);
        draggable.Add(rightMirrorCollider);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Detecting interactable component
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cameraColliderMask))
            {
                currentInteraction = hit.collider;

                if (draggable.Contains(currentInteraction))
                {
                    //Draggable content
                    isDragging = true;
                }
                else
                {
                    //Point and click content

                }
            }
            else if (!isDragging)
            {
                UpdateCameraDirection();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            UpdateDraggableContent();
        }
    }

    void UpdateCameraDirection()
    {
        // Not hitting any collider
        float mouseX = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;

        currentRotation.y += mouseX;
        currentRotation.x -= mouseY;

        currentRotation.x = Mathf.Clamp(currentRotation.x, minX, maxX);
        currentRotation.y = Mathf.Clamp(currentRotation.y, minY, maxY);

        transform.eulerAngles = currentRotation;
    }

    void UpdateDraggableContent()
    {
        if (currentInteraction == leftMirrorCollider)
        {
            leftMirrorCollider.GetComponentInParent<MirrorController>().UpdateMirrorRotation();
        }
        else if (currentInteraction == rightMirrorCollider)
        {
            rightMirrorCollider.GetComponentInParent<MirrorController>().UpdateMirrorRotation();
        }
    }
}