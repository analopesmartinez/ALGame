using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [Header("Health")] 
    [SerializeField] private int totalLives = 3;
    [SerializeField] private TMP_Text livesText;

    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepAudio;
    [SerializeField] private float strideInterval = 0.5f;
    [SerializeField] private float sprintStrideInterval = 0.3f;

    [Header("Game Over")]
    [SerializeField] private TMP_Text gameOverText; 

    private Vector2 moveVector;
    private Vector2 lookVector;
    private Vector3 velocity;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float verticalRotation;
    private bool isMoving;
    private bool isSprinting;
    private float timeBetweenSteps;
    private float originalFootstepVolume;
    private bool wasGrounded;
    private float jumpVelocity;
    private bool isJumping;
    private int currentLives;

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
        startPosition = transform.position;
        startRotation = transform.rotation;
        moveVector = Vector3.zero;
        isSprinting = false;
        isJumping = false;
        wasGrounded = true;
        Physics.gravity = new Vector3(0f, -gravityForce, 0f);
        jumpVelocity = Mathf.Sqrt(-2.0f * Physics.gravity.y * jumpHeight);
        moveSpeedInAir *= moveSpeed;
        originalFootstepVolume = audioSource.volume;
        currentLives = totalLives;
        livesText.text = "Lives: " + currentLives;
        gameOverText.text = "";
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

            playFootstepAudio();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Enemy")
        {
            Debug.Log("Enemy hit");
            transform.position = startPosition;
            transform.rotation = startRotation;   
            currentLives--;
            livesText.text = "Lives: " + currentLives;
            if (currentLives == 0)
            {
                StartCoroutine(EndGameFunction());
            }
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
            playFootstepAudio();
        }
        wasGrounded = isGrounded;

        // Check footstep interval and play footstep if exceeded
        float currentStrideInterval = isSprinting ? sprintStrideInterval : strideInterval;
        if(isGrounded && isMoving)
        {
            timeBetweenSteps += Time.deltaTime;

            if(timeBetweenSteps >= currentStrideInterval)
            {
                playFootstepAudio();
                // Reset interval timer
                timeBetweenSteps = 0f;
            }
        } else
        {
            timeBetweenSteps = 0f;
        }
    }

    private void playFootstepAudio()
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
        currentLives = totalLives;
        gameOverText.text = "GAME OVERRRR";
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

