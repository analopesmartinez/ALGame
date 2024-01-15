using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField][Range(0f, 1f)] private float jumpHeight = 0.5f;
    [SerializeField][Range(0f, 20f)] private float gravityForce = 4.9f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepAudio;
    [SerializeField] private float strideInterval = 0.5f;
    [SerializeField] private float sprintStrideInterval = 0.3f;

    [Header("Game Over")]
    [SerializeField] private TMP_Text canvasText;
    [SerializeField] private GameObject blackoutSquare;
    [SerializeField] private float fadeSpeed;

    private Vector2 moveVector;
    private Vector2 lookVector;
    private Vector3 velocity;
    private float verticalRotation;
    private bool isMoving;
    private bool isSprinting;
    private float timeBetweenSteps;
    private float originalFootstepVolume;
    private bool wasGrounded;
    private float jumpVelocity;
    private bool isJumping;

    private CharacterController characterController;
    private Camera playerCamera;
    private AudioSource audioSource;

    private void Awake()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Lock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Initialising variables
        moveVector = Vector3.zero;
        isSprinting = false;
        isJumping = false;
        wasGrounded = true;
        Physics.gravity = new Vector3(0f, -gravityForce, 0f);
        jumpVelocity = Mathf.Sqrt(-2.0f * Physics.gravity.y * jumpHeight);
        moveSpeedInAir *= moveSpeed;
        originalFootstepVolume = audioSource.volume;
        if (canvasText == null) canvasText = FindObjectOfType<TMP_Text>();
        if(canvasText.text != "YOU ESCAPED") canvasText.text = "";
        if (blackoutSquare == null) blackoutSquare = GameObject.FindGameObjectsWithTag("Blackout")[0];
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        Look();
        FootstepAudio();
    }

    private void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>().normalized;
        if (moveVector.y < 0.5f) isSprinting = false;
    }

    private void OnLook(InputValue value)
    {
        lookVector = value.Get<Vector2>();
    }

    private void OnSprint()
    {
        if (moveVector.y > 0f)
        {
            isSprinting = true;
        }
    }

    private void OnJump()
    {
        if (characterController.isGrounded)
        {
            isJumping = true;

            PlayFootstepAudio();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log(hit.gameObject.tag);
        if(hit.gameObject.tag == "Enemy")
        {
            Debug.Log("Collided with enemy");
            StartCoroutine(EndGameFunction());
        }
    }

    private void Move()
    {
        Vector3 velocityResult;
        if (characterController.isGrounded)
        {
            float yVelocity = isJumping ? jumpVelocity : -0.1f; 
            // Movement input conversion
            velocity = new Vector3(moveVector.x, yVelocity, moveVector.y);
            // Sprint multiplier
            velocity.x *= moveSpeed * (isSprinting ? sprintSpeed : 1f);
            velocity.z *= moveSpeed * (isSprinting ? sprintSpeed : 1f);
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
            // Ensure velocity magnitude doesnt increase
            if(velocityResult.magnitude > velocity.magnitude)
            {
                velocityResult = velocity.magnitude * velocityResult.normalized;
            }
            // Reset isJumping
            isJumping = false;
        }

        // Apply movement to character controller
        characterController.Move(velocityResult);

        isMoving = characterController.velocity.magnitude > 0;
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

    private void FootstepAudio()
    {
        // Play footstep when landing from a jump
        bool isGrounded = characterController.isGrounded;
        if (!wasGrounded && isGrounded)
        {
            PlayFootstepAudio();
        }
        wasGrounded = isGrounded;

        // Check footstep interval and play footstep if exceeded
        float currentStrideInterval = isSprinting ? sprintStrideInterval : strideInterval;
        if(isGrounded && isMoving)
        {
            timeBetweenSteps += Time.deltaTime;

            if(timeBetweenSteps >= currentStrideInterval)
            {
                PlayFootstepAudio();
                // Reset interval timer
                timeBetweenSteps = 0f;
            }
        } else
        {
            timeBetweenSteps = 0f;
        }
    }

    private void PlayFootstepAudio()
    {
        // Pick random footstep audio clip
        int randomIndex = Random.Range(0, footstepAudio.Length - 1);
        audioSource.clip = footstepAudio[randomIndex];
        audioSource.volume = originalFootstepVolume * velocity.magnitude / 0.3f;
        audioSource.Play();
        //Debug.Log("velocity magnitude: " + velocity.magnitude + " || move speed: " + moveSpeed + " || sprint speed: " + sprintSpeed);
    }

    private IEnumerator EndGameFunction()
    {
        canvasText.text = "GoT yOu";
        Color objectColour = blackoutSquare.GetComponent<Image>().color;
        float fadeAmount;
        while(blackoutSquare.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColour.a + (fadeSpeed * Time.deltaTime);

            objectColour = new Color(objectColour.r, objectColour.g, objectColour.g, fadeAmount);
            blackoutSquare.GetComponent<Image>().color = objectColour;
            yield return null;
        }
        SceneManager.LoadScene("Level 0");
    }
}

