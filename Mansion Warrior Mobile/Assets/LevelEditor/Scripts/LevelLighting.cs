using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelEditor/Lighting")]
public class LevelLighting : ScriptableObject
{
    public Material skyboxMaterial;
    [Space]
    [ColorUsage(true, true)]
    public Color skyColor = Color.black;
    [ColorUsage(true, true)]
    public Color equatorColor = Color.black;
    [ColorUsage(true, true)]
    public Color groundColor = Color.black;
    [Space]
    public bool singleColor = false;
    [ColorUsage(true, true)]
    public Color fullColor = Color.black;
    [Space]
    public bool fog = false;
    public Color fogColor;
    public float fogStart = 0;
    public float fogEnd = 500;

    public void ApplyLighting()
    {
        if (singleColor)
        {
            RenderSettings.ambientSkyColor = fullColor;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        }
        else
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;

            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = equatorColor;
            RenderSettings.ambientGroundColor = groundColor;
        }
        

        RenderSettings.skybox = skyboxMaterial;
        RenderSettings.fog = fog;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogStartDistance = fogStart;
        RenderSettings.fogEndDistance = fogEnd;
    }
}
