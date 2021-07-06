using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SurvivalRecords
{
    [System.Serializable]
    public class Record
    {
        public string mapName;
        public float time;
        public int score;

        public Record(string mapName, float time, int score)
        {
            this.mapName = mapName;
            this.time = time;
            this.score = score;
        }
    }

    private List<Record> records;

    public SurvivalRecords()
    {
        records = new List<Record>();
    }

    public Record GetRecord(string mapName)
    {
        foreach (Record currentRecord in records)
            if (currentRecord.mapName == mapName)
                return currentRecord;
        return null;
    }

    public void UpdateRecords(Record record, out bool newTimeRecord,out bool newScoreRecord)
    {
        newTimeRecord = true;
        newScoreRecord = true;
        bool found = false;
        foreach (Record currentRecord in records)
            if (currentRecord.mapName == record.mapName)
            {
                if (record.time > currentRecord.time)
                    currentRecord.time = record.time;
                else
                    newTimeRecord = false;

                if (record.score > currentRecord.score)
                    currentRecord.score = record.score;
                else
                    newScoreRecord = false;

                found = true;
                break;
            }

        if (!found)
            records.Add(record);

        Save();
    }

    public static SurvivalRecords LoadRecords(string name = "records.txt")
    {
        string path = Application.persistentDataPath + "/" + name;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SurvivalRecords records = formatter.Deserialize(stream) as SurvivalRecords;
            stream.Close();
            return records;
        }

        return new SurvivalRecords();
    }

    public void Save(string name = "records.txt")
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + name;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, this);
        stream.Close();
    }
}
