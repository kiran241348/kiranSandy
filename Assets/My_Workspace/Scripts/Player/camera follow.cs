using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float mouseSensitivity = 200f;

    public Vector3 offset = new Vector3(0f, 2f, -4f);

    float xRotation = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player left/right
        player.Rotate(Vector3.up * mouseX);

        // Camera vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Follow player with offset
        transform.position = player.position + player.TransformDirection(offset);
    }
}