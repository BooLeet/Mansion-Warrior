using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Healthbar : MonoBehaviour
{
    public Image healthFill;
    public Image damageEffectFill;
    public Text[] healthCounters;
    public RawImage lowHealthIndicator;
    public float lowHealthIndicatorSpeed = 3;
    private float lowHealthIndicatorParameter = 0;

    private void Update()
    {
        DamageEffectUpdate();
        Color targetColor = lowHealthIndicator.color;
        if (healthFill.fillAmount <= 1 / 3f)
        {
            targetColor.a = Mathf.Abs(Mathf.Sin(lowHealthIndicatorParameter));
            lowHealthIndicatorParameter += Time.deltaTime * lowHealthIndicatorSpeed;
            lowHealthIndicatorParameter %= Mathf.PI;
        }
        else
        {
            targetColor.a = 0;
            lowHealthIndicatorParameter = 0;
        }
        lowHealthIndicator.color = targetColor;
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
