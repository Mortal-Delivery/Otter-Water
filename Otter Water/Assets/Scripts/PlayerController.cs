using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Referencing the transform
    Transform t;

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

    // Start is called before the first frame update
    void Start()
    {
        t = this.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
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

        // Moving the character (swimming)
        t.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed);
        t.Translate(new Vector3(0, moveY, 0) * Time.deltaTime * speed, Space.World);
    }
}
