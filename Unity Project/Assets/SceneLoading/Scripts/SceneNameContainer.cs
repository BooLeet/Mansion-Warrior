using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneNameContainer : MonoBehaviour {
    public static SceneNameContainer Instance { get; private set; }
    public string loadingSceneName = "LoadingScene";
    public string sceneName;

    void Awake()
    {
        if(Instance == null)
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

    public void BeginLoading()
    {
        SceneManager.LoadSceneAsync(loadingSceneName);
    }
}
