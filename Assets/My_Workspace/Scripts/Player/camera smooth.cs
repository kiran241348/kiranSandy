using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    [Header("Smooth Settings")]
    public float smoothTime = 0.3f;
    public float rotationSmoothTime = 0.2f;

    private Vector3 velocityRef = Vector3.zero;
    private float angularVelocityRef = 0f;
    private Transform target;
    private CameraFollow mainCameraFollow;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        mainCameraFollow = GetComponent<CameraFollow>();
        if (mainCameraFollow != null && mainCameraFollow.player != null)
            target = mainCameraFollow.player;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate where camera SHOULD be according to CameraFollow
        CameraFollow cf = mainCameraFollow;
        if (cf == null) return;

        // Get mouse input to calculate target position
        float mouseX = Input.GetAxis("Mouse X") * cf.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cf.mouseSensitivity * Time.deltaTime;

        float currentYaw = cf.GetType().GetField("currentYaw", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cf) as float? ?? 0f;
        float currentPitch = cf.GetType().GetField("currentPitch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(cf) as float? ?? 0f;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, -30f, 80f);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 desiredPosition = target.position - rotation * Vector3.forward * cf.distance + Vector3.up * cf.height;

        // Smoothly move camera
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocityRef,
            smoothTime
        );

        // Smoothly rotate to look at target
        Quaternion desiredRotation = Quaternion.LookRotation(target.position + Vector3.up * cf.height - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothTime / Time.deltaTime
        );
    }
}