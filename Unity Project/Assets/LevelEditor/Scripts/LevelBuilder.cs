using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelBuilder : MonoBehaviour
{
    private Mesh onefaceQuad;

    public Material errorMaterial;
    public Mesh cubeMesh;
    public LevelEditorCollection Collection;
    public NavMeshSurface navMeshSurface;
    public bool buildNavMesh = false;

    private Transform objectHolder;
    private Transform blockHolder;

    private class LevelEditorBlockContainer
    {
        public LevelEditorBlock up, down, right, left, forward, back;
    }
    private LevelEditorBlockContainer[,,] levelEditorBlocks = null;

    public void ClearLevel()
    {
        Utility.RemoveChildren(transform);
        levelEditorBlocks = null;
        navMeshSurface.RemoveData();
    }

    public void BuildLevel(Level level, bool storeEditorInfo)
    {
        Utility.RemoveChildren(transform);
        levelEditorBlocks = null;
        if (storeEditorInfo)
            levelEditorBlocks = new LevelEditorBlockContainer[level.sizeX, level.sizeY, level.sizeZ];
            
        onefaceQuad = CreateQuadOneFace(level.GetScale());

        GameObject blockHolderObj = new GameObject("Block Holder");
        blockHolderObj.transform.parent = transform;
        blockHolderObj.transform.localPosition = Vector3.zero;
        blockHolder = blockHolderObj.transform;

        GameObject objectHolderObj = new GameObject("Object Holder");
        objectHolderObj.transform.parent = transform;
        objectHolderObj.transform.localPosition = Vector3.zero;
        objectHolder = objectHolderObj.transform;

        for (int i = 0; i < level.sizeX; ++i)
            for (int j = 0; j < level.sizeY; ++j)
                for (int k = 0; k < level.sizeZ; ++k)
                    BuildBlock(new Vector3Int(i, j, k), level, storeEditorInfo);



        List<Level.ObjectInfo> objects = level.GetObjects();
        if(objects != null)
            foreach (Level.ObjectInfo obj in objects)
                AddObject(obj, storeEditorInfo);

        ApplyLighting(level);

        if (storeEditorInfo)
            return;

        for (int i = 0; i < level.sizeX; ++i)
            for (int j = 0; j < level.sizeY; ++j)
                for (int k = 0; k < level.sizeZ; ++k)
                {
                    if (level.GetBlock(i, j, k) == null)
                        continue;

                    int blockCount = 1;
                    int endIndx = k;
                    for (int indx = k + 1; indx < level.sizeZ; ++indx)
                    {
                        if (level.GetBlock(i, j, indx) == null)
                        {
                            endIndx = indx;
                            break;
                        }
                        blockCount++;
                    }

                    GameObject obj = new GameObject("Collider " + i + " " + j + " " + k);
                    obj.transform.parent = blockHolder;
                    obj.transform.position = new Vector3(i, j, k + (blockCount - 1) / 2f) * level.GetScale();
                    obj.transform.localScale = new Vector3(1, 1, blockCount) * level.GetScale();
                    BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
                    boxCollider.size = Vector3.one;
                    obj.AddComponent<MeshFilter>().mesh = cubeMesh;
                    MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    renderer.material = errorMaterial;
                    k = endIndx;
                }

        if (buildNavMesh)
            navMeshSurface.BuildNavMesh();
    }

    public void ApplyLighting(Level level)
    {
        if (level == null)
            return;

        LevelLighting lighting = Collection.GetLevelLighting(level.LevelLightingName);
        if (lighting != null)
            lighting.ApplyLighting();
    }

    public void BuildBlock(Vector3Int index, Level level, bool storeBlockInfo)
    {
        int i = index.x;
        int j = index.y;
        int k = index.z;

        Level.LevelBlock levelBlock = level.GetBlock(i, j, k);
        if (levelBlock == null)
            return;

        bool upVisible, downVisible, rightVisible, leftVisible, frontVisible, backVisible;
        upVisible = level.GetBlock(i, j + 1, k) == null;
        downVisible = level.GetBlock(i, j - 1, k) == null;

        rightVisible = level.GetBlock(i + 1, j, k) == null;
        leftVisible = level.GetBlock(i - 1, j, k) == null;

        frontVisible = level.GetBlock(i, j, k + 1) == null;
        backVisible = level.GetBlock(i, j, k - 1) == null;


        //Vector3Int index = new Vector3Int((int)i, (int)j, (int)k);
        if (upVisible)
            InstantiateFace(Collection.GetMaterial(levelBlock.up), level.GetScale(), index, Utility.Direction.Up, blockHolder, storeBlockInfo);

        if (downVisible)
            InstantiateFace(Collection.GetMaterial(levelBlock.down), level.GetScale(), index, Utility.Direction.Down, blockHolder, storeBlockInfo);

        if (frontVisible)
            InstantiateFace(Collection.GetMaterial(levelBlock.forward), level.GetScale(), index, Utility.Direction.Forward, blockHolder, storeBlockInfo);

        if (backVisible)
            InstantiateFace(Collection.GetMaterial(levelBlock.back), level.GetScale(), index, Utility.Direction.Back, blockHolder, storeBlockInfo);

        if (rightVisible)
            InstantiateFace(Collection.GetMaterial(levelBlock.right), level.GetScale(), index, Utility.Direction.Right, blockHolder, storeBlockInfo);

        if (leftVisible)
            InstantiateFace(Collection.GetMaterial(levelBlock.left), level.GetScale(), index, Utility.Direction.Left, blockHolder, storeBlockInfo);
    }

    public GameObject AddObject(Level.ObjectInfo objectInfo, bool storeEditorInfo)
    {
        GameObject obj = Instantiate(Collection.GetObject(objectInfo.index), objectHolder);
        obj.transform.position = objectInfo.GetPosition();
        obj.transform.rotation = Quaternion.Euler(objectInfo.GetRotation());
        if(storeEditorInfo)
        {
            LevelEditorObject levelEditorObject = obj.AddComponent<LevelEditorObject>();
            levelEditorObject.objectInfo = objectInfo;
        }
            
            
        return obj;
    }

    public void RebuildSurroundingBlocks(Vector3Int index, Level level, bool storeEditorInfo)
    {
        if (!level.IndexIsValid(index))
            return;

        DeleteBlock(level, index);
        BuildBlock(index, level, storeEditorInfo);

        Vector3Int upIndex = index + new Vector3Int(0, 1, 0);
        Vector3Int downIndex = index + new Vector3Int(0, -1, 0);

        Vector3Int rightIndex = index + new Vector3Int(1, 0, 0);
        Vector3Int leftIndex = index + new Vector3Int(-1, 0, 0);

        Vector3Int forwardIndex = index + new Vector3Int(0, 0, 1);
        Vector3Int backIndex = index + new Vector3Int(0, 0, -1);

        DeleteBlock(level, upIndex);
        BuildBlock(upIndex, level, storeEditorInfo);

        DeleteBlock(level, downIndex);
        BuildBlock(downIndex, level, storeEditorInfo);

        DeleteBlock(level, rightIndex);
        BuildBlock(rightIndex, level, storeEditorInfo);

        DeleteBlock(level, leftIndex);
        BuildBlock(leftIndex, level, storeEditorInfo);

        DeleteBlock(level, forwardIndex);
        BuildBlock(forwardIndex, level, storeEditorInfo);

        DeleteBlock(level, backIndex);
        BuildBlock(backIndex, level, storeEditorInfo);
    }

    void DeleteBlock(Level level, Vector3Int index)
    {
        if (!level.IndexIsValid(index) || levelEditorBlocks[index.x, index.y, index.z] == null)
            return;

        Utility.DestroyBehavioursObject(levelEditorBlocks[index.x, index.y, index.z].up);
        Utility.DestroyBehavioursObject(levelEditorBlocks[index.x, index.y, index.z].down);

        Utility.DestroyBehavioursObject(levelEditorBlocks[index.x, index.y, index.z].right);
        Utility.DestroyBehavioursObject(levelEditorBlocks[index.x, index.y, index.z].left);

        Utility.DestroyBehavioursObject(levelEditorBlocks[index.x, index.y, index.z].forward);
        Utility.DestroyBehavioursObject(levelEditorBlocks[index.x, index.y, index.z].back);

        levelEditorBlocks[index.x, index.y, index.z] = null;
    }

    private GameObject InstantiateFace(Material mat, float scale, Vector3Int index, Utility.Direction direction, Transform parent, bool storeEditorInfo)
    {
        if (mat == null)
            mat = errorMaterial;

        Vector3 position = new Vector3(index.x, index.y, index.z) * scale;
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        Vector3 offset = Vector3.zero;
        GetRotationAndOffsetFromDirection(direction, ref rotation, ref offset, scale);
        position += offset;


        GameObject obj = new GameObject("Block Face");
        obj.transform.parent = parent;
        obj.transform.localPosition = position;
        obj.transform.localRotation = rotation;
        obj.AddComponent<MeshFilter>().sharedMesh = onefaceQuad;
        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = mat;
        if(storeEditorInfo)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        else
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        if (storeEditorInfo)
        {
            obj.AddComponent<MeshCollider>().sharedMesh = onefaceQuad;
            LevelEditorBlock info = obj.AddComponent<LevelEditorBlock>();
            info.levelIndex = index;
            info.direction = direction;
            if (levelEditorBlocks[index.x, index.y, index.z] == null)
                levelEditorBlocks[index.x, index.y, index.z] = new LevelEditorBlockContainer();

            switch (direction)
            {
                case Utility.Direction.Up:
                    levelEditorBlocks[index.x, index.y, index.z].up = info;
                    break;
                case Utility.Direction.Down:
                    levelEditorBlocks[index.x, index.y, index.z].down = info;
                    break;

                case Utility.Direction.Right:
                    levelEditorBlocks[index.x, index.y, index.z].right = info;
                    break;
                case Utility.Direction.Left:
                    levelEditorBlocks[index.x, index.y, index.z].left = info;
                    break;

                case Utility.Direction.Forward:
                    levelEditorBlocks[index.x, index.y, index.z].forward = info;
                    break;
                case Utility.Direction.Back:
                    levelEditorBlocks[index.x, index.y, index.z].back = info;
                    break;
            }
        }


        return obj;
    }

    public static void GetRotationAndOffsetFromDirection(Utility.Direction direction, ref Quaternion rotation, ref Vector3 offset, float scale)
    {
        offset = Vector3.zero;
        switch (direction)
        {
            case Utility.Direction.Up: rotation = Quaternion.Euler(90, 0, 0); offset += Vector3.up * scale / 2; break;
            case Utility.Direction.Down: rotation = Quaternion.Euler(-90, 0, 0); offset -= Vector3.up * scale / 2; break;

            case Utility.Direction.Right: rotation = Quaternion.Euler(0, -90, 0); offset += Vector3.right * scale / 2; break;
            case Utility.Direction.Left: rotation = Quaternion.Euler(0, 90, 0); offset -= Vector3.right * scale / 2; break;

            case Utility.Direction.Forward: rotation = Quaternion.Euler(0, 180, 0); offset += Vector3.forward * scale / 2; break;
            case Utility.Direction.Back: rotation = Quaternion.Euler(0, 0, 0); offset -= Vector3.forward * scale / 2; break;
        }
    }

    public static Mesh CreateQuadOneFace(float size)
    {
        Mesh mesh = new Mesh();

        float halfSize = size / 2;
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-halfSize, -halfSize, 0),
            new Vector3(halfSize, -halfSize, 0),
            new Vector3(-halfSize, halfSize, 0),
            new Vector3(halfSize, halfSize, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
              new Vector2(0, 0),
              new Vector2(1, 0),
              new Vector2(0, 1),
              new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }

}
