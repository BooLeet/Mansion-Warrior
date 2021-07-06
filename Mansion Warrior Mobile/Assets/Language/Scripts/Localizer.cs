using UnityEngine;

public class Localizer : MonoBehaviour
{
    public static Localizer Instance { get; private set; }
    public LanguagePack languagePack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static string Localize(string key)
    {
        if (Instance != null && Instance.languagePack != null)
            return Instance.languagePack.GetString(key);

        return key;
    }


    public static string LocalizeKeyCode(KeyCode keyCode)
    {
        string str;
        switch (keyCode)
        {
            case KeyCode.Mouse0: str = Localize("lmb"); break;
            case KeyCode.Mouse1: str = Localize("rmb"); break;
            case KeyCode.Mouse2: str = Localize("mmb"); break;
            default: str = keyCode.ToString(); break;
        }

        return "[" + str + "]";
    }
}
