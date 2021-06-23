using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ability : MonoBehaviour
{
    public Image abilityChargeFill;

    void Start()
    {
        
    }


    public void UpdateGraphic(PlayerCharacter player)
    {
        abilityChargeFill.fillAmount = player.AbilityCharge / player.stats.abilityMaxCharge;
    }
}
