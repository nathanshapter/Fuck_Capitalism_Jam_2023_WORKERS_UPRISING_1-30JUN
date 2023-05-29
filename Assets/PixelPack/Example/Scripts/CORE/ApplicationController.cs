/**
 *  ApplicationController is a Singleton (Monobehaviour) class that instantiates and initializes required components for the game.
 *  It's not so much of "(state) manager" class than just a starting point for the application.
 *  You don't need to add this script to any GameObject, singleton instance's Init method is invoked after the game has been loaded.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UniRx;

public class ApplicationController : SingletonMono<ApplicationController>
{
    public ReactiveProperty<int> CurrentLevelTimeLeft { get; private set; } = new ReactiveProperty<int>(0);    /**< CurrentLevelTimeLeft ReactiveProperty for the UI.*/
    public UiController uiController { get; private set; }        
    public PlayerController player { get; private set; }

    private CameraFollow cam;    
    private List<PlatformerObject> resetables;
    private CompositeDisposable disposables = new CompositeDisposable();    
    private bool ignoreFirstCancel = true;
    private string currentSceneName; 
    private float endSequenceFollowTime = 1f;
    
    [RuntimeInitializeOnLoadMethod]
    static void OnInit()
    {
        Instance.Init();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {        
        Debug.Log("Scene loaded: " + loadedScene.name);

        InputController.Instance.InputEnabled = true; //Enable input controller
        currentSceneName = loadedScene.name; //Store current scene's name        

        if (currentSceneName == GameHelper.MainMenu)
            GameStateManager.CurrentState.Value = GameStateManager.GameState.MainMenu;
        else if (currentSceneName == GameHelper.WorldMap)
            GameStateManager.CurrentState.Value =  GameStateManager.GameState.WorldMap;
        else
            GameStateManager.CurrentState.Value = GameStateManager.GameState.InGame;

        player.SetEnabled(currentSceneName != GameHelper.MainMenu && currentSceneName != GameHelper.WorldMap); //Disable player in main menu and world map        
        uiController.SceneLoaded(currentSceneName); //Tell UI what to do with recently loaded scene
        cam.UpdateCameraConstraints(); //Update camera constraints for loaded scene
        GetSceneResetables(); //Get and update resetable objects from loaded scene

        PlayerSpawnPoint spawnPoint = GetPlayerSpawnPoint();
        player.UpdateInitPosition(spawnPoint.transform.position); //Update initPosition to new scene's spawnPoint for reset
        player.WarpToSpawnPoint(spawnPoint); //Warp player and camera to spawnPoint pos

        //Reset CurrentLevelTimeLeft
        CurrentLevelTimeLeft.Value = GameHelper.DefaultLevelCompletionTime;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        disposables.Dispose();
    }

    /**
    * Initialize game. Instantiate player, ui, set camera reference, subsribe to cancel input.
    */
    public void Init()
    {
        //Instantiate and assign values for player and UI prefabs from Resources
        player = InstantiatePlayer();
        uiController = InstantiateUiPrefab();
        //Get player cam reference
        cam = player.cam;

        //Player, his camera and ui are persistent, don't destroy on load
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(uiController);
        DontDestroyOnLoad(cam);

        if (player != null)
        {
            //Init UiController: pass player reference(s)        
            uiController.Init();

            //Subscribe to player's death animation's completion. Reset level when player has finished dying:
            player.DeathCompleted.Subscribe(b => ResetLevel()).AddTo(disposables);
        }

        //Initialize time manager
        TimeManager.Instance.Init();

        //Reactive subsriptions
        InputController.Instance.Cancel.Subscribe(cancel => HandleCancelInput(cancel)).AddTo(disposables); //esc, pad B

        //Manually call OnSceneLoaded for the first time because it doesn't trigger when game is launched
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        //Start simple level timer
        StartCoroutine(DecrementTime());
    }

    /**
    * Load scene with SceneManager.
    * @param sceneName Scene name.
    */
    public static void LoadScene(string sceneName)
    {        
        SceneManager.LoadScene(sceneName);
    }

    /**
    * Reset level.
    */
    public void ResetLevel()
    {
        //Reset all scene objects when player's death animation is completed
        resetables.ForEach(obj => obj.Reset());

        //Reset time as well
        CurrentLevelTimeLeft.Value = GameHelper.DefaultLevelCompletionTime;
    }

    /**
    * Start level completion sequence co-routine.
    * @param player Reference to PlayerCharacter which triggered the EndTile.
    */
    public void StartLevelCompletionSequence(PlayerCharacter player)
    {
        StartCoroutine(LevelCompletedSequence(player));
    }

    //What esc (xbox pad B) does in different scenes/menus
    private void HandleCancelInput(bool cancelDown)
    {
        //When cancel button is released...
        if (!cancelDown && !ignoreFirstCancel)
        {
            if (GameStateManager.CurrentState.Value == GameStateManager.GameState.WorldMap)
            {
                LoadScene(GameHelper.MainMenu);
                GameStateManager.CurrentState.Value = GameStateManager.GameState.MainMenu;
            }            
        }

        //NOTE: hackish, prevent first reactive trigger from telling to load scene
        ignoreFirstCancel = false;
    }

    private void GetSceneResetables()
    {
        //Get all PlatformerObjects (objects with Reset() method)
        resetables = FindObjectsOfType<PlatformerObject>().ToList();
    }

    private UiController InstantiateUiPrefab()
    {
        GameObject uiGo = Instantiate(Resources.Load("UI")) as GameObject;
        return uiGo.GetComponent<UiController>();
    }

    private PlayerController InstantiatePlayer()
    {
        GameObject playerGo = Instantiate(Resources.Load("Player")) as GameObject;        
        return playerGo.GetComponent<PlayerController>();
    }

    private PlayerSpawnPoint GetPlayerSpawnPoint()
    {
        //Get all spawn points...
        List<PlayerSpawnPoint> spawnPointsInScene = FindObjectsOfType<PlayerSpawnPoint>().ToList();
        //..but for now, just use first and notify if there are none or more
        if (!spawnPointsInScene.Any())
            Debug.LogError("No player spawn points found! Add spawn point to scene.");
        if (spawnPointsInScene.Count > 1)
            Debug.LogError("More than one spawn point in scene, selecting first found. Is this intentional?");

        return spawnPointsInScene.FirstOrDefault();
    }

    private IEnumerator DecrementTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (CurrentLevelTimeLeft.Value > 0) 
                CurrentLevelTimeLeft.Value--;
        }
    }

    private IEnumerator LevelCompletedSequence(PlayerCharacter player)
    {
        //Disable whole input controller
        InputController.Instance.InputEnabled = false;
        //Play win jingle
        AudioController.Instance.PlayTrack("WinJingle", 1f, .2f, false);

        float sequenceStartTime = Time.time;

        while (true)
        {
            //Basically the same, if user was pressing 'right'            
            player.OverrideAxisInput(new Vector2(1f, 0));

            //Break after endSequenceFollowTime else yield FixedUpdate
            if (Time.time - sequenceStartTime > endSequenceFollowTime)
                break;
            else
                yield return new WaitForFixedUpdate();
        }

        //stop camera follow after endSequenceFollowTime
        cam.FollowPlayer = false;

        /*
        Return to main menu after end animations
        TODO: add proper level end screen with score and collected coins etc.
        */

        //Wait for Player to run off-screen
        yield return new WaitForSeconds(2.5f);

        //Wait and fade to black
        ApplicationController.Instance.uiController.DipToBlack(1f, 1f, .2f); // <- NOTE: kinda hackish uiController method call through app controller... Obfuscating.
        yield return new WaitForSeconds(1.1f);

        //re-enable cam follow
        cam.FollowPlayer = true;

        //enable input controller
        InputController.Instance.InputEnabled = true;

        //subscribe back to input
        player.SubscribeToInput();

        //Load world map or reload same map for debugging
        if (player.ReloadLoadedMap)
        {
            player.TriggerAnimation(player.initAnimationName, true);
            LoadScene(SceneManager.GetActiveScene().name);
        }
        else
            LoadScene(GameHelper.WorldMap);
    }
}