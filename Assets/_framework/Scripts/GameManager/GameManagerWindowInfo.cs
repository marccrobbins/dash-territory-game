using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.GameData
{
    [CreateAssetMenu(menuName = "GameData/GameManagerWindowInfo", fileName = "GameManagerWindowInfo")]
    public class GameManagerWindowInfo : ScriptableObject
    {
        private static GameManagerWindowInfo instance;
        public static GameManagerWindowInfo Instance
        {
            get
            {
                if (instance) return instance;
                return instance = Resources.Load<GameManagerWindowInfo>("GameManagerWindowInfo");
            }
        }


        public GameDataManagerReference[] references;
    }

    [System.Serializable]
    public sealed class GameDataManagerReference
    {
        public string managerName;
        public string managerPath;
        public object managerType;

        #region Odin

        private void ValidateManagerType()
        {
            if (managerType is GameDataManager) return;
            
            Debug.LogError($"{managerType} does not derive from GameDataManager.");
            managerType = null;
        }

        #endregion Odin
    }
}
