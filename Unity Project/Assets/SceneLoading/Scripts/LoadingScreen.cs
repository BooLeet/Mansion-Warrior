using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {
    public string defaultSceneName = "MainMenuScene";
    public SceneLoader sceneLoader;

	void Start () {
        if (SceneNameContainer.Instance) 
            sceneLoader.LoadScene(SceneNameContainer.Instance.sceneName);
        else
            sceneLoader.LoadScene(defaultSceneName);
    }
}
