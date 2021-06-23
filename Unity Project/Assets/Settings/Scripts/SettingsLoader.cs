using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SettingsLoader
{
    public static void SaveSettings(Settings settings, string name = "settings.txt")
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + name;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, settings);
        stream.Close();
    }

    public static Settings LoadSettings(string name = "settings.txt")
    {
        string path = Application.persistentDataPath + "/" + name;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Settings settings = formatter.Deserialize(stream) as Settings;
            stream.Close();
            return settings;
        }

        return new Settings();
    }
}
