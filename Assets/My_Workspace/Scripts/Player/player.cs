using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;
    public Animator animator;

    [Header("Movement")]
    public float runSpeed = 8f;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumping = false;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // GROUND CHECK
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
            }
        }

        // GET INPUT
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // MOVEMENT DIRECTION (Camera Relative)
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * z + right * x).normalized;

        // MOVE PLAYER (without auto-rotation)
        if (moveDirection.magnitude >= 0.1f && !isJumping)
        {
            controller.Move(moveDirection * runSpeed * Time.deltaTime);

            // ONLY rotate if you want character to face movement direction
            // Comment this out if you don't want auto-rotation
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // JUMP
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            isJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping", true);
        }

        // APPLY GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // UPDATE ANIMATOR
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f);

        bool isRunning = isMoving && isGrounded && !isJumping;
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}