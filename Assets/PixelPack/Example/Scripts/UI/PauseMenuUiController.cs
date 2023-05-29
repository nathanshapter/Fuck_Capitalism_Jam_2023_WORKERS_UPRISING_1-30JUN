/**
 *  Pause menu UI.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public class PauseMenuUiController : MonoBehaviour, IUiBase
{
    public RectTransform PauseMenuButtonsContainer;
    
    private PauseMenuButton initSelectedButton;
    private PauseMenuButton currentlySelectedButton;

    private CompositeDisposable disposables = new CompositeDisposable();
    private List<PauseMenuButton> pauseMenuButtons = new List<PauseMenuButton>();

    private void Start()
    {
        pauseMenuButtons = PauseMenuButtonsContainer.GetComponentsInChildren<PauseMenuButton>().ToList();
        initSelectedButton = pauseMenuButtons.FirstOrDefault(button => button.InitiallySelect);

        //Subscribe to buttons' InternalSelected and Submitted and handle state changes
        pauseMenuButtons.ForEach(button => button.Submitted.Subscribe(b => ButtonSubmittedHandler()).AddTo(disposables));
        pauseMenuButtons.ForEach(button => button.InternalSelected.Subscribe(isSelected => { if (isSelected) currentlySelectedButton = button; } ).AddTo(disposables));
    }

    private void ButtonSubmittedHandler()
    {
        if (currentlySelectedButton.Type == PauseMenuButton.ButtonType.Resume)
        {
            Debug.Log("Resume game...");
            GameStateManager.CurrentState.Value = GameStateManager.GameState.InGame;
            Show(false);            
        }
        else if (currentlySelectedButton.Type == PauseMenuButton.ButtonType.Restart)
        {
            Debug.Log("Restart level...");
            Show(false);
            GameStateManager.CurrentState.Value = GameStateManager.GameState.InGame;
            ApplicationController.Instance.ResetLevel();
        }
        else if (currentlySelectedButton.Type == PauseMenuButton.ButtonType.WorldMap)
        {
            Debug.Log("Load World Map...");
            Show(false);
            GameStateManager.CurrentState.Value = GameStateManager.GameState.WorldMap;
            ApplicationController.LoadScene(GameHelper.WorldMap);
        }        
    }

    /**
    * Implement IUIBase interface to show/hide pause menu.
    * @param show Show/hide Pause menu.
    */
    public void Show(bool show)
    {
        gameObject.SetActive(show);

        //Select first button again when scene is reloaded
        if (initSelectedButton != null)        
            initSelectedButton.Select();
    }
}
