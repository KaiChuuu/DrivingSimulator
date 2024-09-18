using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;

    public float speed = 500f;

    public float minX = -10f;
    public float maxX = 25f;
    public float minY = -50f;
    public float maxY = 45f;

    private Vector3 currentRotation;

    //Other interactive components
    [SerializeField] Collider leftMirrorCollider;
    [SerializeField] Collider rightMirrorCollider;
    [SerializeField] Collider middleMirrorCollider;

    [SerializeField] Collider turnOnOffButton;
    [SerializeField] Collider prevSongButton;
    [SerializeField] Collider nextSongButton;
    [SerializeField] Collider instructionBook;
    [SerializeField] Collider plasticCup;
    [SerializeField] Collider restartButton;
    [SerializeField] Collider trackButton;

    private int cameraColliderMask;
    private HashSet<Collider> draggable = new HashSet<Collider>();

    private bool isDragging = false;
    private bool isMoving = false;
    private Collider currentInteraction;

    void Start()
    {
        currentRotation = transform.eulerAngles;

        cameraColliderMask = 1 << 6; // Layer 6

        draggable.Add(leftMirrorCollider);
        draggable.Add(rightMirrorCollider);
        draggable.Add(middleMirrorCollider);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Detecting interactable component
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!isMoving && Physics.Raycast(ray, out hit, Mathf.Infinity, cameraColliderMask))
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
                    UpdateClickableContent();
                }
            }
            else if (!isDragging)
            {
                isMoving = true;
                UpdateCameraDirection();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isMoving = false;
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

        float relativeRotationY = Mathf.DeltaAngle(player.transform.rotation.eulerAngles.y, currentRotation.y);

        currentRotation.x = Mathf.Clamp(currentRotation.x, minX, maxX);
        currentRotation.y = Mathf.Clamp(relativeRotationY, minY, maxY) + player.transform.rotation.eulerAngles.y;

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
        else if (currentInteraction == middleMirrorCollider)
        {
            middleMirrorCollider.GetComponentInParent<MirrorController>().UpdateMirrorRotation();
        }
    }

    void UpdateClickableContent()
    {
        if(currentInteraction == turnOnOffButton)
        {
            turnOnOffButton.GetComponentInParent<DashboardController>().UpdatePanelState();
        }
        else if (currentInteraction == prevSongButton)
        {
            prevSongButton.GetComponentInParent<DashboardController>().UpdateSong("prev");
        }
        else if(currentInteraction == nextSongButton)
        {
            nextSongButton.GetComponentInParent<DashboardController>().UpdateSong("next");
        }
        else if(currentInteraction == instructionBook)
        {
            instructionBook.GetComponent<InstructionBook>().ReadInstructions();
        }
        else if(currentInteraction == plasticCup)
        {
            plasticCup.GetComponent<PlasticCup>().Drink();
        }
        else if(currentInteraction == restartButton)
        {
            restartButton.GetComponent<RestartGame>().Restart();
        }
        else if(currentInteraction == trackButton)
        {
            trackButton.GetComponentInParent<DashboardController>().UpdateTrack();
        }
    }
}