using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalHealthAltar : Interactable
{
    public float rechargeTime = 60;
    public float healAmount = 15;
    private float timeCounter;
    public string promptLocalizationKey = "survivalHeal";

    [Space]
    public Text uiTimeCounter;
    public Image uiFillImage;
    [Space]
    public Color indicatorColor = Color.green;
    public AudioClip healAudioClip;


    public override bool CanInteract()
    {
        return timeCounter >= rechargeTime;
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        return Localizer.Localize(promptLocalizationKey);
    }

    protected override void _Interact(Character interactingCharacter)
    {
        interactingCharacter.GiveHealth(healAmount);
        (interactingCharacter as PlayerCharacter).PickupIndicator(indicatorColor);
        Audio.PlaySFX(healAudioClip, transform.position, null, 0.2f);
        timeCounter = 0;
    }


    void Update()
    {
        if (!CanInteract())
            timeCounter += Time.deltaTime;

        uiFillImage.fillAmount = Mathf.Clamp(timeCounter / rechargeTime, 0, 1);
        uiTimeCounter.text = ((int)(rechargeTime - timeCounter)).ToString();
    }
}
