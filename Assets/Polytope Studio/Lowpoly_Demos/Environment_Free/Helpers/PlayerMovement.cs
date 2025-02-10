using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform; // Reference to the camera

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f; // To control vertical rotation (up/down)

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center
    }

    void Update()
    {
        // Handle mouse look
        HandleMouseLook();

        // Handle player movement
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // Get the mouse X and Y inputs for looking around
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera up and down (vertical axis)
        xRotation -= mouseY; // Increase or decrease the vertical angle based on the mouse movement
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit up/down angle to prevent over-rotation

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Apply vertical rotation to the camera

        // Rotate the player left and right (horizontal axis)
        transform.Rotate(Vector3.up * mouseX); // Apply horizontal rotation to the player's body
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        // Get movement inputs (WASD or Arrow keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Create a movement vector based on input
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Move the character
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Handle jumping (only if the player is grounded)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Jump based on the physics formula
        }

        // Apply gravity and move character
        characterController.Move(velocity * Time.deltaTime);
    }
}


