using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour
{
    [Tooltip("Giá trị điểm khi nhặt item này")]
    public int scoreValue = 1;

    [Tooltip("Âm thanh chạy khi item được nhặt (tùy chọn)")]
    public AudioClip pickupClip;

    void Reset()
    {
        // Khi thêm component trong Editor, khuyến khích Collider là trigger
        Collider c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }
}