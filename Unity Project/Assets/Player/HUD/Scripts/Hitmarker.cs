using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitmarker : MonoBehaviour
{
    public Image[] notches;
    public RectTransform holder;
    public float fullNotchDistance = 75;
    public float effectLerpParameter = 10;
    public float maxTurnAngle;
    [Space]
    public Color hitColor, killColor;
    private Color targetColor;
    [Space]
    public AudioClip hitmarkerSound;
    public AudioClip killmarkerSound;

    void Start()
    {
        foreach (Image image in notches)
        {
            image.color = Color.clear;
            image.rectTransform.anchoredPosition = image.rectTransform.anchoredPosition.normalized * 0.01f;
        }
    }

    void Update()
    {
        foreach (Image image in notches)
        {
            image.color = (image.rectTransform.anchoredPosition.magnitude > fullNotchDistance * 2 / 3) ? targetColor : Color.clear;
            DecayAnchoredPosition(image.rectTransform);
        }

        holder.localRotation = Quaternion.Lerp(holder.localRotation, Quaternion.Euler(0, 0, -45), Time.deltaTime * effectLerpParameter);
    }

    public void StartEffect(bool kill)
    {
        if (kill)
            targetColor = killColor;
        else
            targetColor = hitColor;

        holder.localRotation = Quaternion.Euler(0, 0, -45 + Random.Range(-maxTurnAngle, maxTurnAngle));

        float distance = fullNotchDistance;
        if (kill)
            distance *= 1.5f;

        foreach (Image image in notches)
        {
            image.color = targetColor;
            image.rectTransform.anchoredPosition = image.rectTransform.anchoredPosition.normalized * distance;
        }

        Audio.PlaySFX(kill? killmarkerSound : hitmarkerSound, transform.position, transform, 0);
    }

    private void DecayAnchoredPosition(RectTransform rect)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, rect.anchoredPosition.normalized * 0.01f, Time.deltaTime * effectLerpParameter);
    }

}
