using UnityEngine;
using System.Collections;

public class CameraSmoothness : MonoBehaviour
{
    [Header("Smooth Follow")]
    public float positionSmoothSpeed = 0.125f;
    public float rotationSmoothSpeed = 5f;

    [Header("Jump & Landing Effects")]
    [Tooltip("How much the camera lifts during jump (relative to player jump)")]
    [Range(0f, 0.5f)]
    public float jumpLiftAmount = 0.08f;  // Small lift, not full player height

    [Tooltip("Speed of camera lift during jump")]
    public float jumpLiftSpeed = 6f;

    public float landingShakeAmount = 0.05f;  // Reduced for subtle shake
    public float landingShakeDuration = 0.15f;

    [Header("Idle Bobbing")]
    public float idleBobbingAmount = 0.02f;
    public float idleBobbingSpeed = 1.5f;

    [Header("Run Bobbing")]
    public float runBobbingAmount = 0.04f;
    public float runBobbingSpeed = 10f;

    private Vector3 velocityRef = Vector3.zero;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Transform player;
    private CharacterController playerController;
    private PlayerMovement playerMovement;

    private float landingShakeTimer = 0f;
    private float currentBobbingTimer = 0f;
    private bool wasGrounded = true;
    private float jumpStartTime = 0f;
    private bool isJumping = false;
    private float jumpLiftValue = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        if (player == null)
        {
            CameraFollow cameraFollow = GetComponent<CameraFollow>();
            if (cameraFollow != null && cameraFollow.player != null)
                player = cameraFollow.player;
        }

        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Get player movement state
        bool isGrounded = playerController != null ? playerController.isGrounded : true;
        bool isMoving = false;
        bool isRunning = false;

        if (playerMovement != null)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            isMoving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;
            isRunning = isMoving && isGrounded;
        }

        // Handle jump effects (camera only lifts slightly)
        HandleJumpEffects(isGrounded);

        // Handle landing shake
        HandleLandingShake(isGrounded);

        // Handle camera bobbing
        Vector3 bobbingOffset = HandleBobbing(isMoving, isRunning, isGrounded);

        // Apply smooth position with bobbing and jump lift
        Vector3 targetPosition = originalPosition + bobbingOffset + new Vector3(0f, jumpLiftValue, 0f);
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition,
            targetPosition,
            ref velocityRef,
            positionSmoothSpeed
        );

        // Smooth rotation
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            originalRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }

    void HandleJumpEffects(bool isGrounded)
    {
        // Detect landing - reset jump lift
        if (!wasGrounded && isGrounded)
        {
            isJumping = false;
            jumpLiftValue = 0f;
        }

        // Handle jump lift (camera only)
        if (wasGrounded && !isGrounded && playerMovement != null)
        {
            isJumping = true;
            jumpStartTime = Time.time;
            StartCoroutine(JumpLift());
        }

        wasGrounded = isGrounded;
    }

    IEnumerator JumpLift()
    {
        float duration = 0.15f;  // Quick lift up
        float elapsed = 0f;

        // Lift up (small amount)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease out curve for smooth lift
            jumpLiftValue = Mathf.Lerp(0f, jumpLiftAmount, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }

        // Hold for a moment
        yield return new WaitForSeconds(0.1f);

        // Return to normal (only fall slightly)
        elapsed = 0f;
        duration = 0.2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            jumpLiftValue = Mathf.Lerp(jumpLiftAmount, 0f, t);
            yield return null;
        }

        jumpLiftValue = 0f;
    }

    void HandleLandingShake(bool isGrounded)
    {
        if (!wasGrounded && isGrounded && landingShakeTimer <= 0f)
        {
            landingShakeTimer = landingShakeDuration;
            StartCoroutine(LandingShake());
        }

        if (landingShakeTimer > 0)
            landingShakeTimer -= Time.deltaTime;
    }

    IEnumerator LandingShake()
    {
        float elapsed = 0f;
        Vector3 originalPos = transform.localPosition;

        while (elapsed < landingShakeDuration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.Lerp(landingShakeAmount, 0f, elapsed / landingShakeDuration);

            Vector3 shake = new Vector3(
                Random.Range(-intensity, intensity),
                Random.Range(-intensity * 0.3f, intensity * 0.3f),
                0f
            );

            transform.localPosition = originalPos + shake;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    Vector3 HandleBobbing(bool isMoving, bool isRunning, bool isGrounded)
    {
        if (!isGrounded)
        {
            // Very subtle air bobbing (almost none)
            if (isJumping && jumpLiftValue > 0.01f)
            {
                // Small vibration during jump
                float microBob = Mathf.Sin(Time.time * 20f) * 0.002f;
                return new Vector3(0f, microBob, 0f);
            }
            return Vector3.zero;
        }

        if (isMoving)
        {
            // Running bobbing
            currentBobbingTimer += Time.deltaTime * (isRunning ? runBobbingSpeed : runBobbingSpeed * 0.7f);
            float bobAmount = isRunning ? runBobbingAmount : runBobbingAmount * 0.7f;

            float verticalBob = Mathf.Sin(currentBobbingTimer * 2f) * bobAmount;
            float horizontalBob = Mathf.Cos(currentBobbingTimer) * (bobAmount * 0.3f);

            return new Vector3(horizontalBob, verticalBob, 0f);
        }
        else
        {
            // Idle bobbing
            currentBobbingTimer += Time.deltaTime * idleBobbingSpeed;
            float idleBob = Mathf.Sin(currentBobbingTimer) * idleBobbingAmount;
            return new Vector3(0f, idleBob, 0f);
        }
    }
}