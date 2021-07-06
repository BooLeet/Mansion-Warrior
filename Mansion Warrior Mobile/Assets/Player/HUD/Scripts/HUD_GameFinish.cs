using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_GameFinish : MonoBehaviour
{
    public RectTransform textTransform;
    public AudioClip audioClip;

    public void StartEffect()
    {
        gameObject.SetActive(true);
        Audio.PlaySFX(audioClip, Vector3.zero, null, 0);
    }

}
