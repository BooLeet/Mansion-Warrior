using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class LevelLoader
{
    public static void SaveLevel(Level level, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + "/" + name + ".level";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, level);
        stream.Close();
    }

    public static Level LoadLevel(string name, bool directPath)
    {
        string path;
        if (directPath)
            path = name;
        else
            path = Application.dataPath + "/" + name + ".level";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Level level = formatter.Deserialize(stream) as Level;
            stream.Close();
            return level;
        }

        return null;
    }
}
