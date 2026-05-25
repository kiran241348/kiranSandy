using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;
    public Animator animator;

    [Header("Movement")]
    public float runSpeed = 8f;
    public float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool hasJumped; // Tracks if player has jumped and hasn't landed yet

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        hasJumped = false; // Start on ground, hasn't jumped
    }

    void Update()
    {
        // ---------------- BUILT-IN GROUND CHECK ----------------
        isGrounded = controller.isGrounded;

        // Reset jump flag when touching ground
        if (isGrounded && velocity.y <= 0)
        {
            if (hasJumped)
            {
                hasJumped = false; // Allow jumping again
                animator.SetBool("IsJumping", false);
            }

            // Reset velocity when grounded
            if (velocity.y < 0)
                velocity.y = -1f;
        }

        // ---------------- INPUT ----------------
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // ---------------- CAMERA RELATIVE MOVEMENT ----------------
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * z + right * x).normalized;

        // ---------------- MOVE PLAYER ----------------
        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * runSpeed * Time.deltaTime);

            // Rotate toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // ---------------- JUMP (CAN ONLY JUMP IF HASN'T JUMPED YET) ----------------
        if (Input.GetButtonDown("Jump") && isGrounded && !hasJumped)
        {
            hasJumped = true; // Lock jump until landing
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping", true);
        }

        // ---------------- APPLY GRAVITY ----------------
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ---------------- ANIMATIONS ----------------
        UpdateAnimator(x, z);
    }

    void UpdateAnimator(float x, float z)
    {
        bool isMoving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;

        animator.SetBool("IsRunning", isMoving);
        animator.SetBool("IsGrounded", isGrounded);
    }
}