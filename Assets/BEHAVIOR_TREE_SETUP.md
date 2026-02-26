# Behavior Tree Setup Guide - Hướng Dẫn Cài Đặt

## 📋 Yêu Cầu Tiên Quyết

- Unity 2019.4+
- Có Enemy GameObject trong scene
- Có PlayerController component

---

## 🔧 Bước 1: Chuẩn Bị Enemy GameObject

### 1.1 Tạo Enemy GameObject
```
Tạo Empty GameObject → Đặt tên "Enemy"
```

### 1.2 Thêm Components Cần Thiết

#### **Physics Setup** (Nếu cần di chuyển vật lý)
1. **Add Component** → **Rigidbody**
   - Mass: 1
   - Drag: 0.3
   - Angular Drag: 0.05
   - Freeze Rotation X, Y, Z: ✓ (tránh xoay vô tình)
   - Use Gravity: ✓

2. **Add Component** → **Collider**
   - Capsule Collider hoặc Box Collider
   - Is Trigger: ✗ (phải là False để vật lý hoạt động)

#### **Render Setup**
1. **Add Mesh Filter**
   - Chọn mesh (hoặc tạm thời dùng cube)

2. **Add Mesh Renderer**
   - Assign Material

#### **Audio Setup** (Tuỳ chọn)
1. **Add Component** → **Audio Source**
   - Để trống, sẽ dùng khi có attack sound

---

## 🎛 Bước 2: Thêm EnemyBehaviorTree Component

### 2.1 Add Component
```
Enemy GameObject → Add Component → EnemyBehaviorTree
```

### 2.2 Cấu Hình trong Inspector

```
┌─ Enemy Behavior Tree (Script)
│
├─ Behavior Tree Type
│  └─ Combat ▼ (chọn: Patrol, Combat, Aggressive, Defensive, Balanced)
│
├─ Enemy Stats
│  ├─ Max Health: 100
│  └─ Current Health: 100 (auto-fill)
```

### 2.3 Các Loại Tree Type

| Type | Mô Tả | Khi Nào Dùng |
|------|-------|-------------|
| **Patrol** | Tuần tra, không chiến đấu | Enemy non-aggressive |
| **Combat** | Chiến đấu khi phát hiện | Enemy thường |
| **Aggressive** | Tấn công tích cực | Boss, enemy quái |
| **Defensive** | Chạy trốn khi HP thấp | Enemy thông minh |
| **Balanced** | Cân bằng tất cả | Enemy linh hoạt |

---

## 🎯 Bước 3: Cấu Hình Từng Task

### 3.1 DetectPlayerTask
```
Detection Range: 20 (đơn vị)
Layer: "Player" (phải setup layer trước)
```

### 3.2 MoveToPlayerTask
```
Move Speed: 3 (đơn vị/giây)
Stopping Distance: 1.5 (tên cách player)
```

### 3.3 AttackPlayerTask
```
Attack Range: 2 (phải gần hơn mới tấn công)
Attack Damage: 10
Attack Cooldown: 2 (giây)
```

### 3.4 PatrolTask
```
Move Speed: 2
Waypoint Tolerance: 0.5 (độ chính xác đến waypoint)

Patrol Points:
- Tự động tạo 4 điểm quanh enemy
- Hoặc assign thủ công trong inspector
```

### 3.5 FleeTask
```
Flee Distance: 10 (chạy nếu player gần hơn)
Move Speed: 4 (chạy nhanh hơn walk)
```

---

## 📐 Bước 4: Setup Layers

### 4.1 Tạo Layer "Player"
```
Edit → Project Settings → Tags and Layers
→ Add Layer "Player"
```

### 4.2 Assign Layer cho Player
```
Player GameObject → Inspector → Layer → Player
```

### 4.3 Assign Layer cho Enemy
```
Enemy GameObject → Inspector → Layer → Enemy (hoặc tạo)
```

---

## 🎮 Bước 5: Test Scene Setup

### 5.1 Scene Structure
```
Scene
├── Player (có PlayerController)
│   └── Model (Mesh)
│
├── Enemy (có EnemyBehaviorTree)
│   ├── Rigidbody
│   ├── Collider
│   ├── Mesh Filter
│   ├── Mesh Renderer
│   └── EnemyBehaviorTree Script
│
├── Camera Main
│   └── (Game Camera)
│
└── Terrain (tuỳ chọn)
```

### 5.2 Cấu Hình Player
```
Player GameObject:
├── Transform: Position (0, 1, 0)
├── Mesh (Capsule)
├── Collider
└── PlayerController Script
   └── Move Speed: 5
   └── Input: WASD hoặc Arrow Keys
```

### 5.3 Cấu Hình Enemy
```
Enemy GameObject:
├── Transform: Position (5, 1, 0) [cách Player]
├── Mesh (Cube hoặc ModelPrefab)
├── Rigidbody
│   └── Freeze Rotation: ✓
├── Collider
└── EnemyBehaviorTree Script
   ├── Tree Type: Combat
   ├── Max Health: 100
   └── Tất cả tasks tự động config
```

---

## 🚀 Bước 6: Chạy và Test

### 6.1 Play Game
```
Press Play Button (Ctrl+P hoặc Play)
```

### 6.2 Expected Behavior (Combat Tree)
```
1. Enemy stốt hoặc tuần tra (no player detected)
2. Bạn chuyển động vào range nhìn thấy
3. Enemy phát hiện bạn → log "Phát hiện người chơi"
4. Enemy xung đến bạn
5. Khi gần hơn 2 unit → Enemy tấn công → log "Tấn công người chơi"
6. Attack cooldown 2 giây rồi lặp lại
```

### 6.3 Theo Dõi in Console
```
Console Messages:
- "Phát hiện người chơi" → Detection works
- "Tấn công người chơi với sát thương X" → Attack works
- "Enemy chết!" → Death works
```

---

## ⚙️ Bước 7: Cấu Hình Nâng Cao

### 7.1 Thêm Multiple Enemies
```
Duplicate Enemy GameObject
Adjust Position (không overlap)
Each có riêng EnemyBehaviorTree
```

### 7.2 Customize cho Enemy Type

#### **Aggressive Enemy**
```
Tree Type: Aggressive
Detection Range: 30 (xa hơn)
Move Speed: 4.5 (nhanh hơn)
Attack Damage: 15 (mạnh hơn)
```

#### **Defensive Enemy**
```
Tree Type: Defensive
Detection Range: 15
Move Speed: 3
Health Threshold: 30% (HP dưới 30% thì chạy)
Flee Speed: 5
```

#### **Patrol Guard**
```
Tree Type: Patrol
Move Speed: 2 (chậm, bình tĩnh)
Patrol Points: Setup manual ở 4 góc area
```

### 7.3 Thêm Health Bar UI (Tuỳ Chọn)

```csharp
// Thêm Canvas + Image cho health bar
// Cập nhật trong Enemy script:

void UpdateHealthBar()
{
    float healthPercent = GetHealthPercent();
    healthBarUI.fillAmount = healthPercent;
    healthBarText.text = $"{currentHealth:F0}/{maxHealth:F0}";
}
```

### 7.4 Thêm Attack Effects (Tuỳ Chọn)

Trong `AttackPlayerTask.cs`:
```csharp
private void PerformAttack()
{
    Debug.Log($"Enemy tấn công!");
    
    // Phát âm thanh
    GetComponent<AudioSource>().PlayOneShot(attackSound);
    
    // Phát hiệu ứng
    Instantiate(attackEffect, transform.position, Quaternion.identity);
    
    // Gây sát thương
    player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
}
```

---

## 🐛 Debug & Troubleshooting

### Vấn Đề: Enemy không phát hiện Player

**Nguyên nhân:**
- Layer không match
- Detection Range quá nhỏ
- Player không có Collider

**Giải Pháp:**
```csharp
1. Kiểm tra Layer "Player" đã gán chưa
2. Aumentar Detection Range (thử 50)
3. Log trong DetectPlayerTask:
   Debug.Log($"Distance: {distance}, Range: {detectionRange}");
```

### Vấn Đề: Enemy không di chuyển

**Nguyên nhân:**
- Velocity bị freeze
- Collider bị trigger mode
- Move Speed = 0

**Giải Pháp:**
```
1. Rigidbody → Constraints: Freeze Rotation (chỉ X, Y, Z)
2. Collider → Is Trigger: ✗ (bỏ check)
3. MoveToPlayerTask → moveSpeed > 0
4. Log position: Debug.Log(transform.position);
```

### Vấn Đề: Enemy không tấn công

**Nguyên nhân:**
- Attack Range quá ngắn
- Attack Cooldown chưa hết
- Player không trong range

**Giải Pháp:**
```
1. AttackPlayerTask → attackRange: 3
2. Giỏi timeout cooldown
3. Log distance:
   Debug.Log($"Distance to player: {distance}, Range: {attackRange}");
```

### Vấn Đề: Enemy di chuyển lạ (xoay tròn, nhảy cóng)

**Nguyên nhân:**
- LookAt gây xung đột
- Velocity quá cao
- Collider không khớp Model

**Giải Pháp:**
```csharp
// Comment LookAt tạm thời:
// agentGameObject.transform.LookAt(player.position);

// Hoặc dùng smooth rotation:
Vector3 direction = (player.position - transform.position).normalized;
transform.rotation = Quaternion.Lerp(
    transform.rotation,
    Quaternion.LookRotation(direction),
    Time.deltaTime * 5f
);
```

---

## 📊 Performance Optimization

### Bật Gizmos để Visualize
```csharp
// Thêm vào BehaviorTreeTasks.cs:

void OnDrawGizmos()
{
    // Draw Detection Range
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, detectionRange);
    
    // Draw Attack Range
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
```

### Giảm Check Frequency
```csharp
// Không check mỗi frame, kiểm tra mỗi 0.2s:
private float checkInterval = 0.2f;
private float lastCheckTime;

public override NodeStatus Execute()
{
    if (Time.time - lastCheckTime < checkInterval)
        return NodeStatus.Running;
    
    lastCheckTime = Time.time;
    // ... actual logic
}
```

---

## 📦 Cấu Trúc File Cuối Cùng

```
Assets/
├── BehaviorTreeNode.cs         (Base class)
├── BehaviorTreeComposites.cs   (Selector, Sequence)
├── BehaviorTreeDecorators.cs   (Cooldown, Timeout, etc)
├── BehaviorTreeTasks.cs        (Detect, Move, Attack, Patrol)
├── EnemyBehaviorTree.cs        (Manager & Tree Builder)
├── BEHAVIOR_TREE_README.md     (Overview)
├── BEHAVIOR_TREE_SETUP.md      (This file)
│
├── Scenes/
│   └── TestScene.unity          (Test scene)
│
├── Prefabs/
│   ├── Enemy_Combat.prefab
│   ├── Enemy_Aggressive.prefab
│   ├── Enemy_Defensive.prefab
│   └── Enemy_Patrol.prefab
│
└── Scripts/
    └── PlayerController.cs
```

---

## ✅ Checklist Setup Hoàn Chỉnh

- [ ] Tạo Enemy GameObject
- [ ] Thêm Rigidbody (Freeze Rotation)
- [ ] Thêm Collider (không Trigger)
- [ ] Thêm Mesh & Material
- [ ] Tạo Layer "Player"
- [ ] Assign Layer cho Player
- [ ] Add EnemyBehaviorTree Component
- [ ] Chọn Tree Type
- [ ] Setup Camera
- [ ] Test Scene
- [ ] Play và kiểm tra behavior
- [ ] Xem Console logs
- [ ] Debug nếu cần

---

## 🎓 Tiếp Theo

1. **Tạo Prefabs:** File → New Prefab, drag Enemy vào
2. **Tạo Multiple Enemies:** Duplicate prefab nhiều lần
3. **UI Health Bar:** Thêm Canvas + Health Bar display
4. **Sound Effects:** Thêm audio cho Attack/Death
5. **Animations:** Blend behavior tree với animation controller
6. **Boss AI:** Kết hợp nhiều trees cho boss complex

---

**Chúc bạn thành công! 🚀**
