using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 100f;
    public float distance = 5f;
    public float height = 2f;

    private float currentYaw = 0f;      // Horizontal rotation
    private float currentPitch = 0f;    // Vertical rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // Mouse input for both horizontal and vertical rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, -30f, 80f);

        // Calculate camera rotation
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);

        // Camera position
        Vector3 targetPosition = player.position
            - rotation * Vector3.forward * distance
            + Vector3.up * height;

        transform.position = targetPosition;

        // Look at player
        transform.LookAt(player.position + Vector3.up * height);
    }
}