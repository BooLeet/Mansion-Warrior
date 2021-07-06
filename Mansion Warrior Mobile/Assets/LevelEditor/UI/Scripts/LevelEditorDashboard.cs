using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorDashboard : MonoBehaviour
{
    private class DashboardItem
    {
        public RawImage image;

        public DashboardItem(RawImage image)
        {
            this.image = image;
        }
    }

    public GameObject dashItemPrefab;
    public Color selectedHighlightColor;
    public Material dashboardErrorMaterial;
    public RectTransform dashboard;
    private DashboardItem[] dashboardItems;
    private int selectedItem = 0;
    [Space]
    public LevelEditorObjectSnapshot snapshotGenerator;

    private void Awake()
    {
        DashboardSetup();

        ChangeSelectedItem(0);
    }

    private void DashboardSetup()
    {
        dashboardItems = new DashboardItem[LevelEditor.maxSelectedItems];
        for (int i = 0; i < LevelEditor.maxSelectedItems; ++i)
        {
            GameObject obj = Instantiate(dashItemPrefab, dashboard);
            RawImage image = obj.GetComponentInChildren<RawImage>();
            image.color = dashboardErrorMaterial.color;
            image.texture = dashboardErrorMaterial.mainTexture;
            image.gameObject.AddComponent<Shadow>().effectDistance = new Vector2(3, -3);

            dashboardItems[i] = new DashboardItem(image);
        }
    }

    public void UpdateDashboardTextures(LevelEditor.SelectedItem[] selectedItems)
    {
        for (int i = 0; i < dashboardItems.Length; ++i)
        {
            if (selectedItems[i].type == LevelEditor.SelectedItem.ItemType.Material)
            {
                Material material = LevelEditor.Instance.builder.Collection.GetMaterial(selectedItems[i].index);
                dashboardItems[i].image.texture = material.mainTexture;
                //dashboardItems[i].image.color = material.color;
            }
            else
            {
                Texture2D texture = snapshotGenerator.GetSnapshot(LevelEditor.Instance.builder.Collection.GetObject(selectedItems[i].index));
                dashboardItems[i].image.texture = texture;
                //dashboardItems[i].image.color = Color.white;
            }
        }
    }

    public void ChangeSelectedItem(int index)
    {
        dashboardItems[selectedItem].image.rectTransform.anchoredPosition = new Vector2(0, 0);
        selectedItem = index;
        dashboardItems[selectedItem].image.rectTransform.anchoredPosition = new Vector2(0, 20);
    }
}
