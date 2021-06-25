using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalPointsIndicator : MonoBehaviour
{
    public Text multiplier;
    public Image multiplierFill;
    [Space]
    public Text points;
    public Text pointsRecord;
    public Text time;
    public Text timeRecord;
    private GameModeSurvival gameMode;

    void Start()
    {
        gameMode = GameMode.Instance as GameModeSurvival;
        SurvivalRecords.Record record = gameMode.GetRecord();

        int recordPoints = 0;
        float recordTime = 0;
        if (record != null)
        {
            recordPoints = record.score;
            recordTime = record.time;
        }

        pointsRecord.text = recordPoints.ToString();
        timeRecord.text = gameMode.GetTimeString(recordTime);
    }


    void Update()
    {
        time.text = gameMode.GetTimeString(gameMode.TimeCounter);

        points.text = gameMode.PTS.ToString();
        multiplier.text = "x" + gameMode.PTSMultiplier.ToString();
        multiplierFill.fillAmount = (gameMode.PTSMultiplierMeter % gameMode.PTSMultiplierPerStage) / gameMode.PTSMultiplierPerStage;
    }
}
