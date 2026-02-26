using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manager để quản lý và load bundles từ nhiều version khác nhau
/// FIXED: Unload TẤT CẢ bundles cũ trước khi load bundle mới
/// </summary>
public class AssetBundleVersionManager : MonoBehaviour
{
    [Header("Available Versions")]
    [Tooltip("Danh sách các version có thể chọn")]
    public List<string> availableVersions = new List<string> { "v1.0", "v2.0", "beta" };
    
    [Header("Current Selection")]
    public string selectedVersion = "v1.0";
    public string bundleName = "player";
    public string assetName = "Capsule";
    
    [Header("Settings")]
    public bool autoDetectVersions = true;
    public bool instantiateOnLoad = true;
    public Vector3 spawnPosition = Vector3.zero;
    
    [Header("Version Switch Behavior")]
    [Tooltip("Tự động xóa object cũ khi đổi version")]
    public bool destroyOldInstanceOnSwitch = true;
    [Tooltip("Xóa tất cả instances cũ (nếu có nhiều)")]
    public bool destroyAllOldInstances = true;

    private Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();
    private GameObject currentPrefab;
    private GameObject currentInstance; // Track instance hiện tại
    private List<GameObject> allInstances = new List<GameObject>(); // Track tất cả instances

    void Start()
    {
        if (autoDetectVersions)
        {
            DetectAvailableVersions();
        }

        // Auto load version đầu tiên
        if (!string.IsNullOrEmpty(selectedVersion))
        {
            StartCoroutine(LoadBundleFromVersion(selectedVersion));
        }
    }

    /// <summary>
    /// Tự động phát hiện các version có sẵn trong StreamingAssets
    /// </summary>
    public void DetectAvailableVersions()
    {
        availableVersions.Clear();
        string bundlesPath = Application.streamingAssetsPath + "/Bundles";

        if (!Directory.Exists(bundlesPath))
        {
            Debug.LogWarning("⚠️ Không tìm thấy folder Bundles");
            return;
        }

        string[] directories = Directory.GetDirectories(bundlesPath);
        
        foreach (string dir in directories)
        {
            string versionName = Path.GetFileName(dir);
            availableVersions.Add(versionName);
        }

        Debug.Log($"✅ Phát hiện {availableVersions.Count} version(s): {string.Join(", ", availableVersions)}");
    }

    /// <summary>
    /// Load bundle từ version cụ thể
    /// </summary>
    public IEnumerator LoadBundleFromVersion(string version)
    {
        selectedVersion = version;
        string path = GetBundlePath(version, bundleName);

        Debug.Log($"🔍 Loading bundle: {bundleName} from version: {version}");
        Debug.Log($"📂 Path: {path}");

        // Kiểm tra file tồn tại
        if (!File.Exists(path))
        {
            Debug.LogError($"❌ File không tồn tại: {path}");
            yield break;
        }

        // ========== FIX: XÓA OBJECT CŨ TRƯỚC KHI LOAD MỚI ==========
        if (destroyOldInstanceOnSwitch)
        {
            if (destroyAllOldInstances)
            {
                // Xóa tất cả instances
                DestroyAllInstances();
            }
            else
            {
                // Chỉ xóa instance hiện tại
                DestroyCurrentInstance();
            }
        }
        // ===========================================================

        // ========== FIX: UNLOAD TẤT CẢ BUNDLES CŨ ==========
        // Vấn đề: Unity không cho load 2 bundles có cùng assets
        // Giải pháp: Unload TẤT CẢ bundles trước khi load bundle mới
        
        UnloadAllBundles();
        
        // ===================================================

        // Load bundle mới
        AssetBundle bundle = AssetBundle.LoadFromFile(path);

        if (bundle == null)
        {
            Debug.LogError($"❌ Load bundle thất bại");
            yield break;
        }

        loadedBundles[version] = bundle;
        Debug.Log($"✅ Bundle loaded: {bundleName}");

        // Load asset
        GameObject prefab = bundle.LoadAsset<GameObject>(assetName);
        
        if (prefab == null)
        {
            Debug.LogError($"❌ Asset '{assetName}' không tồn tại trong bundle");
            yield break;
        }

        currentPrefab = prefab;
        Debug.Log($"✅ Asset loaded: {assetName}");

        // Spawn nếu cần
        if (instantiateOnLoad)
        {
            GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
            instance.name = $"{assetName}_v{version}";
            
            // Track instance mới
            currentInstance = instance;
            allInstances.Add(instance);
            
            Debug.Log($"✅ Spawned: {instance.name}");
        }
    }

    /// <summary>
    /// Chuyển sang version khác
    /// </summary>
    public void SwitchToVersion(string version)
    {
        if (!availableVersions.Contains(version))
        {
            Debug.LogWarning($"⚠️ Version '{version}' không có trong danh sách");
            return;
        }

        StartCoroutine(LoadBundleFromVersion(version));
    }

    /// <summary>
    /// Load version kế tiếp
    /// </summary>
    public void LoadNextVersion()
    {
        int currentIndex = availableVersions.IndexOf(selectedVersion);
        int nextIndex = (currentIndex + 1) % availableVersions.Count;
        string nextVersion = availableVersions[nextIndex];
        
        SwitchToVersion(nextVersion);
    }

    /// <summary>
    /// Load version trước đó
    /// </summary>
    public void LoadPreviousVersion()
    {
        int currentIndex = availableVersions.IndexOf(selectedVersion);
        int prevIndex = (currentIndex - 1 + availableVersions.Count) % availableVersions.Count;
        string prevVersion = availableVersions[prevIndex];
        
        SwitchToVersion(prevVersion);
    }

    /// <summary>
    /// Spawn prefab hiện tại tại vị trí mới
    /// </summary>
    public GameObject SpawnAtPosition(Vector3 position)
    {
        if (currentPrefab != null)
        {
            GameObject instance = Instantiate(currentPrefab, position, Quaternion.identity);
            instance.name = $"{assetName}_v{selectedVersion}_{Time.time}";
            allInstances.Add(instance);
            return instance;
        }
        
        Debug.LogWarning("⚠️ Chưa load prefab nào");
        return null;
    }

    // ========== BUNDLE MANAGEMENT ==========

    /// <summary>
    /// Unload TẤT CẢ bundles đã load
    /// </summary>
    private void UnloadAllBundles()
    {
        if (loadedBundles.Count == 0)
            return;

        Debug.Log($"🗑️ Unloading {loadedBundles.Count} bundle(s)...");

        foreach (var kvp in loadedBundles)
        {
            if (kvp.Value != null)
            {
                kvp.Value.Unload(true); // true = unload assets luôn
                Debug.Log($"   ✅ Unloaded: {kvp.Key}");
            }
        }
        
        loadedBundles.Clear();
        currentPrefab = null; // Clear reference
    }

    /// <summary>
    /// Unload bundle của một version cụ thể
    /// </summary>
    public void UnloadVersion(string version)
    {
        if (loadedBundles.ContainsKey(version))
        {
            loadedBundles[version].Unload(true);
            loadedBundles.Remove(version);
            Debug.Log($"🗑️ Unloaded bundle: {version}");
        }
    }

    // ========== XÓA OBJECT CŨ ==========

    /// <summary>
    /// Xóa instance hiện tại
    /// </summary>
    private void DestroyCurrentInstance()
    {
        if (currentInstance != null)
        {
            Debug.Log($"🗑️ Destroying old instance: {currentInstance.name}");
            Destroy(currentInstance);
            currentInstance = null;
        }
    }

    /// <summary>
    /// Xóa tất cả instances đã spawn
    /// </summary>
    private void DestroyAllInstances()
    {
        int count = 0;
        foreach (GameObject instance in allInstances)
        {
            if (instance != null)
            {
                Destroy(instance);
                count++;
            }
        }
        
        if (count > 0)
        {
            Debug.Log($"🗑️ Destroyed {count} old instance(s)");
        }
        
        allInstances.Clear();
        currentInstance = null;
    }

    /// <summary>
    /// Xóa tất cả instances (public method để gọi từ UI)
    /// </summary>
    public void ClearAllInstances()
    {
        DestroyAllInstances();
    }

    // ====================================

    /// <summary>
    /// Lấy đường dẫn đầy đủ đến bundle
    /// </summary>
    string GetBundlePath(string version, string bundle)
    {
        return Path.Combine(
            Application.streamingAssetsPath,
            "Bundles",
            version,
            bundle
        );
    }

    /// <summary>
    /// Cleanup khi destroy
    /// </summary>
    void OnDestroy()
    {
        UnloadAllBundles();
    }

    // ============ HELPER METHODS CHO UI ============

    /// <summary>
    /// Lấy danh sách tên các version (dùng cho Dropdown)
    /// </summary>
    public List<string> GetVersionNames()
    {
        return new List<string>(availableVersions);
    }

    /// <summary>
    /// Lấy index của version hiện tại (dùng cho Dropdown)
    /// </summary>
    public int GetCurrentVersionIndex()
    {
        return availableVersions.IndexOf(selectedVersion);
    }

    /// <summary>
    /// Set version từ index (gọi từ Dropdown.onValueChanged)
    /// </summary>
    public void SetVersionByIndex(int index)
    {
        if (index >= 0 && index < availableVersions.Count)
        {
            SwitchToVersion(availableVersions[index]);
        }
    }

    /// <summary>
    /// Lấy số lượng instances hiện có
    /// </summary>
    public int GetInstanceCount()
    {
        // Cleanup null references
        allInstances.RemoveAll(x => x == null);
        return allInstances.Count;
    }
}