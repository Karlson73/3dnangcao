# EnemySequenceAI — Hướng dẫn sử dụng ✅

## Mục đích
Đây là script AI đơn giản theo chuỗi bước (sequence): **Detect → Move → Attack**. Dùng cho enemy cơ bản trong Unity.

## Các trường public (Inspector)
- `Transform player` — tham chiếu tới đối tượng người chơi.
- `float moveSpeed` — tốc độ di chuyển khi tiếp cận người chơi.
- `float attackRange` — khoảng cách để bắt đầu tấn công.
- `float attackCooldown` — thời gian chờ giữa các lần tấn công.

## Hành vi (tóm tắt)
1. Detect: nếu `player` != `null` sẽ chuyển sang bước tiếp theo.
2. Move: di chuyển trực tiếp về phía `player` cho đến khi khoảng cách ≤ `attackRange`.
3. Attack: khi đủ cooldown, gọi `AttackPlayer()` (hiện tại chỉ log ở Console) rồi quay lại bước Detect.

## Cách cài đặt nhanh
1. Gắn component `EnemySequenceAI` vào GameObject enemy.
2. Kéo Transform của player vào trường `player` trong Inspector.
3. Điều chỉnh `moveSpeed`, `attackRange`, `attackCooldown` cho phù hợp.

## Ví dụ nâng cấp (gợi ý) 💡
- Thay `DetectPlayer()` bằng kiểm tra khoảng cách hoặc `Physics.OverlapSphere` để chỉ detect khi player gần:

```csharp
bool DetectPlayer() {
    float detectRange = 10f;
    return player != null && Vector3.Distance(transform.position, player.position) <= detectRange;
}
```

- Dùng `NavMeshAgent` để di chuyển mượt và tránh vật cản.
- Thay `AttackPlayer()` bằng gọi hàm làm giảm máu trên component `Health` của player:

```csharp
void AttackPlayer() {
    var health = player.GetComponent<Health>();
    if (health != null) health.TakeDamage(10);
    // play animation or VFX
}
```

- Thêm `Animator` để đồng bộ animation (walk/attack/idle).
- Thêm kiểm tra line-of-sight (raycast) nếu cần AI thông minh hơn.

## Debug & Troubleshooting ⚠️
- Nếu enemy không di chuyển: kiểm tra `player` đã được gán không.
- Nếu enemy xuyên vật thể: tăng kiểm tra va chạm hoặc dùng `NavMeshAgent`.
- Nếu attack không xảy ra: kiểm tra `attackRange` và `attackCooldown`.

## Gợi ý mở rộng (cho production) 🔧
- Dùng state machine hoặc `Unity.VisualScripting`/`Animator` để quản lý trạng thái phức tạp.
- Tách ra interface `IDamageable` cho target để tấn công an toàn.
- Thêm event (C# event/UnityEvent) để phát tín hiệu khi tấn công hoặc phát hiện.

---
Nếu bạn muốn, tôi có thể: thêm ví dụ mã sửa đổi trực tiếp vào `EnemySequenceAI.cs`, hoặc tạo script `Health` mẫu và ví dụ scene để kiểm thử. Chọn một trong hai để tôi tiếp tục. ✨