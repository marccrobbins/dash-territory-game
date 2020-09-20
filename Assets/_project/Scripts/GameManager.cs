using System;
using System.Collections;
using Framework;
using Sirenix.OdinInspector;

namespace DashTerritory
{
    public class GameManager : Manager<GameManager>
    {
        public static event Action<GameState> OnGameStateChangeEvent;

        private GameState state;
        public GameState State
        {
            get => state;
            set
            {
                if (state == value) return;
                state = value;
                OnGameStateChangeEvent?.Invoke(state);
            }
        }

        protected override IEnumerator InitializeManager()
        {
            State = GameState.Main;
            return base.InitializeManager();
        }

        public void StartGame()
        {
            State = GameState.InGame;
            PlayerManager.Instance.SpawnPlayers();
        }
    }

    public enum GameState
    {
        Main,
        InGame
    }
}
