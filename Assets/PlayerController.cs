using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển
    public float rotationSpeed = 100f; // Tốc độ xoay
    public Transform cameraTransform; // Tham chiếu đến camera
    public AudioSource audioSource; // Tham chiếu đến AudioSource để phát âm thanh

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("PlayerController requires a CharacterController component!");
        }
        // Không cần loop nữa vì phát mỗi lần nhấn
    }

    void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("PlayerController: Camera Transform is null! Please assign the camera.");
            return;
        }

        // Nhận input từ bàn phím
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Debug.Log("Input Horizontal: " + moveHorizontal + ", Vertical: " + moveVertical);

        // Tính vector di chuyển dựa trên hướng camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; // Giữ di chuyển trên mặt phẳng ngang
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * moveVertical + right * moveHorizontal) * moveSpeed * Time.deltaTime;

        Debug.Log("Movement vector: " + movement);

        // Áp dụng di chuyển
        controller.Move(movement);

        // Xoay player theo hướng di chuyển (tùy chọn)
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Phát âm thanh khi nhấn giữ J
        if (Input.GetKey(KeyCode.J))
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}