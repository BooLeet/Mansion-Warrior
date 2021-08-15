using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Settings
{
    #region Containers
    [System.Serializable]
    public class SettingStringValue
    {
        public string key;
        public string value;

        public SettingStringValue(string key, string val)
        {
            this.key = key;
            value = val;
        }
    }

    [System.Serializable]
    public class SettingFloatValue
    {
        public string key;
        public float value;

        public SettingFloatValue(string key, float val)
        {
            this.key = key;
            value = val;
        }
    }

    [System.Serializable]
    public class SettingKeyCodeValue
    {
        public string key;
        public KeyCode value;

        public SettingKeyCodeValue(string key, KeyCode val)
        {
            this.key = key;
            value = val;
        }
    }

    private static SettingAppliers.ApplierStringVal[] stringValueAppliers =
        {
        SettingAppliers.ApplyAmbientOcclusion,
        SettingAppliers.ApplyMotionBlur,
        SettingAppliers.ApplyBloom,
        SettingAppliers.ApplyGrain,
        SettingAppliers.ApplyAberration,
        SettingAppliers.ApplyFPSCounter,
        SettingAppliers.ApplyRenderResolutonFilter
    };

    private static SettingAppliers.ApplierFloatVal[] floatValueAppliers =
        {
        SettingAppliers.ApplySFXVolume,
        SettingAppliers.ApplyMusicVolume,
        SettingAppliers.ApplyRenderResolutionScale,
        SettingAppliers.ApplyFOV
    };


    #endregion

    #region Data

    private List<SettingStringValue> stringValueSettings = new List<SettingStringValue>();
    private List<SettingFloatValue> floatValueSettings = new List<SettingFloatValue>();
    private List<SettingKeyCodeValue> keyCodeValueSettings = new List<SettingKeyCodeValue>();

    #endregion

    public Settings()
    {
        stringValueSettings = GetDefaultStringValues();
        keyCodeValueSettings = GetDefaultKeys();
        floatValueSettings = GetDefaultFloatValues();
    }

    private static List<SettingKeyCodeValue> GetDefaultKeys()
    {
        List<SettingKeyCodeValue> keys = new List<SettingKeyCodeValue>
        {
            new SettingKeyCodeValue("forward", KeyCode.W),
            new SettingKeyCodeValue("back", KeyCode.S),
            new SettingKeyCodeValue("right", KeyCode.D),
            new SettingKeyCodeValue("left", KeyCode.A),
            new SettingKeyCodeValue("sprint", KeyCode.LeftShift),

            new SettingKeyCodeValue("fire", KeyCode.Mouse0),
            new SettingKeyCodeValue("ability", KeyCode.V),
            new SettingKeyCodeValue("reload", KeyCode.R),
            new SettingKeyCodeValue("melee", KeyCode.Mouse1),
            new SettingKeyCodeValue("swapWeapon", KeyCode.Q),
            new SettingKeyCodeValue("interact", KeyCode.F),
            new SettingKeyCodeValue("slide", KeyCode.C),
            new SettingKeyCodeValue("jump", KeyCode.Space),
            new SettingKeyCodeValue("inspect", KeyCode.I),
            
        };

        return keys;
    }

    private static List<SettingFloatValue> GetDefaultFloatValues()
    {
        //
        List<SettingFloatValue> floats = new List<SettingFloatValue>
        {
            new SettingFloatValue("settingMouseSensitivity",1.5f),
            new SettingFloatValue("settingSFX",0.5f),
            new SettingFloatValue("settingMusic",0.5f),
            new SettingFloatValue("settingResolutionScale",0.5f),
            new SettingFloatValue("settingFOV",70),
            };
        return floats;
    }

    private static List<SettingStringValue> GetDefaultStringValues()
    {
        List<SettingStringValue> strings = new List<SettingStringValue>
        {
            new SettingStringValue("settingResolutionFilter","point"),
            new SettingStringValue("settingAO","off"),
            new SettingStringValue("settingMotionBlur","off"),
            new SettingStringValue("settingBloom","off"),
            new SettingStringValue("settingGrain","off"),
            new SettingStringValue("settingAberration","off"),
            new SettingStringValue("settingFPSCounter","off"),
            new SettingStringValue("settingButtonHelper","on"),
            new SettingStringValue("settingHoldSprint","on"),
            new SettingStringValue("settingAlwaysSprint","off"),
        };
        return strings;
    }

    #region Changing
    public static void ChangeStringValueSetting(string key, string val)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Settings._ChangeStringValueSetting(key, val);
    }

    public static void ChangeFloatValueSetting(string key, float val)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Settings._ChangeFloatValueSetting(key, val);
    }

    public static void ChangeKeyCodeValueSetting(string key, KeyCode val)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Settings._ChangeKeyCodeValueSetting(key, val);
    }

    private void _ChangeStringValueSetting(string key, string val)
    {
        var found = from setting in stringValueSettings
                    where setting.key == key
                    select setting;

        SettingStringValue newSetting;

        if (found.Count() == 0)
        {
            newSetting = new SettingStringValue(key, val);
            stringValueSettings.Add(newSetting);
        }
        else
        {
            found.First().value = val;
            newSetting = found.First();
        }
        ApplyStringValueSetting(newSetting);
        SettingsLoader.SaveSettings(this);
    }

    private void _ChangeFloatValueSetting(string key, float val)
    {
        var found = from setting in floatValueSettings
                    where setting.key == key
                    select setting;

        SettingFloatValue newSetting;

        if (found.Count() == 0)
        {
            newSetting = new SettingFloatValue(key, val);
            floatValueSettings.Add(newSetting);
        }
        else
        {
            found.First().value = val;
            newSetting = found.First();
        }
        ApplyFloatValueSetting(newSetting);
        SettingsLoader.SaveSettings(this);
    }

    private void _ChangeKeyCodeValueSetting(string key, KeyCode val)
    {
        var found = from setting in keyCodeValueSettings
                    where setting.key == key
                    select setting;

        SettingKeyCodeValue newSetting;

        if (found.Count() == 0)
        {
            newSetting = new SettingKeyCodeValue(key, val);
            keyCodeValueSettings.Add(newSetting);
        }
        else
        {
            found.First().value = val;
            newSetting = found.First();
        }
        SettingsLoader.SaveSettings(this);
    }

    public static void ResetKeys()
    {
        GetSettings()._ResetKeys();
    }

    private void _ResetKeys()
    {
        keyCodeValueSettings = GetDefaultKeys();
    }


    #endregion

    #region Applying
    public void ApplyAllSettings()
    {
        foreach (SettingStringValue setting in stringValueSettings)
            ApplyStringValueSetting(setting);

        foreach (SettingFloatValue setting in floatValueSettings)
            ApplyFloatValueSetting(setting);
    }

    private void ApplyStringValueSetting(SettingStringValue setting)
    {
        foreach (var applier in stringValueAppliers)
            if (applier(setting.key, setting.value))
                return;
    }

    private void ApplyFloatValueSetting(SettingFloatValue setting)
    {
        foreach (var applier in floatValueAppliers)
            if (applier(setting.key, setting.value))
                return;
    }

    #endregion

    #region Getters

    public static Settings GetSettings()
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.Settings;
        return null;
    }

    public static SettingStringValue GetStringValueSetting(string key)
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.Settings._GetStringValueSetting(key);
        return null;
    }

    private SettingStringValue _GetStringValueSetting(string key)
    {
        var found = from setting in stringValueSettings
                    where setting.key == key
                    select setting;

        if (found.Count() == 0)
            return null;

        return found.First();
    }

    public static SettingFloatValue GetFloatValueSetting(string key)
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.Settings._GetFloatValueSetting(key);
        return null;
    }

    private SettingFloatValue _GetFloatValueSetting(string key)
    {
        var found = from setting in floatValueSettings
                    where setting.key == key
                    select setting;

        if (found.Count() == 0)
            return null;

        return found.First();
    }

    public static KeyCode GetKeyCode(string key)
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.Settings._GetKeyCode(key);
        return KeyCode.None;
    }

    private KeyCode _GetKeyCode(string key)
    {
        var found = from setting in keyCodeValueSettings
                    where setting.key == key
                    select setting.value;

        if (found.Count() == 0)
            return KeyCode.None;

        return found.First();
    }

    public static List<SettingKeyCodeValue> GetKeyCodeList()
    {
        Settings settings = GetSettings();
        if (settings == null)
            return GetDefaultKeys();

        if (settings.keyCodeValueSettings == null || settings.keyCodeValueSettings.Count == 0)
            settings.keyCodeValueSettings = GetDefaultKeys();

        return settings.keyCodeValueSettings;
    }

    #endregion
}
