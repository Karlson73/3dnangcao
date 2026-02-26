using System.Collections;
using UnityEngine;
using System.IO;

public class LoadAssetBundle : MonoBehaviour
{
    [Header("Bundle Settings")]
    public string bundleName = "player";
    public string assetName = "Capsule";
    
    [Header("Version Settings")]
    [Tooltip("Để trống sẽ load từ Bundles/ trực tiếp")]
    public string versionFolder = "v1.0"; // Ví dụ: "v1.0", "v2.0", "beta", etc.
    
    [Header("Load Options")]
    public bool useVersionFolder = true;
    public bool instantiateOnLoad = true;
    public Vector3 spawnPosition = Vector3.zero;
    
    private GameObject loadedPrefab;

    IEnumerator Start()
    {
        yield return StartCoroutine(LoadBundle());
    }

    IEnumerator LoadBundle()
    {
        // Xây dựng đường dẫn dựa trên version
        string path = GetBundlePath();
        
        Debug.Log($"🔍 Đang load bundle từ: {path}");

        // Kiểm tra file có tồn tại không
        if (!File.Exists(path))
        {
            Debug.LogError($"❌ Không tìm thấy bundle tại: {path}");
            yield break;
        }

        // Load bundle
        AssetBundle bundle = AssetBundle.LoadFromFile(path);

        if (bundle == null)
        {
            Debug.LogError($"❌ Load bundle thất bại: {bundleName}");
            yield break;
        }

        Debug.Log($"✅ Load bundle thành công: {bundleName}");

        // Load asset từ bundle
        GameObject prefab = bundle.LoadAsset<GameObject>(assetName);
        
        if (prefab == null)
        {
            Debug.LogError($"❌ Không tìm thấy asset '{assetName}' trong bundle");
            bundle.Unload(true);
            yield break;
        }

        Debug.Log($"✅ Load asset thành công: {assetName}");

        // Lưu prefab để dùng sau
        loadedPrefab = prefab;

        // Instantiate nếu cần
        if (instantiateOnLoad)
        {
            GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
            Debug.Log($"✅ Đã spawn: {instance.name}");
        }

        // Unload bundle (giữ lại assets đã load)
        bundle.Unload(false);
    }

    /// <summary>
    /// Xây dựng đường dẫn đến bundle dựa trên version
    /// </summary>
    string GetBundlePath()
    {
        string basePath = Application.streamingAssetsPath + "/Bundles";
        
        if (useVersionFolder && !string.IsNullOrEmpty(versionFolder))
        {
            // Đường dẫn có version: StreamingAssets/Bundles/v1.0/player
            return Path.Combine(basePath, versionFolder, bundleName);
        }
        else
        {
            // Đường dẫn thông thường: StreamingAssets/Bundles/player
            return Path.Combine(basePath, bundleName);
        }
    }

    /// <summary>
    /// Load bundle từ version khác lúc runtime
    /// </summary>
    public void LoadFromVersion(string version)
    {
        versionFolder = version;
        useVersionFolder = true;
        StartCoroutine(LoadBundle());
    }

    /// <summary>
    /// Spawn thêm instance của prefab đã load
    /// </summary>
    public GameObject SpawnLoadedPrefab(Vector3 position)
    {
        if (loadedPrefab != null)
        {
            return Instantiate(loadedPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa load prefab nào!");
            return null;
        }
    }
}