using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static Menu Instance { get; private set; }
    private List<MenuSet> menuSets;
    public MenuSet startingSet;
    public GameObject mainPanel;

    private Stack<MenuSet> setStack;
    public bool canHide = false;
    public bool isHiddenByDefault = false;
    public bool pauseOnShow = true;
    public static bool IsHidden { get { return !Instance.mainPanel.activeSelf; } }
    public bool CanShow { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        setStack = new Stack<MenuSet>();
        menuSets = new List<MenuSet>();
        CanShow = true;
    }

    private void Update()
    {
        if (CanShow && GameManager.Instance.ListenForPause && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape) || (PlayerCharacter.Instance != null && PlayerCharacter.Instance.input.GetPause())))
        {
            if (IsHidden)
                Show();
            else
                GoToPreviousSet();
        } 
    }

    private void Start()
    {
        NextSet(startingSet);
        if (isHiddenByDefault)
            Hide();
        else
            Show();
    }

    public static void RegisterSet(MenuSet set)
    {
        if (!Instance)
            return;

        Instance.menuSets.Add(set);
    }

    public static void UnegisterSet(MenuSet set)
    {
        if (!Instance)
            return;

        Instance.menuSets.Remove(set);
    }

    public static void NextSet(MenuSet nextSet)
    {
        if (!Instance)
            return;

        if (Instance.setStack.Count > 0 && Instance.setStack.Peek() == nextSet)
            return;

        Instance.setStack.Push(nextSet);
        Instance.ShowSet(nextSet);
    }

    public static void GoToPreviousSet()
    {
        if (!Instance)
            return;

        if (Instance.setStack.Count <= 1)
        {
            if (Instance.canHide)
                Hide();
            return;
        }
            
        Instance.setStack.Pop();
        Instance.ShowSet(Instance.setStack.Peek());
    }

    private void ShowSet(MenuSet setToShow)
    {
        foreach (MenuSet set in menuSets)
            set.gameObject.SetActive(false);

        setToShow.gameObject.SetActive(true);
    }

    public static void Show()
    {
        if (Instance == null)
            return;

        Instance.mainPanel.SetActive(true);
        Utility.EnableCursor();
        if (Instance.pauseOnShow)
            GameManager.Pause();
    }

    public static void Hide()
    {
        if (Instance == null)
            return;

        Instance.mainPanel.SetActive(false);
        Utility.DisableCursor();
        if (Instance.pauseOnShow)
            GameManager.Unpause();
    }
}
