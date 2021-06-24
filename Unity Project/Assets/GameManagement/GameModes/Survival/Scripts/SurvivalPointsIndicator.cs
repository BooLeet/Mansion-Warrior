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
    public Text time;
    private GameModeSurvival gameMode;

    void Start()
    {
        gameMode = GameMode.Instance as GameModeSurvival;
    }


    void Update()
    {
        time.text = gameMode.GetTimeString();

        points.text = gameMode.PTS.ToString();
        multiplier.text = "x" + gameMode.PTSMultiplier.ToString();
        multiplierFill.fillAmount = (gameMode.PTSMultiplierMeter % gameMode.PTSMultiplierPerStage) / gameMode.PTSMultiplierPerStage;
    }
}
