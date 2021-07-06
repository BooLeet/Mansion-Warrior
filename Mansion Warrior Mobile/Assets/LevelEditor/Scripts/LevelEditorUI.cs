using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    public LevelEditor levelEditor;
    public LevelEditorDashboard dashboard;
    public LevelEditorObjectSnapshot snapshotGenerator;

    [Header("Material Selector")]
    public GameObject materialSelector;
    public RectTransform materialSelectorContent;
    public GameObject materialSelectorButton;

    [Header("Object Selector")]
    public GameObject objectSelector;
    public RectTransform objectSelectorContent;
    public GameObject objectSelectorButton;

    [Header("Save Screen")]
    public GameObject saveScreen;
    public InputField inputField;

    [Header("Load Screen")]
    public GameObject loadScreen;
    public RectTransform levelSelectorContent;
    public GameObject levelButtonPrefab;

    private void Awake()
    {
        FillMaterialSelector();
    }

    public void ShowDashboard()
    {
        dashboard.gameObject.SetActive(true);
    }

    public void HideDashboard()
    {
        dashboard.gameObject.SetActive(false);
    }

    #region Material Selector

    public void ShowMaterialSelector()
    {
        materialSelector.SetActive(true);
    }

    public void HideMaterialSelector()
    {
        materialSelector.SetActive(false);
    }

    private void FillMaterialSelector()
    {
        for (int i = 0; i < levelEditor.builder.Collection.materials.Length; ++i)
            for (int j = 0; j < levelEditor.builder.Collection.materials[i].materials.Length; ++j)
            {
                Level.Index materialIndex = new Level.Index(levelEditor.builder.Collection.materials[i].setName, levelEditor.builder.Collection.materials[i].materials[j].name);
                Material material = levelEditor.builder.Collection.GetMaterial(materialIndex);

                GameObject obj = Instantiate(materialSelectorButton, materialSelectorContent);
                RawImage image = obj.GetComponentInChildren<RawImage>();
                //image.color = material.color;
                image.texture = material.mainTexture;
                image.gameObject.AddComponent<Shadow>().effectDistance = new Vector2(3, -3);

                ItemSelectorButton button = obj.GetComponent<ItemSelectorButton>();
                button.SetIndex(materialIndex, LevelEditor.SelectedItem.ItemType.Material);
            }
    }

    #endregion

    #region Object Selector
    bool objectSelectorIsFilled = false;

    public void ShowObjectSelector()
    {
        if (!objectSelectorIsFilled)
            FillObjectSelector();
        objectSelector.SetActive(true);
    }

    public void HideObjectSelector()
    {
        objectSelector.SetActive(false);
    }

    private void FillObjectSelector()
    {
        objectSelectorIsFilled = true;

        for (int i = 0; i < levelEditor.builder.Collection.objects.Length; ++i)
            for (int j = 0; j < levelEditor.builder.Collection.objects[i].objects.Length; ++j)
            {
                Level.Index objectIndex = new Level.Index(levelEditor.builder.Collection.objects[i].setName, levelEditor.builder.Collection.objects[i].objects[j].name);
                Texture2D texture = snapshotGenerator.GetSnapshot(levelEditor.builder.Collection.GetObject(objectIndex));

                GameObject obj = Instantiate(materialSelectorButton, objectSelectorContent);
                RawImage image = obj.GetComponentInChildren<RawImage>();
                image.color = Color.white;
                image.texture = texture;
                image.gameObject.AddComponent<Shadow>().effectDistance = new Vector2(3, -3);

                ItemSelectorButton button = obj.GetComponent<ItemSelectorButton>();
                button.SetIndex(objectIndex, LevelEditor.SelectedItem.ItemType.Object);
            }
    }

    #endregion

    #region Save Screen

    public string GetSaveLevelName()
    {
        return inputField.text;
    }

    public void ShowSaveScreen()
    {
        saveScreen.SetActive(true);
    }

    public void HideSaveScreen()
    {
        saveScreen.SetActive(false);
    }

    #endregion

    #region Load Screen

    public void ShowLoadScreen()
    {
        loadScreen.SetActive(true);
    }

    public void HideLoadScreen()
    {
        loadScreen.SetActive(false);
    }

    public void LoadScreenUpdateLevels(string path)
    {
        Utility.RemoveChildren(levelSelectorContent);
        string[] files = System.IO.Directory.GetFiles(path, "*.level");
        foreach (string file in files)
        {
            GameObject obj = Instantiate(levelButtonPrefab, levelSelectorContent);
            LevelEditorLoadLevelButton loadLevelButton = obj.GetComponent<LevelEditorLoadLevelButton>();
            loadLevelButton.SetLevel(file);
        }
    }

    #endregion
}
