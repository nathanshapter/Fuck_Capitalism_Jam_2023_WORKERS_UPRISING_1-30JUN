/**
 *  Class for setting time scale. 
 *  Subscribes to GameStateManager's CurrentState to pause game when state changes.
 */

using UnityEngine;
using UniRx;

public class TimeManager : Singleton<TimeManager>
{
    /**
    * Set initial time scale to 1f, subscribe to GameStateManager's CurrentState
    */
    public void Init()
    {
        SetTimeScale(1f);
        GameStateManager.CurrentState.Subscribe(s => HandleGameStateChange(s));
    }

    /**
    * Set new time scale (0f-1f)
    * @param _timeScale New time scale.
    */
    public void SetTimeScale(float _timeScale)
    {
        Time.timeScale = _timeScale;
    }

    private void HandleGameStateChange(GameStateManager.GameState _gameState)
    {
        if (_gameState == GameStateManager.GameState.Paused)
            SetTimeScale(0f);
        else
            SetTimeScale(1f);
    }
}
