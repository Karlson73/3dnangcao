using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform cameraTransform;

    void Update()
    {
        Vector3 lookDirection = cameraTransform.forward;
        lookDirection.y = 0f; // giữ nhân vật không ngửa lên/xuống
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

}