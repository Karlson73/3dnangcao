using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    [Header("Settings")]
    public string itemTag = "Item";
    public int scorePerItem = 1;

    [Header("Audio (optional)")]
    public AudioSource audioSource;
    public AudioClip pickupClip;

    private ScoreManager scoreManager;

    void Start()
    {
        // Tìm ScoreManager
        scoreManager = FindFirstObjectByType<ScoreManager>();

        if (scoreManager == null)
        {
            // Fallback to singleton
            if (ScoreManager.Instance != null)
            {
                scoreManager = ScoreManager.Instance;
            }
        }

        if (scoreManager == null)
        {
            Debug.LogError("❌ Không tìm thấy ScoreManager trong scene! Hãy tạo GameObject với ScoreManager script.");
        }
        else
        {
            Debug.Log("✅ Đã kết nối với ScoreManager");
        }

        // Kiểm tra Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("⚠️ Player không có Rigidbody!");
        }

        // Kiểm tra Collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("⚠️ Player không có Collider!");
        }

        // Auto-assign AudioSource nếu chưa gán
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("⚠️ Không có AudioSource trên Player. Nếu muốn âm thanh pick-up, thêm AudioSource và gán vào Inspector.");
            }
            else
            {
                Debug.Log("✅ AudioSource tự động gán cho PlayerItemCollector");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔍 Trigger detected: {other.name} (Tag: {other.tag})");

        if (!other.CompareTag(itemTag))
        {
            Debug.Log($"⚠️ Tag không khớp. Cần '{itemTag}' nhưng nhận '{other.tag}'");
            return;
        }

        Debug.Log($"✅ Picked item: {other.name}");

        // Try read PickupItem component for per-item settings
        PickupItem item = other.GetComponent<PickupItem>();
        int gain = (item != null) ? item.scoreValue : scorePerItem;
        AudioClip clip = (item != null) ? item.pickupClip : pickupClip;

        if (scoreManager != null)
        {
            scoreManager.AddScore(gain);
        }
        else
        {
            // Try singleton fallback
            if (ScoreManager.Instance != null)
            {
                ScoreManager.AddToScore(gain);
            }
            else
            {
                Debug.LogError("❌ ScoreManager = null, không thể cộng điểm!");
            }
        }

        // Play pickup sound on player's audioSource if available
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }

        Destroy(other.gameObject);
    }

    // Thêm debug cho collision thường (nếu trigger không hoạt động)
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"💥 Collision (không phải trigger) với: {collision.gameObject.name}");
        Debug.Log("   → Kiểm tra: Ít nhất một Collider phải có Is Trigger = ON");
    }
}