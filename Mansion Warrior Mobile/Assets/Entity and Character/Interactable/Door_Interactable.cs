using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Interactable : Interactable
{
    public string localizationKey = "doorOpen";
    public Animator animator;
    private bool open = false;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";

    public override bool CanInteract()
    {
        return !open;
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        return Localizer.Localize(localizationKey);
    }

    protected override void _Interact(Character interactingCharacter)
    {
        animator.SetTrigger(openTrigger);
        open = true;
    }
}
