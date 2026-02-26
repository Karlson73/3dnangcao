using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraLayerCullDistances : MonoBehaviour
{
    [Header("Per-layer cull distances (meters)")]
    [Tooltip("Array length must be 32 (one entry per layer). Set 0 to disable layer-based culling for that layer.")]
    public float[] layerCullDistances = new float[32];

    [Tooltip("Use spherical culling instead of planar culling")]
    public bool sphericalCulling = true;

    Camera _camera;

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
        Apply();
    }

    void OnValidate()
    {
        if (layerCullDistances == null || layerCullDistances.Length != 32)
            layerCullDistances = new float[32];
        Apply();
    }

    void Start()
    {
        // ensure runtime application as well
        if (_camera == null) _camera = GetComponent<Camera>();
        Apply();
    }

    /// <summary>
    /// Apply the current settings directly to the Camera component.
    /// </summary>
    public void Apply()
    {
        if (_camera == null) _camera = GetComponent<Camera>();
        if (_camera == null) return;

        _camera.layerCullDistances = layerCullDistances;
        _camera.layerCullSpherical = sphericalCulling;
    }

    /// <summary>
    /// Convenience methods to set per-layer distances.
    /// </summary>
    public void SetDistanceForLayer(int layer, float distance)
    {
        if (layer < 0 || layer >= 32) return;
        layerCullDistances[layer] = distance;
        Apply();
    }

    public void SetDistanceForLayer(string layerName, float distance)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer >= 0) SetDistanceForLayer(layer, distance);
    }
}
