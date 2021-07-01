using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public static class SettingAppliers
{
    public delegate bool ApplierStringVal(string key, string val);
    public delegate bool ApplierFloatVal(string key, float val);

    #region Audio

    public static bool ApplySFXVolume(string key, float val)
    {
        if (key != "settingSFX")
            return false;

        if (SettingReferenceContainer.Instance == null)
            return true;

        SettingReferenceContainer.Instance.sfxMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1)) * 20);
        return true;
    }

    public static bool ApplyMusicVolume(string key, float val)
    {
        if (key != "settingMusic")
            return false;

        if (SettingReferenceContainer.Instance == null)
            return true;

        SettingReferenceContainer.Instance.musicMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1)) * 20);
        return true;
    }

    #endregion

    #region Post Processing
    private static bool ApplyPostProcessing<T>(string targetKey, string key, string val) where T : PostProcessEffectSettings
    {
        if (key != targetKey)
            return false;

        if (SettingReferenceContainer.Instance == null)
            return true;

        T setting;
        SettingReferenceContainer.Instance.postProcessVolume.profile.TryGetSettings(out setting);

        if (setting)
            setting.enabled.value = val == "on";
        return true;
    }

    public static bool ApplyAmbientOcclusion(string key, string val)
    {
        return ApplyPostProcessing<AmbientOcclusion>("settingAO", key, val);
    }

    public static bool ApplyMotionBlur(string key, string val)
    {
        return ApplyPostProcessing<MotionBlur>("settingMotionBlur", key, val);
    }

    public static bool ApplyBloom(string key, string val)
    {
        return ApplyPostProcessing<Bloom>("settingBloom", key, val);
    }

    public static bool ApplyGrain(string key, string val)
    {
        return ApplyPostProcessing<Grain>("settingGrain", key, val);
    }

    public static bool ApplyAberration(string key, string val)
    {
        return ApplyPostProcessing<ChromaticAberration>("settingAberration", key, val);
    }

    public static bool ApplyFPSCounter(string key, string val)
    {
        if (key != "settingFPSCounter")
            return false;

        if (SettingReferenceContainer.Instance == null)
            return true;

        if (val == "on")
            SettingReferenceContainer.Instance.fPSCounter.Show();
        else
            SettingReferenceContainer.Instance.fPSCounter.Hide();

        return true;
    }

    #endregion

    #region Resolution

    public static bool ApplyRenderResolutionScale(string key, float val)
    {
        if (key != "settingResolutionScale")
            return false;

        if (SettingReferenceContainer.Instance == null)
            return true;

        if (SettingReferenceContainer.Instance.renderResolutionScaler != null)
            SettingReferenceContainer.Instance.renderResolutionScaler.ApplyResolutionScale(val);
        return true;
    }

    public static bool ApplyRenderResolutonFilter(string key, string val)
    {
        if (key != "settingResolutionFilter")
            return false;

        if (SettingReferenceContainer.Instance == null)
            return true;

        FilterMode mode = FilterMode.Point;
        if (val == "bilinear")
            mode = FilterMode.Bilinear;
        else if (val == "trilinear")
            mode = FilterMode.Trilinear;

        if(SettingReferenceContainer.Instance.renderResolutionScaler != null)
            SettingReferenceContainer.Instance.renderResolutionScaler.ApplyFilterMode(mode);
        return true;
    }

    #endregion

    public static bool ApplyFOV(string key, float val)
    {
        if (key != "settingFOV")
            return false;

        if (SettingReferenceContainer.Instance == null)
            return true;

        if(SettingReferenceContainer.Instance.applyCameraFov)
            SettingReferenceContainer.Instance.mainCamera.fieldOfView = val;
        return true;
    }
}
