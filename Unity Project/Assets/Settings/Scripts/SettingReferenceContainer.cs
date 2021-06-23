using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class SettingReferenceContainer : MonoBehaviour
{
    public static SettingReferenceContainer Instance { get; private set; }

    public PostProcessVolume postProcessVolume;
    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;
    public FPSCounter fPSCounter;
    public RenderResolutionScaler renderResolutionScaler;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        Settings.GetSettings().ApplyAllSettings();
    }
}
