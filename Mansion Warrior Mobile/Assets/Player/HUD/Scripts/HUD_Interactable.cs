using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Interactable : MonoBehaviour
{
    public Camera cam;
    public Canvas canvas;
    public RectTransform rectTransform;
    public Text prompt;

    private void ShowHide(bool show)
    {
        gameObject.SetActive(show);
    }

    public void UpdatePrompt(PlayerCharacter player)
    {
        if (!player.CurrentInteractable)
        {
            ShowHide(false);
            return;
        }

        prompt.text = player.input.GetInteractionKey() + " " + player.CurrentInteractable.GetPrompt(player);
        ShowHide(true);
        Vector3 targetPosition = cam.WorldToScreenPoint(player.CurrentInteractable.ButtonPosition) - new Vector3(cam.pixelWidth, cam.pixelHeight, 0) / 2;
        rectTransform.localPosition = targetPosition / canvas.scaleFactor;
    }
}
