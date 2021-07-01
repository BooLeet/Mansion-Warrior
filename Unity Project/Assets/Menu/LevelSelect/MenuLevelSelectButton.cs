using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MenuLevelSelectButton : MonoBehaviour
{
    public string playerSceneName;
    private string levelPath;
    public Text levelNameText;
    public Text levelScore;
    string levelName;

    public void SetLevel(string path)
    {
        levelPath = path;
        levelName = Path.GetFileNameWithoutExtension(path);
        levelNameText.text = levelName;
        SurvivalRecords.Record record = SurvivalRecords.LoadRecords().GetRecord(levelName);
        if (record != null)
            levelScore.text = record.score.ToString() + " PTS  " + GameModeSurvival.GetTimeString(record.time);
        else
            levelScore.text = Localizer.Localize("survivalNoRecord");
    }

    public void ButtonCallback()
    {
        SceneNameContainer.Instance.sceneName = playerSceneName;
        SceneNameContainer.Instance.additionalData = levelName;
        SceneNameContainer.Instance.BeginLoading();
    }
}
