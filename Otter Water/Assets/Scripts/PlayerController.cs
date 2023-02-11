using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Referencing the transform
    Transform t;

    //if not in water, normal walking.
    //In water but not swimming to float
    //In water and swimming... swim.
    public static bool inWater;
    public static bool isSwimming;

    public LayerMask waterMask;

    [Header("Player Rotation")]
    public float sensitivity;
    // Mouse input variables
    float rotationX;
    float rotationY;
    // Clamp Camera Rotation
    public float rotationMin;
    public float rotationMax;

    [Header("Player Movement")]
    public float speed = 1;
    float moveX;
    float moveY;
    float moveZ;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = this.transform;

        Cursor.lockState = CursorLockMode.Locked;

        inWater = false;
    }

    private void FixedUpdate()
    {
        SwimmingOrFloating();
        Move();
    }

    // When the player hits a collider, this happens.
    private void OnTriggerEnter(Collider other)
    {
        SwitchMovement();
    }

    // When the player leaves the entered collider, this happens.
    private void OnTriggerExit(Collider other)
    {
        SwitchMovement();
    }

    void SwitchMovement()
    {
        //toggles inWater
        inWater = !inWater;

        rb.useGravity = !rb.useGravity;
    }

    void SwimmingOrFloating()
    {
        bool swimCheck = false;

        if(inWater)
        {
            RaycastHit hit;
            if(Physics.Raycast(new Vector3(t.position.x, t.position.y + 0.5f, t.position.z), Vector3.down, out hit, Mathf.Infinity, waterMask))
            {
                if(hit.distance < 0.1f)
                {
                    swimCheck = true;
                }
            } else {
                swimCheck = true;
            }
        }

        isSwimming = swimCheck;
    }

    // Update is called once per frame
    void Update()
    {
        LookAround();
    }

    void LookAround()
    {
        // Get Mouse Inputs
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;

        // Clamp Camera Rotation
        rotationY = Mathf.Clamp(rotationY, rotationMin, rotationMax);

        // Rotating every update
        t.localRotation = Quaternion.Euler(-rotationY, rotationX, 0);
    }

    void Move()
    {
        // Movement Input
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Forward");

        if (!inWater)
        {
            // Moving the character (Land)
            t.Translate(new Quaternion(0, t.rotation.y, 0, t.rotation.w) * new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed, Space.World);
        }
        else 
        {
            // Check if swimming or floating.
            if(!isSwimming)
            {
                // Moving the player (floating)
                // Clamp moveY so player can't use space or shift to move up.
                moveY = Mathf.Min(moveY, 0);

                // Converting local vector to worldspace vector.
                Vector3 clampedDirection = t.TransformDirection(new Vector3(moveX, moveY, moveZ));

                // Clamp the worldspace vector.
                clampedDirection = new Vector3(clampedDirection.x, Mathf.Min(clampedDirection.y, 0), clampedDirection.z);

                t.Translate(clampedDirection * Time.deltaTime * speed, Space.World);
            }
            else 
            {
                // Moving the character (swimming)
                t.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed);
                t.Translate(new Vector3(0, moveY, 0) * Time.deltaTime * speed, Space.World);
            }
        }
    }
}
