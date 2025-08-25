using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameUI : MonoBehaviour
{
    public void ToggleEnlargeGalaxyMapPreview()
    {

    }
    public void CloseUIScene()
    {
        MainUI main = GameObject.Find("EventSystem").GetComponent<MainUI>();
        main.newGameOpen = false;
        SceneManager.UnloadSceneAsync(1);
    }
}
