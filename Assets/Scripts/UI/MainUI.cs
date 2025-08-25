using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    public bool newGameOpen;
    public GameObject loadGamePanel, settingsPanel, creditsPanel;
    public GameObject openedUI;
    public void NewGameSetup()
    {
        if (newGameOpen != true)
        {
            CloseOldUI();
            //Load scene additive; UI needs to be reassigned into the new scene, or references won't work correctly
            newGameOpen = true;
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }
    }
    public void LoadGame()
    {
        CloseOldUI();
        openedUI = loadGamePanel;
        openedUI.SetActive(true);
    }
    public void OpenSettings()
    {
        CloseOldUI();
        openedUI = settingsPanel;
        openedUI.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenCredits()
    {
        CloseOldUI();
        openedUI = creditsPanel;
        openedUI.SetActive(true);
    }
    public void OpenDiscord()
    {
        Application.OpenURL("");
    }

    //Might need a less elegant solution - something specific to the scene thats loaded. BUG: New game panel not closing when X clicked
    public void CloseOldUI()
    {
        if (openedUI != null)
        {
            if (newGameOpen)
            {
                newGameOpen = false;
            }
            else
            {
                openedUI.SetActive(false);
                openedUI = null;
            }
        }
    }
}
