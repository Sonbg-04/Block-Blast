using UnityEngine;

namespace Sonn.BlockBlast
{
    public class UIManager : MonoBehaviour, ISingleton
    {
        public static UIManager Ins;

        [SerializeField] private UILoading m_loadingUI;
        [SerializeField] private UIMainmenu m_mainMenuUI;
        [SerializeField] private UIGameplay m_gameplayUI;

        private UIState m_currentUIState;
        private LoadingState m_loadingState;
        private MainmenuState m_mainmenuState;
        private GameplayState m_gameplayState;

        public UILoading LoadingUI => m_loadingUI;
        public UIMainmenu MainmenuUI => m_mainMenuUI;
        public UIGameplay GameplayUI => m_gameplayUI;
        public bool IsInGameplay => m_currentUIState != null && m_currentUIState.UIType == UIType.Gameplay;

        private void Awake()
        {
            MakeSingleton();
            InitStates();
        }
        private void Start()
        {
            ChangeState(UIType.Loading);
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        private void InitStates()
        {
            m_loadingState = new LoadingState();
            m_mainmenuState = new MainmenuState();
            m_gameplayState = new GameplayState();
        }
        public void ChangeState(UIType type)
        {
            UIState newState = GetUIState(type);
            if (newState == null)
            {
                return;
            }
            if (m_currentUIState == newState)
            {
                return;
            }
            m_currentUIState?.Exit();
            m_currentUIState = newState;
            m_currentUIState.Enter();
        }
        private UIState GetUIState(UIType type)
        {
            return type switch
            {
                UIType.Loading => m_loadingState,
                UIType.Mainmenu => m_mainmenuState,
                UIType.Gameplay => m_gameplayState,
                _ => null
            };
        }    
    }
}

