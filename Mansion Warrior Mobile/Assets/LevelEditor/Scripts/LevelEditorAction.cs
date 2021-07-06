using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelEditorAction
{
    public abstract bool Action(Level level, LevelEditor editor);

    public abstract void ReverseAction(Level level, LevelEditor editor);
}

public class LevelEditorActionAddBlock : LevelEditorAction
{
    Vector3Int blockPosition;
    Level.LevelBlock block;

    public LevelEditorActionAddBlock(Vector3Int newBlockPosition, Level.LevelBlock newBlock)
    {
        blockPosition = newBlockPosition;
        block = newBlock;
    }

    public override bool Action(Level level, LevelEditor editor)
    {
        level.SetBlock(blockPosition, block);
        editor.SetBlockDirty(blockPosition);

        return true;
    }

    public override void ReverseAction(Level level, LevelEditor editor)
    {
        level.SetBlock(blockPosition, null);
        editor.SetBlockDirty(blockPosition);
    }
}

public class LevelEditorActionRemoveBlock : LevelEditorAction
{
    Vector3Int blockPosition;
    Level.LevelBlock block;

    public LevelEditorActionRemoveBlock(Vector3Int newBlockPosition)
    {
        blockPosition = newBlockPosition;
    }

    public override bool Action(Level level, LevelEditor editor)
    {
        block = level.GetBlock(blockPosition);
        level.SetBlock(blockPosition, null);
        editor.SetBlockDirty(blockPosition);

        return true;
    }

    public override void ReverseAction(Level level, LevelEditor editor)
    {
        level.SetBlock(blockPosition, block);
        editor.SetBlockDirty(blockPosition);
    }
}

public class LevelEditorActionPaintBlock : LevelEditorAction
{
    Level.LevelBlock block;
    Vector3Int blockPosition;
    Utility.Direction direction;
    Level.Index newMaterial;
    Level.Index previousMaterial;


    public LevelEditorActionPaintBlock(Level.LevelBlock block, Vector3Int blockPosition, Utility.Direction direction, Level.Index materialReference)
    {
        this.block = block;
        this.blockPosition = blockPosition;
        this.direction = direction;
        this.newMaterial = materialReference;
    }

    bool ChangeBlockMaterial(ref Level.Index oldMaterial, Level.Index newMaterial, bool savePreviousMaterial = true)
    {
        if (oldMaterial == newMaterial)
            return false;

        if(savePreviousMaterial)
            previousMaterial = oldMaterial;
        oldMaterial = newMaterial;

        return true;
    }

    public override bool Action(Level level, LevelEditor editor)
    {
        bool setDirty = false;
        switch (direction)
        {
            case Utility.Direction.Up: setDirty = ChangeBlockMaterial(ref block.up, newMaterial); break;
            case Utility.Direction.Down: setDirty = ChangeBlockMaterial(ref block.down, newMaterial); break;

            case Utility.Direction.Right: setDirty = ChangeBlockMaterial(ref block.right, newMaterial); break;
            case Utility.Direction.Left: setDirty = ChangeBlockMaterial(ref block.left, newMaterial); break;

            case Utility.Direction.Forward: setDirty = ChangeBlockMaterial(ref block.forward, newMaterial); break;
            case Utility.Direction.Back: setDirty = ChangeBlockMaterial(ref block.back, newMaterial); break;
        }
        if (setDirty)
            editor.SetBlockDirty(blockPosition);

        return setDirty;
    }

    public override void ReverseAction(Level level, LevelEditor editor)
    {
        bool setDirty = false;
        switch (direction)
        {
            case Utility.Direction.Up: setDirty = ChangeBlockMaterial(ref block.up, previousMaterial, false); break;
            case Utility.Direction.Down: setDirty = ChangeBlockMaterial(ref block.down, previousMaterial, false); break;

            case Utility.Direction.Right: setDirty = ChangeBlockMaterial(ref block.right, previousMaterial, false); break;
            case Utility.Direction.Left: setDirty = ChangeBlockMaterial(ref block.left, previousMaterial, false); break;

            case Utility.Direction.Forward: setDirty = ChangeBlockMaterial(ref block.forward, previousMaterial, false); break;
            case Utility.Direction.Back: setDirty = ChangeBlockMaterial(ref block.back, previousMaterial, false); break;
        }
        if (setDirty)
            editor.SetBlockDirty(blockPosition);
    }
}


public class LevelEditorActionAddObject : LevelEditorAction
{
    Level.Index index;
    Vector3 position;
    Vector3 rotation;
    LevelEditorObject levelEditorObject;

    public LevelEditorActionAddObject(Level.Index index, Vector3 position, Vector3 rotation)
    {
        this.index = index;
        this.position = position;
        this.rotation = rotation;
    }

    public override bool Action(Level level, LevelEditor editor)
    {
        levelEditorObject = editor.builder.AddObject(new Level.ObjectInfo(index, position, rotation), true).GetComponent<LevelEditorObject>();
        editor.addedObjects.Add(levelEditorObject);

        return true;
    }

    public override void ReverseAction(Level level, LevelEditor editor)
    {
        editor.addedObjects.Remove(levelEditorObject);
        if (levelEditorObject == null)
            return;

        level.RemoveObject(levelEditorObject.objectInfo);
        Object.Destroy(levelEditorObject.gameObject);
    }
}

public class LevelEditorActionRemoveObject : LevelEditorAction
{
    LevelEditorObject selectedObject;

    public LevelEditorActionRemoveObject(LevelEditorObject selectedObject)
    {
        this.selectedObject = selectedObject;
    }

    public override bool Action(Level level, LevelEditor editor)
    {
        editor.addedObjects.Remove(selectedObject);
        level.RemoveObject(selectedObject.objectInfo);
        //Object.Destroy(selectedObject.gameObject);
        selectedObject.gameObject.SetActive(false);

        return true;
    }

    public override void ReverseAction(Level level, LevelEditor editor)
    {
        selectedObject.gameObject.SetActive(true);
        editor.addedObjects.Add(selectedObject);
        //editor.addedObjects.Add(editor.builder.AddObject(index, position, rotation, true).GetComponent<LevelEditorObject>());
    }
}