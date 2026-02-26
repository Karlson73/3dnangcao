# Hướng Dẫn Setup: Sound, Score & Pickup 🔊💯🎒

Tài liệu này hướng dẫn chi tiết cách cài đặt và kiểm tra ba hệ thống: **Audio (âm lượng)**, **Score (điểm)** và **Pickup (nhặt item)** dùng các script `AudioOnlySettings`, `ScoreManager`, và `PlayerItemCollector`.

---

## Nội dung chính
- Thiết lập UI âm lượng (Slider + %)
- Thiết lập ScoreManager và hiển thị điểm (TMP Text)
- Thiết lập PlayerItemCollector để nhặt item, cộng điểm và xóa item
- (Tùy chọn) Thêm âm thanh khi nhặt item
- Cách kiểm tra và troubleshooting

---

## 1) Sound — AudioOnlySettings 🔊
**Mục đích:** Quản lý master volume (global) qua UI Slider và lưu vào `PlayerPrefs`.

### Cài đặt
1. Tạo UI:
   - Canvas → thêm `Slider` (UI > Slider)
   - Thêm `Text (TMP)` để hiển thị phần trăm (ví dụ: "85%")
2. Tạo GameObject `SettingsUI` và gán component `AudioOnlySettings`.
3. Kéo `volumeSlider` và `volumePercentText` vào các trường tương ứng trong Inspector.
4. Tạo Button Save và gán sự kiện OnClick → `AudioOnlySettings.SaveSettings()`.

**PlayerPrefs key:** `MASTER_VOLUME` (float, 0..1, mặc định 1)

**Lưu ý:** Script chỉ thay `AudioListener.volume`. Đảm bảo có `AudioListener` trong scene (thường ở `Main Camera`).

---

## 2) Score — ScoreManager 💯
**Mục đích:** Quản lý và hiển thị điểm, tự động lưu/ load bằng `PlayerPrefs`.

### Cài đặt
1. Tạo GameObject `GameManager` và thêm `ScoreManager`.
2. Trên Canvas, tạo `Text (TMP)` và kéo vào `scoreText` trong Inspector.
3. (Tuỳ chọn) Tạo Button Reset và gán OnClick → `ScoreManager.ResetScore()`.

**PlayerPrefs key:** `PLAYER_SCORE` (int, mặc định 0)

**Sử dụng từ code:** gọi `FindObjectOfType<ScoreManager>().AddScore(1);` để cộng điểm (hoặc gán reference để tránh Find runtime cost).

---

## 3) Pickup — PlayerItemCollector 🎒
**Mục đích:** Nhận trigger từ item (tag xác định), cộng điểm qua `ScoreManager`, và destroy item.

### Cài đặt
1. Gắn `PlayerItemCollector` vào Player GameObject.
2. Thiết lập `itemTag` (mặc định `Item`) và `scorePerItem`.
3. Item prefab:
   - Gán Tag = `Item` (hoặc tag đã set)
   - Thêm `Collider` và tick **Is Trigger = true**
   - (Tùy chọn) Thêm `Rigidbody` với `isKinematic = true` nếu không cần vật lý
4. Đảm bảo Player có `Collider` và `Rigidbody` hoặc `CharacterController`.

### Ghi chú vận hành
- `OnTriggerEnter(Collider other)` kiểm tra `other.CompareTag(itemTag)` → `ScoreManager.AddScore(scorePerItem)` → `Destroy(other.gameObject)`.
- Nếu không tìm thấy `ScoreManager` sẽ log lỗi; bạn có thể gán thủ công reference trong PlayerItemCollector để chắc chắn.

---

## 4) Thêm âm thanh pick-up (tùy chọn) 🔔
Muốn thêm hiệu ứng âm thanh khi nhặt item, mở rộng `PlayerItemCollector` như sau:

```csharp
public AudioSource audioSource; // kéo vào Inspector
public AudioClip pickupClip;

void OnTriggerEnter(Collider other)
{
    if (other.CompareTag(itemTag))
    {
        if (scoreManager != null) scoreManager.AddScore(scorePerItem);
        if (pickupClip != null && audioSource != null) audioSource.PlayOneShot(pickupClip);
        Destroy(other.gameObject);
    }
}
```

**Setup:** Thêm `AudioSource` vào Player, bỏ tick `Play On Awake`, kéo `audioSource` và `pickupClip` vào Inspector.

---

## 5) Kiểm tra (Testing) ▶️
1. Play scene.
2. Di chuyển Player chạm item:
   - Item bị xóa
   - Điểm tăng trên UI (Score cập nhật)
   - Console show log lưu điểm (ScoreManager)
   - Nghe sound nếu đã thêm
3. Chỉnh slider âm lượng và nhấn Save, stop & play lại để kiểm tra giá trị lưu.

---

## 6) Troubleshooting ⚠️
- Items không được nhặt: kiểm tra `Tag`, `Collider.IsTrigger = true`, và Player có `Collider`/`Rigidbody`/`CharacterController`.
- Score không cập nhật: kiểm tra `scoreText` đã gán và `ScoreManager` tồn tại.
- Volume không thay đổi/không lưu: kiểm tra `AudioOnlySettings` bindings và Button Save.
- Không có tiếng pick-up: kiểm tra `AudioSource`, `AudioClip`, và `AudioListener.volume` (không phải 0).

---

## 7) Tips & Best-Practices 💡
- Tránh gọi `FindObjectOfType` trong Update; gán reference qua Inspector hoặc tạo Singleton cho `ScoreManager` nếu cần.
- Lưu ý performance khi spawn items nhiều; cân nhắc pooling nếu số lượng lớn.
- Dùng `PlayerPrefs` cho dữ liệu nhẹ; dùng file hoặc server nếu cần lưu nhiều hơn và an toàn hơn.

---

Nếu muốn, tôi có thể:
- Tạo một **sample item prefab** và **sample UI Slider** trong project, hoặc
- Sửa `PlayerItemCollector` để tích hợp sẵn âm thanh pick-up và đảm bảo null-checks.

Chọn 1 trong 2 để tôi thực hiện bước tiếp theo.