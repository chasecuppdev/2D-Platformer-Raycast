using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired.UI.ControlMapper;

public class TitleScreenController : MonoBehaviour
{
    public ControlMapper controlMapper;
    public Button defaultSelection;

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
