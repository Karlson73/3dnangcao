using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UI Controller để chọn version bundle
/// IMPROVED: Thêm nút Clear All và hiển thị số lượng instances
/// </summary>
public class BundleVersionUI : MonoBehaviour
{
    [Header("References")]
    public AssetBundleVersionManager bundleManager;
    public TMP_Dropdown versionDropdown;
    public Button loadButton;
    public Button nextButton;
    public Button prevButton;
    public Button clearAllButton; // NÚT MỚI
    public TMP_Text statusText;
    public TMP_Text instanceCountText; // TEXT MỚI: Hiển thị số instances

    [Header("Settings")]
    public bool updateInstanceCountEveryFrame = true;

    void Start()
    {
        // Tìm BundleManager nếu chưa assign
        if (bundleManager == null)
        {
            bundleManager = FindFirstObjectByType<AssetBundleVersionManager>();
        }

        if (bundleManager == null)
        {
            Debug.LogError("❌ Không tìm thấy AssetBundleVersionManager!");
            return;
        }

        // Setup Dropdown
        SetupDropdown();

        // Setup Buttons
        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadButtonClick);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButtonClick);
        
        if (prevButton != null)
            prevButton.onClick.AddListener(OnPrevButtonClick);

        // ===== NÚT CLEAR ALL MỚI =====
        if (clearAllButton != null)
            clearAllButton.onClick.AddListener(OnClearAllButtonClick);

        UpdateStatusText("Sẵn sàng");
    }

    void Update()
    {
        if (updateInstanceCountEveryFrame)
        {
            UpdateInstanceCount();
        }
    }

    void SetupDropdown()
    {
        if (versionDropdown == null) return;

        // Clear dropdown
        versionDropdown.ClearOptions();

        // Thêm các options từ BundleManager
        List<string> versions = bundleManager.GetVersionNames();
        versionDropdown.AddOptions(versions);

        // Set giá trị hiện tại
        versionDropdown.value = bundleManager.GetCurrentVersionIndex();

        // Listen to changes
        versionDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int index)
    {
        string versionName = bundleManager.availableVersions[index];
        UpdateStatusText($"🔄 Đang chuyển sang {versionName}...");
        bundleManager.SetVersionByIndex(index);
        UpdateStatusText($"✅ Đã load: {bundleManager.selectedVersion}");
    }

    void OnLoadButtonClick()
    {
        int index = versionDropdown.value;
        string versionName = bundleManager.availableVersions[index];
        UpdateStatusText($"🔄 Đang load {versionName}...");
        bundleManager.SetVersionByIndex(index);
        UpdateStatusText($"✅ Đã load: {bundleManager.selectedVersion}");
    }

    void OnNextButtonClick()
    {
        bundleManager.LoadNextVersion();
        versionDropdown.value = bundleManager.GetCurrentVersionIndex();
        UpdateStatusText($"➡️ Chuyển sang: {bundleManager.selectedVersion}");
    }

    void OnPrevButtonClick()
    {
        bundleManager.LoadPreviousVersion();
        versionDropdown.value = bundleManager.GetCurrentVersionIndex();
        UpdateStatusText($"⬅️ Chuyển sang: {bundleManager.selectedVersion}");
    }

    // ===== NÚT CLEAR ALL MỚI =====
    void OnClearAllButtonClick()
    {
        int count = bundleManager.GetInstanceCount();
        bundleManager.ClearAllInstances();
        UpdateStatusText($"🗑️ Đã xóa {count} object(s)");
        UpdateInstanceCount();
    }

    void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log(message);
    }

    // ===== CẬP NHẬT SỐ LƯỢNG INSTANCES =====
    void UpdateInstanceCount()
    {
        if (instanceCountText != null && bundleManager != null)
        {
            int count = bundleManager.GetInstanceCount();
            instanceCountText.text = $"Instances: {count}";
        }
    }
}