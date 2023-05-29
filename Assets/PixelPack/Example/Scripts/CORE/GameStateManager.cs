/**
 *  GameStateManager is extremely simple class for managing GameState. It uses GameState enum
 *  and CurrentState ReactiveProperty<GameState> to notify game's state changes to components.
 */

using UniRx;

public class GameStateManager : Singleton<GameStateManager>
{
    public enum GameState { InGame, Paused, MainMenu, WorldMap }
    public static ReactiveProperty<GameState> CurrentState = new ReactiveProperty<GameState>();
}
