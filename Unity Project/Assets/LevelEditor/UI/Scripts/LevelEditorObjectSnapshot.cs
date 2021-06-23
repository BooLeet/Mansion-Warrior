using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorObjectSnapshot : MonoBehaviour
{
    // https://antoinefougea.com/blog/dynamic-inventory-thumbnails-in-unity
    public Camera snapshotCamera;
    public Transform objectSlot;
    public Vector2Int snapshotSize;
    public float snapshotPadding;
    public float snapshotInvertRatio;
    public int layerNumber = 9;
    RenderTexture rt;

    void Awake()
    {
        rt = new RenderTexture(snapshotSize.x, snapshotSize.y, 24, RenderTextureFormat.ARGB32, 1);
        snapshotCamera.targetTexture = rt;
    }

    private static Bounds CalculateCompoundBounds(GameObject root)
    {
        Bounds bounds = new Bounds(root.transform.position, Vector3.zero);

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        return bounds;
    }

    private Texture2D CaptureSnapshot(Bounds bounds)
    {
        // Position the camera
        snapshotCamera.orthographicSize = Mathf.Max(bounds.extents.x * snapshotInvertRatio, bounds.extents.y) + snapshotPadding;
        snapshotCamera.transform.position = new Vector3(bounds.center.x, bounds.center.y, snapshotCamera.transform.position.z);

        // Render the thumbnail
        snapshotCamera.Render();

        // Save the thumbnail to a Texture2D
        RenderTexture rtCache = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D thumbnail = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        thumbnail.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        thumbnail.Apply();
        RenderTexture.active = rtCache;

        return thumbnail;
    }

    public Texture2D GetSnapshot(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, objectSlot);
        Utility.ChangeLayerFull(obj, layerNumber);

        Texture2D texture = CaptureSnapshot(CalculateCompoundBounds(objectSlot.gameObject));
        //Debug.LogError("Aye");
        DestroyImmediate(obj);
        return texture;
    }
}
