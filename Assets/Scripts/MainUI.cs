using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    public enum ActiveUI
    {
        none,
        newgame,
        loadgame,
        settings,
        credits
    }
    public ActiveUI activeInterface;
    private bool NewGameActivated;
    public void NewGameSetup()
    {
        CloseOldUI();
        if (!NewGameActivated)
        {
            NewGameActivated = true;
        }
        else
        {

        }
    }
    public void LoadGame()
    {

    }
    public void OpenSettings()
    {

    }
    public void QuitGame()
    {
        
    }
    public void OpenCredits()
    {
        
    }
    public void OpenDiscord()
    {
        Application.OpenURL("");
    }

    void CloseOldUI()
    {

    }

}
