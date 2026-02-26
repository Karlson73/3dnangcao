# Hướng Dẫn Setup Cho Unity Project: Dynamic Texture và Item Spawning

## Tổng Quan
Project này bao gồm hai hệ thống chính:
- **TextureChanger.cs**: Thay đổi texture trên terrain bên dưới player ngẫu nhiên khi player di chuyển vào khu vực mới. Texture được reset khi player rời khu vực.
- **ItemSpawner.cs**: Spawn ngẫu nhiên các prefab items xung quanh player khi di chuyển vào khu vực mới. Items được xóa khi player rời khu vực.
- **PlayerController.cs**: Script điều khiển di chuyển player.
- **CameraFollow.cs**: Script làm camera theo dõi player.

Hệ thống sử dụng chunk-based để quản lý texture và items, chỉ giữ lại trong render distance xung quanh player để tối ưu hiệu suất.

## Yêu Cầu Hệ Thống
- Unity 2021.3 hoặc cao hơn.
- Terrain trong scene với nhiều texture layers (splat textures).
- Các prefab items để spawn (ví dụ: trees, rocks, collectibles).

## Cài Đặt Project

### 1. Thiết Lập Scene Cơ Bản
1. Mở Unity và tạo một scene mới hoặc sử dụng scene hiện có.
2. Đảm bảo có Terrain GameObject trong scene.
3. Tạo các prefab items: Tạo GameObject với model, collider, v.v., và lưu thành prefab trong thư mục Assets.

### 2. Cấu Hình Terrain
1. Chọn Terrain trong Hierarchy.
2. Trong Inspector > Terrain > Paint Texture:
   - Thêm ít nhất 2-4 texture layers (splat prototypes). Ví dụ: grass, dirt, rock, sand.
   - TextureChanger sẽ thay đổi giữa các layers này ngẫu nhiên.

### 3. Cài Đặt TextureChanger (Map Generator)

#### Tạo GameObject cho TextureChanger
1. Tạo một GameObject trống (GameObject > Create Empty) và đặt tên là "TextureChanger".
2. Thêm script `TextureChanger.cs` vào GameObject này (Add Component > Scripts > TextureChanger).

#### Cấu Hình Trong Inspector
- **Player**: Kéo thả Transform của Player GameObject vào đây.
- **Terrain**: Kéo thả Terrain GameObject vào đây.
- **Chunk Size**: Kích thước mỗi chunk (mặc định 50f). Điều chỉnh dựa trên scale của scene.
- **Render Distance**: Số chunks giữ xung quanh player (mặc định 3). Tăng để render xa hơn, nhưng ảnh hưởng hiệu suất.
- **Texture Layer Count**: Số texture layers trên terrain (phải khớp với số splat prototypes trong Terrain). Mặc định 4.

#### Cách Hoạt Động
- Khi player di chuyển vào chunk mới, texture trong chunk đó được thay đổi ngẫu nhiên.
- Khi player rời xa chunk (ngoài render distance), texture được reset về layer đầu tiên (mặc định).

### 4. Cài Đặt ItemSpawner (Item Generator)

#### Tạo GameObject cho ItemSpawner
1. Tạo một GameObject trống và đặt tên là "ItemSpawner".
2. Thêm script `ItemSpawner.cs` vào GameObject này.

#### Cấu Hình Trong Inspector
- **Player**: Kéo thả Transform của Player vào đây.
- **Item Prefabs**: Gán mảng các prefab items (ví dụ: tree, rock, coin). Ít nhất 1 prefab.
- **Chunk Size**: Kích thước mỗi chunk (phải khớp với TextureChanger, mặc định 50f).
- **Render Distance**: Số chunks giữ xung quanh player (mặc định 3).
- **Items Per Chunk**: Số items spawn trong mỗi chunk (mặc định 5).
- **Spawn Height**: Chiều cao Y để spawn items (mặc định 0f, trên mặt đất).

#### Cách Hoạt Động
- Khi player di chuyển vào chunk mới, spawn ngẫu nhiên items trong chunk đó.
- Khi player rời xa chunk, tất cả items trong chunk đó được xóa.

### 5. Cài Đặt Player

#### Tạo Player GameObject
1. Tạo GameObject trống (GameObject > Create Empty).
2. Đặt tên là "Player".
3. Thêm component `CharacterController` (Add Component > Physics > Character Controller).
4. Điều chỉnh CharacterController: Radius 0.5, Height 2, Center Y=1.
5. Thêm Capsule Collider nếu cần (Add Component > Physics > Capsule Collider), và set Is Trigger = false.

#### Thêm PlayerController Script
1. Tạo script `PlayerController.cs` nếu chưa có (hoặc sử dụng script hiện có).
2. Thêm vào Player GameObject.
3. Cấu hình input: Sử dụng Input System hoặc legacy input cho di chuyển (WASD hoặc arrow keys).

Ví dụ PlayerController.cs cơ bản:
```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
    }
}
```

### 6. Cài Đặt Camera

#### Thêm CameraFollow Script
1. Chọn Main Camera trong Hierarchy.
2. Thêm script `CameraFollow.cs` vào Camera.
3. Trong Inspector:
   - **Target**: Kéo thả Player Transform vào đây.
   - **Offset**: Vector3 cho vị trí camera tương đối player (ví dụ: 0,5,-10).
   - **Smooth Speed**: Tốc độ mượt (mặc định 5f).

Ví dụ CameraFollow.cs:
```csharp
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            transform.LookAt(target);
        }
    }
}
```

### 7. Chạy và Test
1. Nhấn Play trong Unity.
2. Di chuyển player bằng WASD hoặc arrow keys.
3. Quan sát texture terrain thay đổi ngẫu nhiên dưới player khi vào khu vực mới.
4. Quan sát items spawn xung quanh khi vào khu vực mới, và biến mất khi rời xa.
5. Điều chỉnh parameters nếu cần để phù hợp với scene.

### Lưu Ý Quan Trọng
- **Chunk Size**: Phải giống nhau giữa TextureChanger và ItemSpawner để đồng bộ.
- **Terrain Size**: Đảm bảo terrain đủ lớn để cover các chunks.
- **Performance**: Render Distance cao hơn làm tăng số chunks, ảnh hưởng hiệu suất. Test trên device mục tiêu.
- **Prefabs**: Items prefabs nên có colliders nếu cần tương tác.
- **Reset Texture**: Texture được reset về layer 0 khi rời chunk. Nếu muốn layer khác, sửa code trong ResetTextureInChunk.

### Troubleshooting
- **Texture không thay đổi**: Kiểm tra số Texture Layer Count khớp với splat prototypes trong Terrain.
- **Items không spawn**: Kiểm tra Item Prefabs array không rỗng.
- **Player không di chuyển**: Kiểm tra CharacterController và input axes.
- **Errors**: Kiểm tra Console cho missing references hoặc null references.

Nếu gặp vấn đề, kiểm tra scripts và cấu hình lại theo hướng dẫn.