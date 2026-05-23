using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float mouseSensitivity = 200f;

    float xRotation = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player left/right
        player.Rotate(Vector3.up * mouseX);

        // Camera up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Follow player position
        transform.position = player.position;
    }
}