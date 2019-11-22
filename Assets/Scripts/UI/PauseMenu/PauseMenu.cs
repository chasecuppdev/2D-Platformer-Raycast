using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;
using UnityEngine.UI;
using Rewired;

public class PauseMenu : MonoBehaviour
{
    Canvas pauseMenu;
    [SerializeField]
    PlayerInput playerInput; //Getting reference to player input so we can enable/disable controller maps as necessary. Hooked up in editor
    public Button defaultSelection; //Hooked up in editor

    private void Start()
    {
        pauseMenu = GetComponent<Canvas>();
        MessageKit.addObserver(EventTypes.UI_PAUSE_MENU, TogglePauseMenu);
        //EnableGameplayControls();
        //DisableUIControls();
    }

    public void SetDefaultSelection()
    {
        defaultSelection.Select();
    }

    public void TogglePauseMenu()
    {
        if (!pauseMenu.enabled)
        {
            SetDefaultSelection();
            EnableUIControls();
            DisableGameplayControls();
            Time.timeScale = 0;
            pauseMenu.enabled = true;
        }
        else
        {
            EnableGameplayControls();
            DisableUIControls();
            Time.timeScale = 1;
            pauseMenu.enabled = false;
        }
    }

    public void QuitToMainMenu()
    {
        EnableUIControls();
        DisableGameplayControls();
        Time.timeScale = 1;
        SceneController.LoadTitleScene();
    }

    private void EnableUIControls()
    {
        playerInput.rewiredPlayer.controllers.maps.SetMapsEnabled(true, "UI"); //UI is categoryId 1
    }

    private void DisableUIControls()
    {
        playerInput.rewiredPlayer.controllers.maps.SetMapsEnabled(false, "UI"); //UI is categoryId 1
    }

    private void EnableGameplayControls()
    {
        playerInput.rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Default"); //The default map is categoryId 0 and is used for the game actions
    }

    private void DisableGameplayControls()
    {
        playerInput.rewiredPlayer.controllers.maps.SetMapsEnabled(false, "Default"); //The default map is categoryId 0 and is used for the game actions
    }
}
