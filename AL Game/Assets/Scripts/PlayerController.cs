using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField][Range(0f, 1f)] private float moveSpeed = 0.1f;
    [SerializeField][Range(1f, 5f)] private float sprintSpeed = 2f;
    [SerializeField][Range(0f, 1f)] private float moveSpeedInAir = 0.25f;

    [Header("Look")]
    [SerializeField][Range(0f, 2f)] private float lookSensitivity = 0.5f;
    [SerializeField][Range(0f, 90f)] private float verticalRotationCap = 80f;

    [Header("Jump")]
    [SerializeField][Range(0f, 1f)] float jumpHeight = 0.5f;
    [SerializeField][Range(0f, 2f)] float gravityFactor = 0.5f;

    private Vector2 moveVector;
    private Vector2 lookVector;
    private Vector3 velocity;
    private float verticalRotation;
    private bool isSprinting;

    private CharacterController characterController;
    private Camera playerCamera;

    private void Awake()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    private void Start()
    {
        // Lock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Initialising variables
        moveVector = Vector3.zero;
        isSprinting = false;
        Physics.gravity = new Vector3(0f, Physics.gravity.y * gravityFactor, 0f);
        moveSpeedInAir *= moveSpeed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        Look();
    }

    private void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>();
        if (moveVector.x < 0.5f) isSprinting = false;
    }

    private void OnLook(InputValue value)
    {
        lookVector = value.Get<Vector2>();
    }

    private void OnSprint()
    {
        isSprinting = true;
    }

    private void OnJump()
    {
        if (characterController.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2.0f * Physics.gravity.y * jumpHeight);
            velocity.y = jumpVelocity;
        }
    }

    private void Move()
    {
        Vector3 velocityResult;
        if (characterController.isGrounded)
        {
            // Movement input conversion
            velocity = new Vector3(moveVector.x, velocity.y, moveVector.y);
            // Sprint multiplier
            velocity.x *= moveSpeed * (isSprinting ? sprintSpeed : 1f);
            velocity.z *= moveSpeed * (isSprinting ? sprintSpeed : 1f);
            //Slight gravitational force applied to keep player grounded
            //velocity.y = -0.02f;
            // Move relative to the direction the player is facing
            velocityResult = velocity = transform.rotation * velocity;
        }
        else
        {
            // Move speed whilst in the air
            Vector3 airVelocity = new Vector3(moveVector.x, 0f, moveVector.y) * moveSpeedInAir;
            airVelocity = transform.rotation * airVelocity;
            // Apply gravity
            velocity += Physics.gravity * Time.deltaTime;
            velocityResult = velocity + airVelocity;
        }
        Debug.Log("Result: " + velocityResult);
        // Apply movement to character controller
        characterController.Move(velocityResult);
    }

    private void Look()
    {
        // Horizontal rotation
        transform.Rotate(0f, lookVector.x * lookSensitivity, 0f);
        // Vertical rotation
        verticalRotation -= lookVector.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationCap, verticalRotationCap);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}

