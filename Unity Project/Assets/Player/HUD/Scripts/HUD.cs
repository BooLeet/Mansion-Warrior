using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Reticle reticle;
    public Hitmarker hitmarker;
    public HUD_Ammo ammo;
    public HUD_Healthbar healthbar;
    public HUD_Interactable interactable;
    public HUD_PickupIndicator pickupIndicator;
    public HUD_Ability ability;

    public Text messageText;
    public float messageDuration = 1.5f;
    private bool messagePermanent = false;
    private float messageTimeCounter = 0;

    public GameObject[] canvasObjects;
    public Transform worldSpaceCanvas;
    public GameObject damageIndicatorPrefab;
    
    public void Show()
    {
        foreach(GameObject obj in canvasObjects)
            obj.SetActive(true);
    }

    public void Hide()
    {
        foreach (GameObject obj in canvasObjects)
            obj.SetActive(false);
    }

    public void DamageIndicator(Vector3 damageSource, PlayerCharacter player)
    {
        Instantiate(damageIndicatorPrefab, worldSpaceCanvas).GetComponent<HUD_DamageIndicator>().StartEffect(damageSource, player);
    }

    private void Start()
    {
        GameObject gameModeHUDElement = GameMode.Instance.GetHUDElement();
        if (gameModeHUDElement)
            Instantiate(gameModeHUDElement, worldSpaceCanvas);
    }

    private void Update()
    {
        if (messagePermanent)
            return;

        messageTimeCounter -= Time.deltaTime;
        if (messageTimeCounter <= 0)
            messageText.gameObject.SetActive(false);
    }

    public void UIMessage(string message, bool permanent)
    {
        messageText.text = message;
        messageTimeCounter = messageDuration;
        messagePermanent = permanent;
        messageText.gameObject.SetActive(true);
    }
}
