using System.Collections.Generic;
using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    public Transform player; // Vị trí của player
    public Terrain terrain; // Terrain để thay đổi texture
    public float chunkSize = 50f; // Kích thước chunk
    public int renderDistance = 3; // Số chunk giữ xung quanh player
    public int textureLayerCount = 4; // Số texture layers trên terrain (giả sử có 4)

    private Dictionary<Vector2Int, bool> modifiedChunks = new Dictionary<Vector2Int, bool>();
    private Vector2Int currentChunk;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain not assigned!");
            return;
        }
        // Khởi tạo chunk hiện tại
        currentChunk = GetChunkPosition(player.position);
        UpdateTextures();
    }

    void Update()
    {
        Vector2Int newChunk = GetChunkPosition(player.position);
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            UpdateTextures();
        }
    }

    Vector2Int GetChunkPosition(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.z / chunkSize)
        );
    }

    void UpdateTextures()
    {
        // Thêm chunks mới
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(currentChunk.x + x, currentChunk.y + z);
                if (!modifiedChunks.ContainsKey(chunkPos))
                {
                    ChangeTextureInChunk(chunkPos);
                    modifiedChunks[chunkPos] = true;
                }
            }
        }

        // Xóa chunks xa
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var chunk in modifiedChunks)
        {
            if (Vector2Int.Distance(chunk.Key, currentChunk) > renderDistance)
            {
                ResetTextureInChunk(chunk.Key);
                toRemove.Add(chunk.Key);
            }
        }
        foreach (var chunk in toRemove)
        {
            modifiedChunks.Remove(chunk);
        }
    }

    void ChangeTextureInChunk(Vector2Int chunkPos)
    {
        TerrainData terrainData = terrain.terrainData;
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;

        // Tính vị trí trên alphamap
        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        int startX = Mathf.FloorToInt((chunkPos.x * chunkSize / terrainSizeX) * alphamapWidth);
        int startZ = Mathf.FloorToInt((chunkPos.y * chunkSize / terrainSizeZ) * alphamapHeight);
        int endX = Mathf.FloorToInt(((chunkPos.x + 1) * chunkSize / terrainSizeX) * alphamapWidth);
        int endZ = Mathf.FloorToInt(((chunkPos.y + 1) * chunkSize / terrainSizeZ) * alphamapHeight);

        startX = Mathf.Clamp(startX, 0, alphamapWidth);
        endX = Mathf.Clamp(endX, 0, alphamapWidth);
        startZ = Mathf.Clamp(startZ, 0, alphamapHeight);
        endZ = Mathf.Clamp(endZ, 0, alphamapHeight);

        float[,,] alphamaps = terrainData.GetAlphamaps(startX, startZ, endX - startX, endZ - startZ);

        // Chọn texture ngẫu nhiên
        int randomTexture = Random.Range(0, textureLayerCount);

        for (int x = 0; x < alphamaps.GetLength(0); x++)
        {
            for (int z = 0; z < alphamaps.GetLength(1); z++)
            {
                for (int i = 0; i < textureLayerCount; i++)
                {
                    alphamaps[x, z, i] = (i == randomTexture) ? 1f : 0f;
                }
            }
        }

        terrainData.SetAlphamaps(startX, startZ, alphamaps);
    }

    void ResetTextureInChunk(Vector2Int chunkPos)
    {
        TerrainData terrainData = terrain.terrainData;
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;

        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        int startX = Mathf.FloorToInt((chunkPos.x * chunkSize / terrainSizeX) * alphamapWidth);
        int startZ = Mathf.FloorToInt((chunkPos.y * chunkSize / terrainSizeZ) * alphamapHeight);
        int endX = Mathf.FloorToInt(((chunkPos.x + 1) * chunkSize / terrainSizeX) * alphamapWidth);
        int endZ = Mathf.FloorToInt(((chunkPos.y + 1) * chunkSize / terrainSizeZ) * alphamapHeight);

        startX = Mathf.Clamp(startX, 0, alphamapWidth);
        endX = Mathf.Clamp(endX, 0, alphamapWidth);
        startZ = Mathf.Clamp(startZ, 0, alphamapHeight);
        endZ = Mathf.Clamp(endZ, 0, alphamapHeight);

        float[,,] alphamaps = terrainData.GetAlphamaps(startX, startZ, endX - startX, endZ - startZ);

        // Reset về texture đầu tiên (mặc định)
        for (int x = 0; x < alphamaps.GetLength(0); x++)
        {
            for (int z = 0; z < alphamaps.GetLength(1); z++)
            {
                for (int i = 0; i < textureLayerCount; i++)
                {
                    alphamaps[x, z, i] = (i == 0) ? 1f : 0f;
                }
            }
        }

        terrainData.SetAlphamaps(startX, startZ, alphamaps);
    }
}