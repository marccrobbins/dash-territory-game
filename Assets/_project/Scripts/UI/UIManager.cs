using System;
using System.Collections;
using Framework;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DashTerritory
{
    public class UIManager : Manager<UIManager>
    {
        private const string LogPrefix = "UIMANAGER :: ";

        public static event Action<UIState> OnUIStateChangeEvent;

        public SceneReference mainLevel;
        public SceneReference campaignLevel;
        public SceneReference freeForAllLevel;
        public EventSystem eventSystem;
        public Page mainMenuPage;
        public Page inGamePage;
        
        private Page currentPage;
        private Page lastPage;
        private UIState state;
        
        public UIState State
        {
            get => state;
            set
            {
                if (state == value) return;
                state = value;
                OnUIStateChangeEvent?.Invoke(state);
            }
        }

        protected override IEnumerator InitializeManager()
        {
            InputManager.OnPlayerJoined += OnPlayerJoined;
            GameManager.OnGameStateChangeEvent += OnGameStateChanged;
            SwitchPages(mainMenuPage);
            State = UIState.Main;
            
            return base.InitializeManager();
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Main:
                    State = UIState.Main;
                    break;
                case GameState.InGame:
                    State = UIState.Closed;
                    break;
            }
        }
        
        #region Input

        private void OnPlayerJoined(PlayerInputActions inputActions)
        {
            inputActions.OnBackEvent += BackButton;
            inputActions.OnStartEvent += StartButton;
        }

        private void BackButton()
        {
            if (!lastPage) return; 
            SwitchPages(lastPage);
        }

        private void StartButton()
        {
            //only works in game
            if (GameManager.Instance.State != GameState.InGame) return;

            switch (State)
            {
                case UIState.Closed:
                    SwitchPages(inGamePage);
                    State = UIState.InGame;
                    break;
                case UIState.InGame:
                    SwitchPages(null);
                    State = UIState.Closed;
                    break;
            }
        }
        
        #endregion Input

        public void LoadMain()
        {
            SceneManager.LoadScene(mainLevel.SceneName);
        }

        public void LoadCampaign()
        {
            SceneManager.LoadScene(campaignLevel.SceneName);
            GameManager.Instance.StartGame();
        }

        public void LoadFreeForAll()
        {
            SceneManager.LoadScene(freeForAllLevel.SceneName);
            GameManager.Instance.StartGame();
        }

        public void SwitchPages(Page nextPage)
        {
            if (currentPage) currentPage.HidePage();
            
            //If no next page, close everything
            if (!nextPage)
            {
                currentPage = null;
                lastPage = null;
                State = UIState.Closed;
                return;
            }
            
            nextPage.OpenPage();

            lastPage = lastPage == nextPage ? null : currentPage;
            currentPage = nextPage;

            eventSystem.SetSelectedGameObject(currentPage.firstSelected);
        }
    }

    public enum UIState
    {
        Closed,
        Main, 
        InGame
    }
}
