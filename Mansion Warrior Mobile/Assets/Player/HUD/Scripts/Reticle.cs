using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    public Camera cam;
    public Canvas canvas;
    public RectTransform rectTransform;

    public Image[] notches;
    public Image center;
    public float distanceToCamera = 10;
    public float canvasScale = 0.01f;
    private Color targetNotchesColor = Color.white;
    private Color targetPointColor = Color.white;

    private float targetSpreadAngle = 0.01f;
    private float currentSpreadAngle = 0.01f;


    void Update()
    {
        foreach (Image image in notches) 
            image.color = Color.Lerp(image.color, targetNotchesColor, Time.deltaTime * 30);

        center.color = Color.Lerp(center.color, targetPointColor, Time.deltaTime * 30);
        SpreadAngleUpdate();
    }

    public void UpdateTarget(Entity targetedEntity)
    {
        if (!targetedEntity)
        {
            rectTransform.localPosition = Vector3.zero;
            return;
        }

        Vector3 targetPosition = cam.WorldToScreenPoint(targetedEntity.Position) - new Vector3(cam.pixelWidth, cam.pixelHeight, 0) / 2;
        rectTransform.localPosition = targetPosition / canvas.scaleFactor;
    }

    public void PointOnly(bool val)
    {
        if (val)
            targetNotchesColor = new Color(1, 1, 1, 0);
        else
            targetNotchesColor = Color.white;
    }

    public void NoPoint(bool val)
    {
        if (val)
            targetPointColor = new Color(1, 1, 1, 0);
        else
            targetPointColor = Color.white;
    }


    public void ShowHide(bool val)
    {

    }

    public void SetSpreadAngle(float spreadAngleDeg)
    {
        targetSpreadAngle = Mathf.Abs(spreadAngleDeg) / 2;
        if (targetSpreadAngle == 0)
            targetSpreadAngle = 0.01f;
    }

    private void SpreadAngleUpdate()
    {
        currentSpreadAngle = Mathf.Lerp(currentSpreadAngle, targetSpreadAngle, Time.deltaTime * 10);

        float offset = Mathf.Tan(Mathf.Deg2Rad * currentSpreadAngle) * distanceToCamera;
        offset /= canvasScale;

        foreach (Image image in notches)
        {
            image.rectTransform.anchoredPosition = image.rectTransform.anchoredPosition.normalized * offset;
        }
    }
}
