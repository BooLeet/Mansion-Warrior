using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_PickupIndicator : MonoBehaviour
{
    public RawImage image;
    public float effectTime = 0.5f;
    private float timeCounter = 0;
    private Color currentFullColor = Color.clear;

    private void Update()
    {
        if (timeCounter <= 0)
        {
            image.enabled = false;
            return;
        }

        image.color = Color.Lerp(Color.clear, currentFullColor, timeCounter / effectTime);
        timeCounter -= Time.deltaTime;
    }

    public void StartEffect(Color color)
    {
        image.enabled = true;
        currentFullColor = color;
        image.color = color;
        timeCounter = effectTime;
    }
}
