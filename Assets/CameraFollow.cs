using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Player transform
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Khoảng cách camera so với player
    public float smoothSpeed = 1f; // Tốc độ mượt mà của camera - tăng lên để follow nhanh hơn
    public float mouseSensitivity = 100f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Target is null! Please assign the player transform.");
            return;
        }

        // Xử lý input chuột để xoay camera
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        Vector3 rotatedOffset = rotation * offset;

        // Tính vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + rotatedOffset;

        // Làm mượt vị trí camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Cập nhật vị trí camera
        transform.position = smoothedPosition;

        // Cập nhật xoay camera
        transform.rotation = rotation;

        // Debug: In vị trí để kiểm tra
        Debug.Log("Camera position: " + transform.position + ", Target position: " + target.position);
    }
}