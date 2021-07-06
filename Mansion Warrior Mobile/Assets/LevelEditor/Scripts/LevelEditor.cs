using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor Instance { get; private set; }

    public LevelBuilder builder;
    public LevelEditorInput input;

    [Header("Camera")]
    public Transform cameraHolder;
    public Transform cameraTransform;
    public float cameraSpeed = 1;
    public float cameraSpeedMultiplier = 2;
    [Space]
    public float cameraRotationSpeed = 1;
    private float cameraVerticalAxis = 0;
   
    private Level level;
    [Header("Editing")]
    public Vector3Int levelSize = new Vector3Int(55, 20, 55);
    public float levelScale = 5;
    public Level.Index defaultLevelIndex = new Level.Index("default", "White");
    public string defaultLevelLighting = "Foundry01";
    [Space]
    public float raycastLength = 30;
    public float objectRaycastDistance = 12;
    private LevelEditorBlock currentBlock = null;
    private Vector3 currentObjectPosition;
    private float currentObjectRotation;
    public float objectRotationSpeed = 1;
    public int raycastIgnoreLayer = 10;
    public List<LevelEditorObject> addedObjects;
    public LevelEditorObject SelectedObject { get; private set; }

    private Vector3Int dirtyIndex;
    private bool dirtyFlag = false;

    private Stack<LevelEditorAction> previousActions;
    private Stack<LevelEditorAction> redoActions;

    [Header("Highlighter")]
    public Material highlightMaterial;
    public Mesh cubeMesh;
    private GameObject blockHighlighter;
    private GameObject objectHighlighter;
    private GameObject objectToPlace;
    private LevelEditorStates.LevelEditorState currentState;


    [Header("UI")]
    public LevelEditorUI ui;
    public const int maxSelectedItems = 8;
    public bool ConfirmSave { get; set; }
    public bool CancelSave { get; set; }
    public bool ConfirmLoad { get; set; }
    public string LevelToLoad { get; private set; }
    public bool CancelLoad { get; set; }

    public class SelectedItem
    {
        public enum ItemType { Material, Object}

        public Level.Index index;
        public ItemType type;

        public SelectedItem(Level.Index indx, ItemType itemType)
        {
            index = indx;
            type = itemType;
        }

    }
    private SelectedItem[] currentSelectedItems;
    public SelectedItem.ItemType SelectedItemType { get { return currentSelectedItems[selectedItemIndex].type; } }
    private int selectedItemIndex = 0;

    private KeyCode[] selectionKeyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        level = new Level((uint)levelSize.x, (uint)levelSize.y, (uint)levelSize.z, levelScale, defaultLevelIndex, defaultLevelLighting);
        builder.BuildLevel(level, true);
        addedObjects = new List<LevelEditorObject>();
        previousActions = new Stack<LevelEditorAction>();
        redoActions = new Stack<LevelEditorAction>();

        CreateBlockHighlighter();
        CreateObjectHighlighter();
        SelectedItemsSetup();
        UpdateDashboard();
        
        currentState = new LevelEditorStates.EditingState();

        cameraHolder.transform.position = new Vector3(levelSize.x, levelSize.y + 2, levelSize.z) * levelScale / 2;
    }

    private void Update()
    {
        if (GameManager.IsPaused())
            return;

        StateMachine();

        if (dirtyFlag)
        {
            builder.RebuildSurroundingBlocks(dirtyIndex, level, true);
            dirtyFlag = false;
        }

    }

    #region Editing
    private void DoAction(LevelEditorAction action)
    {
        if (!action.Action(level, this))
            return;

        previousActions.Push(action);
        redoActions.Clear();
    }

    public void UndoAction()
    {
        if (previousActions.Count == 0)
            return;
        redoActions.Push(previousActions.Pop());
        redoActions.Peek().ReverseAction(level, this);
    }

    public void RedoAction()
    {
        if (redoActions.Count == 0)
            return;
        previousActions.Push(redoActions.Pop());
        previousActions.Peek().Action(level, this);
    }

    public void AddBlock()
    {
        if (currentBlock == null)
            return;

        Vector3Int direction = new Vector3Int(0, 0, 0);
        switch (currentBlock.direction)
        {
            case Utility.Direction.Up: direction += Vector3Int.up; break;
            case Utility.Direction.Down: direction += Vector3Int.down; break;

            case Utility.Direction.Right: direction += Vector3Int.right; break;
            case Utility.Direction.Left: direction += Vector3Int.left; break;

            case Utility.Direction.Forward: direction += new Vector3Int(0, 0, 1); break;
            case Utility.Direction.Back: direction += new Vector3Int(0, 0, -1); break;
        }

        Vector3Int newBlockPosition = currentBlock.levelIndex + direction;
        if (level.GetBlock(newBlockPosition) != null)
            return;
        Level.LevelBlock block = new Level.LevelBlock(currentSelectedItems[selectedItemIndex].index);

        DoAction(new LevelEditorActionAddBlock(newBlockPosition, block));
        //level.SetBlock(newBlockPosition, block);
        //SetBlockDirty(newBlockPosition);
    }

    public void RemoveBlock()
    {
        if (currentBlock == null || !level.IndexIsRemovable(currentBlock.levelIndex))
            return;
        DoAction(new LevelEditorActionRemoveBlock(currentBlock.levelIndex));
        //level.SetBlock(currentBlock.levelIndex, null);
        //SetBlockDirty(currentBlock.levelIndex);
    }

    public void PaintBlock()
    {
        PaintBlock(currentSelectedItems[selectedItemIndex].index);
    }
    private void PaintBlock(Level.Index materialReference)
    {
        if (currentBlock == null)
            return;

        Level.LevelBlock block = level.GetBlock(currentBlock.levelIndex);
        DoAction(new LevelEditorActionPaintBlock(block, currentBlock.levelIndex, currentBlock.direction, materialReference));
    }

    public void SetBlockDirty(Vector3Int index)
    {
        dirtyIndex = index;
        dirtyFlag = true;
    }

    public void AddObject()
    {
        DoAction(new LevelEditorActionAddObject(currentSelectedItems[selectedItemIndex].index, currentObjectPosition, new Vector3(0, currentObjectRotation, 0)));
        //addedObjects.Add(builder.AddObject(
        //    currentSelectedItems[selectedItemIndex].index, currentObjectPosition, new Vector3(0, currentObjectRotation, 0), true).GetComponent<LevelEditorObject>());
    }

    public void RemoveObject()
    {
        if (SelectedObject == null)
            return;
        DoAction(new LevelEditorActionRemoveObject(SelectedObject));
        //addedObjects.Remove(SelectedObject);
        //level.RemoveObject(new Level.ObjectInfo(SelectedObject.index, SelectedObject.transform.position, SelectedObject.transform.eulerAngles));
        //Destroy(SelectedObject.gameObject);
    }

    public void RotateObject(bool clockWise)
    {
        currentObjectRotation += 15 * (clockWise ? 1 : -1);
    }

    #endregion

    #region State Machine
    bool stateInit = true;
    void StateMachine()
    {
        if (stateInit)
        {
            currentState.Init(this);
            stateInit = false;
        }

        currentState.Action(this);
        LevelEditorStates.LevelEditorState nextState = currentState.Transition(this);
        if (nextState != null)
        {
            stateInit = true;
            currentState = nextState;
        }
    }
    #endregion

    #region Camera
    public void UpdateCamera()
    {
        Vector2 directionInput = input.GetCameraMovementInput();
        if (directionInput.magnitude > 1)
            directionInput.Normalize();

        float vertical = (input.GetCameraUp() ? 1 : 0) - (input.GetCameraDown() ? 1 : 0);
        float speedMultiplier = input.GetCameraSpeedUp() ? cameraSpeedMultiplier : 1;
        Vector3 speedVector = speedMultiplier * cameraSpeed * (cameraHolder.forward * directionInput.y + cameraHolder.up * vertical + cameraHolder.right * directionInput.x);

        cameraHolder.position += speedVector * Time.deltaTime;

        Vector2 rotationInput = input.GetCameraRotationInput();
        cameraHolder.Rotate(cameraRotationSpeed * new Vector3(0, rotationInput.x, 0));

        cameraVerticalAxis = Mathf.Clamp(cameraVerticalAxis - rotationInput.y * cameraRotationSpeed, -90, 90);
        cameraTransform.localRotation = Quaternion.Euler(cameraVerticalAxis, 0, 0);
    }

    public void UpdateCurrentBlock()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastLength))
            currentBlock = hit.collider.GetComponent<LevelEditorBlock>();
        else
            currentBlock = null;
    }

    public void UpdateObjectPosition()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        int layerMask = 1 << raycastIgnoreLayer;
        layerMask = ~layerMask;
        if (Physics.Raycast(ray, out hit, objectRaycastDistance,layerMask))
        {
            currentObjectPosition = hit.point;
            SelectedObject = Utility.GetComponentOnObject<LevelEditorObject>(hit.transform.gameObject);
        }
        else
        {
            currentObjectPosition = cameraTransform.position + cameraTransform.forward * objectRaycastDistance;
            SelectedObject = null;
        }
            

        float gridSize = level.GetScale() / 10;
        currentObjectPosition.x = Mathf.Round(currentObjectPosition.x / gridSize) * gridSize;
        currentObjectPosition.y = Mathf.Round(currentObjectPosition.y / gridSize) * gridSize;
        currentObjectPosition.z = Mathf.Round(currentObjectPosition.z / gridSize) * gridSize;
    }

    #endregion

    #region Highlighter
    void CreateBlockHighlighter()
    {
        if (blockHighlighter)
            Destroy(blockHighlighter);

        blockHighlighter = new GameObject("Block Highlighter");
        blockHighlighter.AddComponent<MeshFilter>().sharedMesh = LevelBuilder.CreateQuadOneFace(levelScale);
        MeshRenderer renderer = blockHighlighter.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = highlightMaterial;
        renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

        blockHighlighter.SetActive(false);
    }

    void CreateObjectHighlighter()
    {
        if (objectHighlighter)
            Destroy(objectHighlighter);

        objectHighlighter = new GameObject("Object Highlighter");
        objectHighlighter.transform.localScale = Vector3.one * level.GetScale();
        objectHighlighter.AddComponent<MeshFilter>().sharedMesh = cubeMesh;
        MeshRenderer renderer = objectHighlighter.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = highlightMaterial;
        renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

        objectHighlighter.SetActive(false);
    }

    public void ShowHideBlockHighilighter(bool val)
    {
        blockHighlighter.SetActive(val);
    }

    public void UpdateBlockHighlighter()
    {
        if (currentBlock == null)
        {
            blockHighlighter.SetActive(false);
            return;
        }

        blockHighlighter.SetActive(true);

        Vector3Int index = currentBlock.levelIndex;
        Vector3 position = new Vector3(index.x, index.y, index.z) * level.GetScale();
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        Vector3 offset = Vector3.zero;
        LevelBuilder.GetRotationAndOffsetFromDirection(currentBlock.direction, ref rotation, ref offset, level.GetScale() + 0.1f);
        position += offset;

        blockHighlighter.transform.position = position;
        blockHighlighter.transform.rotation = rotation;
    }

    public void ShowHideObjectHighilighter(bool val)
    {
        objectHighlighter.SetActive(val);
    }

    public void UpdateObjectHighlighter(bool showObject)
    {
        if (SelectedObject != null && (objectToPlace == null || !showObject))
        {
            objectHighlighter.transform.position = SelectedObject.transform.position + Vector3.up * level.GetScale() / 2;
            objectHighlighter.transform.rotation = SelectedObject.transform.rotation;
        }
        else
        {
            objectHighlighter.transform.position = currentObjectPosition + Vector3.up * level.GetScale() / 2;
            objectHighlighter.transform.rotation = Quaternion.Euler(0, currentObjectRotation, 0);
        }
        
        if (objectToPlace == null)
            return;
        objectToPlace.transform.position = currentObjectPosition;
        objectToPlace.transform.rotation = Quaternion.Euler(0, currentObjectRotation, 0);

        objectToPlace.SetActive(showObject);
    }

    #endregion

    #region Controls
    public void DashboardSelectionUpdate()
    {
        for (int i = 0; i < maxSelectedItems; ++i)
        {
            if (Input.GetKeyDown(selectionKeyCodes[i]))
            {
                ChangeSelectedItemIndex(i);
                break;
            }
        }

        if(input.GetNextSelectedItem())
            ChangeSelectedItemIndex(selectedItemIndex + 1);
        else if(input.GetPreviousSelectedItem())
            ChangeSelectedItemIndex(selectedItemIndex - 1);
    }

    void SelectedItemsSetup()
    {
        currentSelectedItems = new SelectedItem[maxSelectedItems];
        int currentIndex = 0;

        for (int i = 0; i < builder.Collection.materials.Length; ++i)
            for (int j = 0; j < builder.Collection.materials[i].materials.Length; ++j)
            {
                currentSelectedItems[currentIndex++] = new SelectedItem(new Level.Index(builder.Collection.materials[i].setName, builder.Collection.materials[i].materials[j].name), SelectedItem.ItemType.Material);
                if (currentIndex >= maxSelectedItems)
                    return;
            }

    }

    public void ChangeSelectedItem(SelectedItem item)
    {
        currentSelectedItems[selectedItemIndex] = item;
        UpdateDashboard();
        UpdateObjectToPlace();
    }


    void UpdateDashboard()
    {
        ui.dashboard.UpdateDashboardTextures(currentSelectedItems);
    }

    void ChangeSelectedItemIndex(int nextIndex)
    {
        if (nextIndex >= maxSelectedItems)
            nextIndex %= maxSelectedItems;
        if (nextIndex < 0)
            nextIndex = maxSelectedItems - Mathf.Abs(nextIndex) % maxSelectedItems;
        selectedItemIndex = nextIndex;
        ui.dashboard.ChangeSelectedItem(selectedItemIndex);
        UpdateObjectToPlace();
    }

    void UpdateObjectToPlace()
    {
        Destroy(objectToPlace);
        if (SelectedItemType == SelectedItem.ItemType.Material)
            return;

        objectToPlace = Instantiate(builder.Collection.GetObject(currentSelectedItems[selectedItemIndex].index));
        Utility.ChangeLayerFull(objectToPlace, raycastIgnoreLayer);
    }

    #endregion

    #region Lighting

    public void CycleLighting()
    {
        int currentIndex = 0;

        for (int i = 0; i < builder.Collection.levelLightings.Length; ++i)
            if (level.LevelLightingName == builder.Collection.levelLightings[i].name)
            {
                currentIndex = i;
                break;
            }

        int nextIndex = (currentIndex + 1) % builder.Collection.levelLightings.Length;
        level.SetLevelLighting(builder.Collection.levelLightings[nextIndex].name);
        builder.ApplyLighting(level);
    }

    #endregion

    #region Saving and Loading

    public void SetLevelToLoad(string filePath)
    {
        ConfirmLoad = true;
        LevelToLoad = filePath;
    }

    public void SaveLevel(string levelName)
    {
        foreach(LevelEditorObject obj in addedObjects)
            level.AddObject(obj.objectInfo);
        addedObjects = new List<LevelEditorObject>();

        LevelLoader.SaveLevel(level, levelName);
    }

    public void LoadLevel(string levelName,bool directPath)
    {
        Level loadedLevel = LevelLoader.LoadLevel(levelName, directPath);
        if (loadedLevel == null)
            return;

        level = loadedLevel;
        builder.BuildLevel(loadedLevel, true);
        previousActions.Clear();
        redoActions.Clear();
    }

    #endregion
}
