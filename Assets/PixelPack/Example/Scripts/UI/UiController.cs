/**
 *  Show/hide different UI views and update in-game UI.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class UiController : MonoBehaviour
{
    //Different ui part controllers
    public IngameUiController IngameUiController;
    public MainMenuUiController MainMenuUiController;
    public PauseMenuUiController PauseMenuUiController;

    public Image FadeBlackImage;        
    private Sequence fadeBlackSequence;
    private InventoryItemData playerIsCurrentlyUsingItem;
    private CompositeDisposable disposables = new CompositeDisposable();
    
    /**
    * Subscribe to coins and time ReactiveProperties
    */
    public void Init()
    {
        PlayerCharacter.Coins.Subscribe(amount => HandleCoinsChange(amount)).AddTo(disposables);
        //This may seem like a bit weird loop, but player may lose item when taking damage from enemies:
        PlayerController.ItemCurrentlyUsed.Subscribe(item => playerIsCurrentlyUsingItem = item);
        ApplicationController.Instance.CurrentLevelTimeLeft.Subscribe(time => HandleTimeChange(time)).AddTo(disposables);
        InputController.Instance.InventoryLeft.Subscribe(x => HandleInventoryLeftRight(-x)).AddTo(disposables);
        InputController.Instance.InventoryRight.Subscribe(x => HandleInventoryLeftRight(x)).AddTo(disposables);
        InputController.Instance.InventoryUse.Subscribe(i => HandleInventoryUse(i)).AddTo(disposables);
        InputController.Instance.Cancel.Subscribe(c =>  { if (c) ShowPauseMenu(); });
    }

    /**
    * Show pause menu. Set GameState via GameStateManager   
    */
    public void ShowPauseMenu()
    {
        if (GameStateManager.CurrentState.Value == GameStateManager.GameState.InGame)
        {
            GameStateManager.CurrentState.Value = GameStateManager.GameState.Paused;
            PauseMenuUiController.Show(true);
        }
        else if (GameStateManager.CurrentState.Value == GameStateManager.GameState.Paused)
        {
            GameStateManager.CurrentState.Value = GameStateManager.GameState.InGame;
            PauseMenuUiController.Show(false);
        }
    }

    /**
    * Show correct UI view when scene changes.
    * @param _sceneName Name of the loaded scene.
    */
    public void SceneLoaded(string _sceneName)
    {
        //TODO: Music tracks shouldn't be played from here. Move music logic to app controller?
        switch (_sceneName)
        {
            case GameHelper.MainMenu:
                AudioController.Instance.PlayTrack("Menu", 1f, 1f);
                MainMenuUiController.Show(true);
                IngameUiController.Show(false);                
                EnterMainMenu();
                break;
            case GameHelper.WorldMap:
                AudioController.Instance.PlayTrack("WorldMap", 1f, .5f);
                MainMenuUiController.Show(false);
                IngameUiController.Show(false);
                break;
            default:
                bool trackIsOnPlayList = AudioController.Instance.PlayTrack(_sceneName, 1f, 1f);
                if (!trackIsOnPlayList)
                    Debug.LogWarning("Music track '" + _sceneName + "' is not on the playlist.\nAdd track named '" + _sceneName + "' to playlist object.");

                MainMenuUiController.Show(false);
                IngameUiController.Show(true);
                break;
        }
    }

    /**
    * Fade screen when entering Main menu.
    */
    public void EnterMainMenu()
    {
        //NOTE: for reasons known only to Unity developers, does not work with IEnumerator variable...
        StopCoroutine("EnterMainMenuSequence");        
        SetBlackImageAlpha(1f);
        FadeBlackImage.gameObject.SetActive(true);
        StartCoroutine("EnterMainMenuSequence");
    }

    IEnumerator EnterMainMenuSequence()
    {
        Debug.Log("Main menu sequence...");
        yield return new WaitForSeconds(1f);
        FadeBlackOut(1f);
        yield return new WaitForSeconds(1f);
        MainMenuUiController.OpenCurtains();
    }

    /**
    * Fade screen to black with DOTween's DOFade.
    */
    public void FadeBlackIn(float _time)
    {
        SetBlackImageAlpha(0);
        FadeBlackImage.gameObject.SetActive(true);
        FadeBlackImage.DOFade(1f, _time);
    }

    /**
    * Fade screen out from black with DOTween's DOFade.
    */
    public void FadeBlackOut(float _time, float _fromAlpha = 1f)
    {
        SetBlackImageAlpha(1f);
        FadeBlackImage.gameObject.SetActive(true);
        FadeBlackImage.DOFade(0f, _time).OnComplete(() => FadeBlackImage.gameObject.SetActive(false));
    }

    /**
    * Dip to black fade with DOTween's DOFade.
    */
    public void DipToBlack(float _fadeInTime, float _fadeOutTime, float _stayTime = 0f)
    {
        SetBlackImageAlpha(0);
        FadeBlackImage.gameObject.SetActive(true);

        fadeBlackSequence = DOTween.Sequence();
        fadeBlackSequence.Append(FadeBlackImage.DOFade(1f, _fadeInTime));
        fadeBlackSequence.Insert(_fadeInTime + _stayTime, FadeBlackImage.DOFade(0f, _fadeOutTime));
        fadeBlackSequence.OnComplete(() => FadeBlackImage.gameObject.SetActive(false));
    }

    private void SetBlackImageAlpha(float _alpha)
    {
        Color currentImgCol = FadeBlackImage.color;
        currentImgCol.a = _alpha;
        FadeBlackImage.color = currentImgCol;
    }

    private void HandleCoinsChange(int currentCoins)
    {
        IngameUiController.UpdateTextCoins(currentCoins);
    }

    private void HandleTimeChange(int timeLeft)
    {
        IngameUiController.UpdateTextTime(timeLeft);
    }

    private void HandleInventoryLeftRight(int leftRight)
    {
        if (leftRight != 0)
            IngameUiController.SelectItem(leftRight);
    }

    private void HandleInventoryUse(bool _down)
    {
        if (_down)
        {
            Debug.Log("playerIsCurrentlyUsingItem: " + playerIsCurrentlyUsingItem);
            Debug.Log("IngameUiController.CurrentlySelectedItem: " + IngameUiController.CurrentlySelectedItem.ItemData);
            if (playerIsCurrentlyUsingItem != null && playerIsCurrentlyUsingItem == IngameUiController.CurrentlySelectedItem.ItemData)
                Debug.Log("You are already using item: " + playerIsCurrentlyUsingItem.ItemName);
            else 
                IngameUiController.UseItem();
        }
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
