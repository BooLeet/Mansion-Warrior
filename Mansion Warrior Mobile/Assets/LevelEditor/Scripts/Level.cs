using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    [System.Serializable]
    public struct Index
    {
        public string setName;
        public string setIndex;
        public Index(string name, string index)
        {
            setName = name;
            setIndex = index;
        }

        public static bool operator==(Index lhs, Index rhs)
        {
            return lhs.setName == rhs.setName && lhs.setIndex == rhs.setIndex;
        }

        public static bool operator !=(Index lhs, Index rhs)
        {
            return lhs.setName != rhs.setName || lhs.setIndex != rhs.setIndex;
        }
    }

    [System.Serializable]
    public struct ObjectInfo
    {
        public Index index;
        public float posX, posY, posZ;
        public float rotX, rotY, rotZ;

        public ObjectInfo(Index indx, Vector3 position, Vector3 eulerAngles)
        {
            index = indx;
            posX = position.x;
            posY = position.y;
            posZ = position.z;

            rotX = eulerAngles.x;
            rotY = eulerAngles.y;
            rotZ = eulerAngles.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(posX, posY, posZ);
        }

        public Vector3 GetRotation()
        {
            return new Vector3(rotX, rotY, rotZ);
        }

        public static bool operator ==(ObjectInfo lhs, ObjectInfo rhs)
        {
            Vector3 lhsPosition = new Vector3(lhs.posX, lhs.posY, lhs.posZ);
            Vector3 rhsPosition = new Vector3(rhs.posX, rhs.posY, rhs.posZ);

            bool samePosition = Vector3.Distance(lhsPosition, rhsPosition) <= 0.001f;

            Vector3 lhsRotation = new Vector3(lhs.rotX, lhs.rotY, lhs.rotZ);
            Vector3 rhsRotation = new Vector3(rhs.rotX, rhs.rotY, rhs.rotZ);

            bool sameRotation = Vector3.Distance(lhsRotation, rhsRotation) <= 0.001f;

            return lhs.index == rhs.index &&
                samePosition &&
                sameRotation;
        }

        public static bool operator !=(ObjectInfo lhs, ObjectInfo rhs)
        {
            Vector3 lhsPosition = new Vector3(lhs.posX, lhs.posY, lhs.posZ);
            Vector3 rhsPosition = new Vector3(rhs.posX, rhs.posY, rhs.posZ);

            bool samePosition = Vector3.Distance(lhsPosition, rhsPosition) <= 0.001f;

            Vector3 lhsRotation = new Vector3(lhs.rotX, lhs.rotY, lhs.rotZ);
            Vector3 rhsRotation = new Vector3(rhs.rotX, rhs.rotY, rhs.rotZ);

            bool sameRotation = Vector3.Distance(lhsRotation, rhsRotation) <= 0.001f;

            return lhs.index != rhs.index ||
                !samePosition ||
                !sameRotation;
        }
    }


    [System.Serializable]
    public class LevelBlock
    {
        public Index up;
        public Index down;
        public Index right;
        public Index left;
        public Index forward;
        public Index back;

        public LevelBlock(Index mat)
        {
            this.up = mat;
            this.down = mat;
            this.right = mat;
            this.left = mat;
            this.forward = mat;
            this.back = mat;
        }

        public LevelBlock(Index up, Index down, Index right, Index left, Index forward, Index back)
        {
            this.up = up;
            this.down = down;
            this.right = right;
            this.left = left;
            this.forward = forward;
            this.back = back;
        }
    }
    private LevelBlock[,,] levelStructure;
    private List<ObjectInfo> objects;

    public uint sizeX { get; private set; }
    public uint sizeY { get; private set; }
    public uint sizeZ { get; private set; }

    private float scale;
    public string LevelLightingName { get; private set; }


    public Level(uint sizeX, uint sizeY, uint sizeZ, float scale, Index defaultMaterial, string leveLightingName)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.sizeZ = sizeZ;
        this.scale = scale;

        levelStructure = new LevelBlock[sizeX, sizeY, sizeZ];
        
        for (uint i = 1; i < sizeX - 1; ++i)
        {
            // Floor and ceiling
            for (uint j = 1; j < sizeZ - 1; ++j)
            {
                levelStructure[i, sizeY / 2, j] = new LevelBlock(defaultMaterial, defaultMaterial, defaultMaterial, defaultMaterial, defaultMaterial, defaultMaterial);
                //levelStructure[i, sizeY - 1, j] = new LevelBlock(null, defaultMaterial, null, null, null, null);
            }

            // Front and back Walls
            //for (uint j = 1; j < sizeY - 1; ++j)
            //{
            //    levelStructure[i, j, 0] = new LevelBlock(null, null, null, null, defaultMaterial, null);
            //    levelStructure[i, j, sizeZ - 1] = new LevelBlock(null, null, null, null, null, defaultMaterial);
            //}
        }

        //for (uint i = 1; i < sizeZ - 1; ++i)
        //{
        //    // Right and left Walls
        //    for (uint j = 1; j < sizeY - 1; ++j)
        //    {
        //        levelStructure[0, j, i] = new LevelBlock(null, null, defaultMaterial, null, null, null);
        //        levelStructure[sizeX - 1, j, i] = new LevelBlock(null, null, null, defaultMaterial, null, null);
        //    }
        //}

        objects = new List<ObjectInfo>();
        LevelLightingName = leveLightingName;
    }

    public bool IndexIsRemovable(Vector3Int index)
    {
        return IndexIsRemovable(index.x, index.y, index.z);
    }

    public bool IndexIsRemovable(int x, int y, int z)
    {
        return y > 0;
    }

    public bool IndexIsBorder(Vector3Int index)
    {
        return IndexIsBorder(index.x, index.y, index.z);
    }

    public bool IndexIsBorder(int x, int y, int z)
    {
        if (x == 0 || x == sizeX - 1)
            return true;

        if (y == 0 || y == sizeY - 1)
            return true;

        if (z == 0 || z == sizeZ - 1)
            return true;

        return false;
    }

    public bool IndexIsValid(Vector3Int index)
    {
        return IndexIsValid(index.x, index.y, index.z);
    }

    public bool IndexIsValid(int x, int y, int z)
    {
        if (x < 0 || x >= sizeX)
            return false;

        if (y < 0 || y >= sizeY)
            return false;

        if (z < 0 || z >= sizeZ)
            return false;

        return true;
    }

    public LevelBlock GetBlock(int x, int y, int z)
    {
        if (!IndexIsValid(x, y, z))
            return null;

        return levelStructure[x, y, z];
    }

    public LevelBlock GetBlock(Vector3Int index)
    {
        return GetBlock(index.x, index.y, index.z);
    }

    public void SetBlock(int x, int y, int z,LevelBlock newBlock)
    {
        if (!IndexIsValid(x, y, z))
            return;

        levelStructure[x, y, z] = newBlock;
    }

    public void SetBlock(Vector3Int index, LevelBlock newBlock)
    {
        SetBlock(index.x, index.y, index.z, newBlock);
    }

    public float GetScale()
    {
        return scale;
    }

    public void AddObject(ObjectInfo objectInfo)
    {
        if (objects == null)
            objects = new List<ObjectInfo>();
        objects.Add(objectInfo);
    }

    public void RemoveObject(ObjectInfo obj)
    {
        objects.Remove(obj);
    }

    public List<ObjectInfo> GetObjects()
    {
        return objects;
    }

    public void SetLevelLighting(string levelLightingName)
    {
        LevelLightingName = levelLightingName;
    }
}
