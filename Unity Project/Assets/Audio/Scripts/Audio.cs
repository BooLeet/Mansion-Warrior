using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Audio
{
    public static void PlaySFX(AudioClip clip, Vector3 position, Transform parent, float spatialBlend)
    {
        Utility.PlayAudioClipAtPoint(clip, position, parent, spatialBlend, SettingReferenceContainer.Instance.sfxMixer.FindMatchingGroups("Master")[0]);
    }

}
