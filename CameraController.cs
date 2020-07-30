using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Follow other in-game objects during runtime when selected
    public static CameraController instance;
    public Transform followTransform;

    public Transform cameraTransform;

    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime = 10f;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    //HandleMouseInput variables
    public Transform player;

    public float rotationSpeed = 5f;
    public float screenEdgeBorderThickness = 5f;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    public float minZoomY = 50f;
    public float maxZoomY = 200f;
    public float minZoomZ = -200f;
    public float maxZoomZ = -50f;

    public bool followPlayerLocked = false;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //Start camera rig at player position
        transform.position = player.position;

        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera follows selected object
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
        else
        {
            if (player != null)
            {
                //HandleMovementInput();
                HandleMouseInput();
            }
        }

        //Clear the selected object the camera is following
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    private void HandleMouseInput()
    {
        //Lock camera to player
        if (Input.GetKeyDown(KeyCode.L) && !followPlayerLocked)
        {
            followPlayerLocked = true;
        }
        else if (Input.GetKeyDown(KeyCode.L) && followPlayerLocked)
        {
            followPlayerLocked = false;
        }

        //Speed of camera rig movement
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        //Camera shifts to player when held down
        if (Input.GetKey(KeyCode.Space) || followPlayerLocked)
        {
            newPosition = player.position;
        }

        //Mouse wheel zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
            newZoom.y = Mathf.Clamp(newZoom.y, minZoomY, maxZoomY);
            newZoom.z = Mathf.Clamp(newZoom.z, minZoomZ, maxZoomZ);
        }

        //Mouse button click and drag camera rotation
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / rotationSpeed));
        }

        //Mouse position screen boundary camera movement
        if (Input.mousePosition.y >= Screen.height - screenEdgeBorderThickness &&
            !Input.GetKey(KeyCode.Space) &&
            !followPlayerLocked)
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.mousePosition.y <= screenEdgeBorderThickness &&
            !Input.GetKey(KeyCode.Space) &&
            !followPlayerLocked)
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.mousePosition.x >= Screen.width - screenEdgeBorderThickness &&
            !Input.GetKey(KeyCode.Space) &&
            !followPlayerLocked)
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.mousePosition.x <= screenEdgeBorderThickness &&
            !Input.GetKey(KeyCode.Space) &&
            !followPlayerLocked)
        {
            newPosition += (transform.right * -movementSpeed);
        }

        //Set and smooth movement, rotation, and zoom
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
