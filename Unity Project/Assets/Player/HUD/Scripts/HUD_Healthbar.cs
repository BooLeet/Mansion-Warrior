using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Healthbar : MonoBehaviour
{
    public Image healthFill;
    public Image damageEffectFill;
    public Text[] healthCounters;

    private void Update()
    {
        DamageEffectUpdate();
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthFill.fillAmount = currentHealth / maxHealth;
        foreach(Text healthCounter in healthCounters)
            healthCounter.text = ((int)currentHealth).ToString();
    }

    private void DamageEffectUpdate()
    {
        damageEffectFill.fillAmount = Mathf.Lerp(damageEffectFill.fillAmount, healthFill.fillAmount, Time.deltaTime * 5);
    }
}
