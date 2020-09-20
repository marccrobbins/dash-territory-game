using System.Collections;
using Framework;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DashTerritory
{
    public class UIManager : Manager<UIManager>
    {
        public SceneReference campaignLevel;
        public SceneReference freeForAllLevel;
        public EventSystem eventSystem;
        public Page firstPage;

        public Page currentPage;
        public Page lastPage;

        protected override IEnumerator InitializeManager()
        {
            SwitchPages(firstPage);
            
            return base.InitializeManager();
        }

        public void LoadCampaign()
        {
            SceneManager.LoadScene(campaignLevel.SceneName);
        }

        public void LoadFreeForAll()
        {
            SceneManager.LoadScene(freeForAllLevel.SceneName);
        }

        public void SwitchPages(Page nextPage)
        {
            if (currentPage) currentPage.HidePage();
            nextPage.OpenPage();

            lastPage = lastPage == nextPage ? null : currentPage;
            currentPage = nextPage;

            eventSystem.SetSelectedGameObject(currentPage.firstSelected);
        }
    }
}
