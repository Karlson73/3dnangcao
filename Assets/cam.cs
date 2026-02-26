using UnityEngine;

public class AutoMoveCameraZ : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }
}