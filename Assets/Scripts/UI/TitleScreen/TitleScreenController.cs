using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired.UI.ControlMapper;

public class TitleScreenController : MonoBehaviour
{
    public ControlMapper controlMapper; //Hooked up in editor
    public Button defaultSelection; //Hooked up in editor

    public void StartGame(int sceneIndex)
    {
        SceneController.LoadScene(sceneIndex);
    }

    public void OpenControlMapper()
    {
        controlMapper.Open();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetDefaultSelection()
    {
        defaultSelection.Select();
    }
}
