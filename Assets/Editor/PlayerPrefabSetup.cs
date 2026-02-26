#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class PlayerPrefabSetup
{
    private const string PrefabPath = "Assets/player.prefab";
    private const string PlayerTag = "Player";

    [MenuItem("Tools/Player Prefab/Setup Player Prefab")]
    public static void SetupPlayerPrefab()
    {
        // Load prefab contents
        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
        if (root == null)
        {
            Debug.LogError($"❌ Không tìm thấy prefab tại {PrefabPath}");
            return;
        }

        // Ensure tag exists
        AddTagIfMissing(PlayerTag);

        // Set tag
        root.tag = PlayerTag;

        // Add AudioSource if missing
        AudioSource audio = root.GetComponent<AudioSource>();
        if (audio == null)
        {
            audio = root.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            Debug.Log("✅ Đã thêm AudioSource (PlayOnAwake=false)");
        }
        else
        {
            audio.playOnAwake = false;
            Debug.Log("ℹ️ AudioSource đã tồn tại; PlayOnAwake=false đã được áp dụng");
        }

        // Add PlayerItemCollector if missing
        var collector = root.GetComponent<PlayerItemCollector>();
        if (collector == null)
        {
            collector = root.AddComponent<PlayerItemCollector>();
            collector.itemTag = "Item"; // sensible default
            Debug.Log("✅ Đã thêm PlayerItemCollector");
        }
        else
        {
            Debug.Log("ℹ️ PlayerItemCollector đã tồn tại");
        }

        // Assign audioSource reference in collector
        if (collector != null && collector.audioSource != audio)
        {
            collector.audioSource = audio;
            Debug.Log("✅ Gán AudioSource cho PlayerItemCollector.audioSource");
        }

        // Save and unload prefab contents
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        PrefabUtility.UnloadPrefabContents(root);
        AssetDatabase.Refresh();

        Debug.Log("🔧 Player prefab updated successfully.");
    }

    private static void AddTagIfMissing(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // Check if tag exists
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue == tag)
                return;
        }

        // Add new tag
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
        newTag.stringValue = tag;
        tagManager.ApplyModifiedProperties();

        Debug.Log($"✅ Tag '{tag}' đã được thêm vào Tag Manager");
    }
}
#endif