# Asset Bundles — Hướng dẫn thiết lập và sử dụng 🔧

**Mô tả ngắn:** Tài liệu này hướng dẫn cách gán bundle, build theo version, load runtime, dùng UI để chuyển đổi version và các lưu ý quan trọng.

---

## 🚀 Quick Start (3 bước)
1. **Gán bundle name** cho asset/prefab: chọn asset → menu **Assets → Assign Bundle Name/...** (hoặc *Custom...*).  
2. **Build bundle**: Editor → **Assets → Build Asset Bundles → Build to Version v1.0** (hoặc `Normal Build`/`Custom...`). Output mặc định: `Assets/StreamingAssets/Bundles/<version>/<bundleName>`  
3. **Chạy scene**: Gắn `AssetBundleVersionManager` + (tùy) `BundleVersionUI`. Chọn version → **Load/Next/Prev** → prefab sẽ được instantiate theo `assetName`.

---

## 🧩 Thành phần chính
- `AssetBundleVersionManager.cs` — quản lý các version, load/unload bundle, instantiate prefab, track instances.
- `BundleVersionUI.cs` — UI để chọn version, điều khiển load/clear và hiển thị trạng thái + số instance.
- `LoadAssetBundle_Version.cs` (file: `LoadAssetBundle.cs`) — script đơn giản để load 1 bundle/asset từ folder version.
- `QuickBundleAssign.cs` — menu editor để gán/clear bundle name nhanh (kèm dialog Custom).
- `BuildAssetBundles.cs` — menu build bundles vào folder version (`Assets/StreamingAssets/Bundles/<version>`).

---

## 🔧 Thiết lập Inspector (chi tiết)
- `AssetBundleVersionManager`
  - `availableVersions`: danh sách version; bật `autoDetectVersions` để tự động lấy từ `StreamingAssets/Bundles`.
  - `selectedVersion`, `bundleName`, `assetName`: phải đúng tên folder/bundle/asset.
  - `instantiateOnLoad`, `spawnPosition`.
  - `destroyOldInstanceOnSwitch` / `destroyAllOldInstances` để kiểm soát object cũ khi đổi version.

- `BundleVersionUI`
  - Gán: `bundleManager`, `versionDropdown`, `loadButton`, `nextButton`, `prevButton`, `clearAllButton`, `statusText`, `instanceCountText`.
  - `updateInstanceCountEveryFrame`: true giúp cập nhật số lượng instances liên tục.

- `LoadAssetBundle`
  - `useVersionFolder` & `versionFolder`: bật để load từ `Bundles/<version>/<bundleName>`.

---

## 📂 Cấu trúc folder & conventions
- Build output mặc định: `Assets/StreamingAssets/Bundles/<version>/<bundleName>` (mỗi bundle là 1 file).  
- Quy ước tên bundle: **chữ thường**, ví dụ: `player`, `enemies`, `terrain`.

---

## 🧪 Lưu ý & Troubleshooting ⚠️
- Nếu file bundle không tồn tại: kiểm tra đường dẫn `Application.streamingAssetsPath + "/Bundles/<version>/<bundleName>"`.
- Asset không tìm thấy: `assetName` phải khớp chính xác (case-sensitive).
- Trước khi load version mới, `AssetBundleVersionManager` gọi `UnloadAllBundles()` → gọi `Unload(true)` để giải phóng assets cũ.
- `LoadAssetBundle` dùng `bundle.Unload(false)` (giữ assets) → **quản lý bộ nhớ thủ công** nếu cần.
- Kiểm tra platform build target (EditorUserBuildSettings.activeBuildTarget) khi build bundles.

---

## ✅ Best Practices
- Sử dụng `QuickBundleAssign` để gán bundle nhanh cho nhiều assets (menu Assets → Assign Bundle Name).
- Luôn kiểm tra `Assets/StreamingAssets/Bundles/<version>/` sau khi build để đảm bảo bundle được tạo.
- Dùng version folders (v1.0, v2.0, beta, ...) để dễ rollback và test.

---

## 📚 API nhanh (Public methods)
- `AssetBundleVersionManager`
  - `SwitchToVersion(string)`
  - `LoadNextVersion()` / `LoadPreviousVersion()`
  - `ClearAllInstances()`
  - `SpawnAtPosition(Vector3)`
  - `SetVersionByIndex(int)`
- `LoadAssetBundle`
  - `LoadFromVersion(string)`
  - `SpawnLoadedPrefab(Vector3)`

---

## 🔁 Ví dụ workflow mẫu
1. Chọn prefab `player` → **Assets → Assign Bundle Name/player**.  
2. **Assets → Build Asset Bundles → Build to Version v1.0**.  
3. Chạy scene có `BundleVersionUI`, chọn `v1.0`, nhấn **Load** → sẽ instantiate asset `Capsule` (nếu `assetName = "Capsule"`).

---

## ✅ Checklist (quick)
- [ ] Gán bundle name cho tất cả assets cần đóng gói
- [ ] Build bundles cho version test (v1.0)
- [ ] Kiểm tra file output trong `Assets/StreamingAssets/Bundles/v1.0/`
- [ ] Gắn `AssetBundleVersionManager` vào scene và cấu hình `bundleName`/`assetName`
- [ ] (Tùy) Gắn `BundleVersionUI` và map các UI element
- [ ] Chạy scene và test load/switch/clear

---

> Muốn mình thêm phần ảnh/screenshots cho từng bước Inspector (để dễ follow) không? 📸
