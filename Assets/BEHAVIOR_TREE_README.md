# Behavior Tree System cho Enemy

## Giới thiệu

Behavior Tree là một hệ thống AI dựa trên cây quyết định, cho phép tạo các hành vi phức tạp và dễ bảo trì cho enemies trong game.

## Cấu trúc Hệ Thống

### 1. **BehaviorTreeNode** (Node cơ bản)
- Base class cho tất cả các nodes
- Status: `Success`, `Failure`, `Running`
- Methods:
  - `Execute()`: Thực thi node
  - `Initialize(GameObject agent)`: Khởi tạo
  - `OnEnter()`, `OnExit()`: Được gọi khi bắt đầu/kết thúc
  - `Reset()`: Reset trạng thái

### 2. **Composite Nodes** (Nút kết hợp)

#### **SequenceNode**
- Thực thi children theo thứ tự
- Trả về `Failure` nếu bất kỳ child nào fail
- Trả về `Success` nếu tất cả children success
- Thường dùng cho: "Làm A, sau đó Làm B, sau đó Làm C"

```csharp
// Ví dụ: Phát hiện -> Xung đến -> Tấn công
SequenceNode attackSequence = new SequenceNode();
attackSequence.AddChild(detectTask);
attackSequence.AddChild(moveTask);
attackSequence.AddChild(attackTask);
```

#### **SelectorNode**
- Thực thi children theo thứ tự
- Trả về `Success` nếu bất kỳ child nào success
- Trả về `Failure` nếu tất cả children fail
- Thường dùng cho: "Thử A, nếu fail thì thử B, nếu fail thì thử C"

```csharp
// Ví dụ: Tấn công hoặc Xung đến hoặc Tuần tra
SelectorNode root = new SelectorNode();
root.AddChild(attackTask);
root.AddChild(chaseSequence);
root.AddChild(patrolTask);
```

#### **ParallelNode**
- Thực thi tất cả children cùng lúc
- Có thể cấu hình success/failure policy

### 3. **Decorator Nodes** (Nút trang trí)

#### **InverterDecoratorNode**
- Đảo ngược kết quả: Success ↔ Failure

#### **RepeatDecoratorNode**
- Lặp lại child cho đến N lần

#### **CooldownDecoratorNode**
- Thêm cooldown timer cho child

#### **TimeoutDecoratorNode**
- Timeout nếu child chạy quá lâu

#### **SucceederDecoratorNode**
- Luôn trả về Success

#### **FailerDecoratorNode**
- Luôn trả về Failure

### 4. **Task Nodes** (Nút tác vụ - Leaf nodes)

#### **DetectPlayerTask**
- Phát hiện người chơi trong range
- Trả về `Success` nếu phát hiện được

#### **MoveToPlayerTask**
- Di chuyển về phía người chơi
- Trả về `Success` khi tới gần player
- Trả về `Running` khi đang di chuyển

#### **AttackPlayerTask**
- Tấn công người chơi
- Trả về `Success` nếu attack
- Có cooldown tư động

#### **PatrolTask**
- Tuần tra các điểm cho trước
- Tự động tạo patrol points nếu chưa có

#### **FleeTask**
- Chạy trốn khỏi player
- Trả về `Success` khi đã chạy đủ xa

#### **WaitTask**
- Chờ đợi một khoảng thời gian
- Trả về `Success` khi hết thời gian

## Các Loại Behavior Tree

### 1. **Patrol** ✓
Chỉ tuần tra không chiến đấu
```
Root (Selector)
└── Patrol
```

### 2. **Combat** ✓
Chiến đấu khi phát hiện
```
Root (Selector)
├── Attack
├── Sequence
│   ├── DetectPlayer
│   └── MoveToPlayer
└── Patrol
```

### 3. **Aggressive** ✓
Tích cực tấn công, tăng range phát hiện
```
Root (Selector)
├── Attack (range To tây)
├── Sequence
│   ├── DetectPlayer (range lớn)
│   └── MoveToPlayer (nhanh)
└── (không Patrol)
```

### 4. **Defensive** ✓
Phòng thủ, chạy trốn khi HP thấp
```
Root (Selector)
├── Sequence (HP < 30%)
│   ├── FleeCondition
│   └── Flee
├── Attack
└── Chase
```

### 5. **Balanced** ✓
Cân bằng giữa nhiều hành vi
```
Root (Selector)
├── Attack
├── Chase
└── Patrol (với Wait)
```

## Cách Sử Dụng

### Trên Enemy GameObject

1. **Thêm EnemyBehaviorTree Component:**
   - Chọn Enemy GameObject
   - Add Component > EnemyBehaviorTree

2. **Cấu Hình:**
   ```
   - Max Health: 100
   - Tree Type: Combat (hoặc loại khác)
   ```

3. **Component sẽ tự động:**
   - Tạo Behavior Tree khi Start
   - Update tree mỗi frame
   - Xử lý damage và death

### Tạo Behavior Tree Tùy Chỉnh

```csharp
public void BuildCustomTree()
{
    // Root: Selector - thử các hành vi theo thứ tự
    SelectorNode root = gameObject.AddComponent<SelectorNode>();

    // Branch 1: Nếu trong range tấn công
    AttackPlayerTask attack = gameObject.AddComponent<AttackPlayerTask>();
    attack.attackRange = 2f;

    // Branch 2: Nếu phát hiện được, xung đến
    SequenceNode chase = gameObject.AddComponent<SequenceNode>();
    DetectPlayerTask detect = gameObject.AddComponent<DetectPlayerTask>();
    MoveToPlayerTask move = gameObject.AddComponent<MoveToPlayerTask>();

    chase.AddChild(detect);
    chase.AddChild(move);

    // Branch 3: Tuần tra
    PatrolTask patrol = gameObject.AddComponent<PatrolTask>();

    root.AddChild(attack);
    root.AddChild(chase);
    root.AddChild(patrol);

    rootNode = root;
}
```

### Thêm Decorator

```csharp
// Attack với cooldown
CooldownDecoratorNode attackWithCooldown = gameObject.AddComponent<CooldownDecoratorNode>();
attackWithCooldown.cooldownDuration = 2f;

AttackPlayerTask attack = gameObject.AddComponent<AttackPlayerTask>();
attackWithCooldown.SetChild(attack);

root.AddChild(attackWithCooldown);
```

## Ví Dụ Thực Tế

### Enemy "Skeletal Warrior"
```csharp
private void BuildAdvancedTree()
{
    SelectorNode root = gameObject.AddComponent<SelectorNode>();

    // 1. Dodge (nếu HP quá thấp)
    SequenceNode dodge = gameObject.AddComponent<SequenceNode>();
    // Conditions + FleeTask

    // 2. Power Attack (cooldown 5 giây)
    CooldownDecoratorNode powerAttack = gameObject.AddComponent<CooldownDecoratorNode>();
    powerAttack.cooldownDuration = 5f;

    // 3. Normal Attack
    AttackPlayerTask normalAttack = gameObject.AddComponent<AttackPlayerTask>();

    // 4. Chase
    SequenceNode chase = gameObject.AddComponent<SequenceNode>();

    root.AddChild(dodge);
    root.AddChild(powerAttack);
    root.AddChild(normalAttack);
    root.AddChild(chase);
}
```

## Mở Rộng Hệ Thống

### Tạo Task Tùy Chỉnh
```csharp
public class CustomTask : BehaviorTreeNode
{
    public override NodeStatus Execute()
    {
        // Thực thi logic của bạn
        if (/* điều kiện */)
            return NodeStatus.Success;
        else
            return NodeStatus.Failure;
    }
}
```

### Tạo Composite Tùy Chỉnh
```csharp
public class RandomNode : BehaviorTreeNode
{
    private List<BehaviorTreeNode> children;

    public override NodeStatus Execute()
    {
        // Chọn random một child
        int index = Random.Range(0, children.Count);
        return children[index].Execute();
    }
}
```

## Debug

- **Debug Output**: Các log message được in ra Console khi:
  - Enemy phát hiện người chơi
  - Enemy tấn công
  - Enemy chết

- **Gizmo**: Có thể thêm Gizmo để visualize:
  - Detection range (círcle)
  - Attack range
  - Patrol points

## Hiệu Năng

- **Efficient**: Chỉ update các node đang active
- **Scalable**: Dễ thêm nhiều enemies
- **Reusable**: Những loại tree có thể tái sử dụng

## Networking (Future)

Có thể mở rộng để hỗ trợ:
- Multiplayer synchronization
- State serialization
- Save/Load behavior
