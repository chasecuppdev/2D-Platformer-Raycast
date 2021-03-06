﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;
using UnityEngine.UI;
using Rewired;
using Rewired.UI.ControlMapper;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    Canvas pauseMenu;
    [SerializeField]
    Canvas dimCanvas; //Hooked up in editor
    [SerializeField]
    PlayerInput playerInput; //Getting reference to player input so we can enable/disable controller maps as necessary. Hooked up in editor
    [SerializeField]
    AudioSource music; //Hooked up in editor
    public Button defaultSelection; //Hooked up in editor
    public ControlMapper controlMapper; //Hooked up in editor
    public EventSystem eventSystem;
    public bool isPaused = false;

    private void Start()
    {
        pauseMenu = GetComponent<Canvas>();
        MessageKit.addObserver(EventTypes.UI_PAUSE_MENU, TogglePauseMenu);
        EnableGameplayControls();
        DisableUIControls();
    }

    public void SetDefaultSelection()
    {
        defaultSelection.Select();
    }

    public void TogglePauseMenu()
    {
        if (!controlMapper.isOpen)
        {
            if (!pauseMenu.enabled)
            {
                isPaused = true;
                music.Pause();
                SetDefaultSelection();
                defaultSelection.interactable = true;
                //EnableUIControls();
                //DisableGameplayControls();
                Time.timeScale = 0;
                dimCanvas.enabled = true;
                pauseMenu.enabled = true;
            }
            else
            {
                isPaused = false;
                music.UnPause();
                //EnableGameplayControls();
                //DisableUIControls();
                defaultSelection.interactable = false;
                Time.timeScale = 1;
                dimCanvas.enabled = false;
                pauseMenu.enabled = false;
            }
        }
    }

    IEnumerator ArtificialLoadTime(float duration)
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(duration);
        SceneController.LoadTitleScene();
    }

    public void QuitToMainMenu()
    {
        //We have to do this because the scene loads so fast most of the time the user is still pressing the submit button and this input carries over to the title scene,
        //causing the game to restart instantly since it is the first selected button
        StartCoroutine(ArtificialLoadTime(0.5f)); 
    }

    public void OpenControlMapper()
    {
        controlMapper.Open();
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
