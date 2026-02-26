using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public Transform player; // Vị trí của player
    public GameObject[] itemPrefabs; // Danh sách prefab items để spawn
    public float chunkSize = 50f; // Kích thước chunk
    public int renderDistance = 3; // Số chunk giữ xung quanh player
    public int itemsPerChunk = 5; // Số items spawn trong mỗi chunk
    public float spawnHeight = 0f; // Chiều cao spawn items

    private Dictionary<Vector2Int, List<GameObject>> spawnedItems = new Dictionary<Vector2Int, List<GameObject>>();
    private Vector2Int currentChunk;

    void Start()
    {
        if (itemPrefabs.Length == 0)
        {
            Debug.LogError("No item prefabs assigned!");
            return;
        }
        // Khởi tạo chunk hiện tại
        currentChunk = GetChunkPosition(player.position);
        UpdateItems();
    }

    void Update()
    {
        Vector2Int newChunk = GetChunkPosition(player.position);
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            UpdateItems();
        }
    }

    Vector2Int GetChunkPosition(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.z / chunkSize)
        );
    }

    void UpdateItems()
    {
        // Thêm chunks mới
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(currentChunk.x + x, currentChunk.y + z);
                if (!spawnedItems.ContainsKey(chunkPos))
                {
                    SpawnItemsInChunk(chunkPos);
                }
            }
        }

        // Xóa items trong chunks xa
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var chunk in spawnedItems)
        {
            if (Vector2Int.Distance(chunk.Key, currentChunk) > renderDistance)
            {
                foreach (var item in chunk.Value)
                {
                    Destroy(item);
                }
                toRemove.Add(chunk.Key);
            }
        }
        foreach (var chunk in toRemove)
        {
            spawnedItems.Remove(chunk);
        }
    }

    void SpawnItemsInChunk(Vector2Int chunkPos)
    {
        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < itemsPerChunk; i++)
        {
            // Chọn prefab ngẫu nhiên
            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            // Vị trí ngẫu nhiên trong chunk
            float x = Random.Range(chunkPos.x * chunkSize, (chunkPos.x + 1) * chunkSize);
            float z = Random.Range(chunkPos.y * chunkSize, (chunkPos.y + 1) * chunkSize);
            Vector3 position = new Vector3(x, spawnHeight, z);

            // Spawn item
            GameObject item = Instantiate(prefab, position, Quaternion.identity);
            items.Add(item);
        }
        spawnedItems[chunkPos] = items;
    }
}