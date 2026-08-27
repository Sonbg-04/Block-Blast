using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class UIGameplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_scoreTxt;
        [SerializeField] private TextMeshProUGUI m_bestScoreTxt;
        [SerializeField] private TextMeshProUGUI m_countTxt;
        [SerializeField] private Button m_btnSettings;
        [SerializeField] private Button m_btnReturn;
        [SerializeField] private Button m_btnBoosterRandomShape;
        [SerializeField] private SettingsDialog m_settingsUI;
        [SerializeField] private GameOverDialog m_gameOverUI;

        private int m_currentBoosterCount;

        public GameOverDialog GameOverUI  => m_gameOverUI;
        public int CurrentBoosterCount
        {
            get => m_currentBoosterCount;
            set
            {
                m_currentBoosterCount = value;
                Pref.BoosterCount = m_currentBoosterCount;
                UpdateCountBoosterTxt();
            }
        }

        private void OnEnable()
        {
            UIEvent.OnScoreChanged += UpdateScoreTxt;
        }
        private void OnDisable()
        {
            UIEvent.OnScoreChanged -= UpdateScoreTxt;
        }
        public void Show()
        {
            gameObject.SetActive(true);
            m_currentBoosterCount = Pref.BoosterCount;
            Refresh();
        }    
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        private void Refresh()
        {
            UpdateScoreTxt(GameManager.Ins.Score);
            UpdateBestScoreTxt(Pref.BestScore);
            UpdateCountBoosterTxt();
            m_btnSettings.onClick.RemoveAllListeners();
            m_btnSettings.onClick.AddListener(OnShowSettingsUIClick);
            m_btnReturn.onClick.RemoveAllListeners();
            m_btnReturn.onClick.AddListener(OnReturnToMainMenuClick);
            m_btnBoosterRandomShape.interactable = true;
            m_btnBoosterRandomShape.onClick.RemoveAllListeners();
            m_btnBoosterRandomShape.onClick.AddListener(OnBoosterRandomClick);
        }
        private void UpdateScoreTxt(int score)
        {
            m_scoreTxt.text = score.ToString("0000");
        }    
        private void UpdateBestScoreTxt(int score)
        {
            m_bestScoreTxt.text = score.ToString("0000");
        }    
        private void UpdateCountBoosterTxt()
        {
            m_countTxt.text = m_currentBoosterCount.ToString();
        }
        private void OnShowSettingsUIClick()
        {
            m_settingsUI.Show();
        }    
        private void OnReturnToMainMenuClick()
        {
            UIManager.Ins.ChangeState(UIType.Mainmenu);
        }    
        private void OnBoosterRandomClick()
        {
            ShapeManager.Ins.ResetShapes();
            m_currentBoosterCount--;
            if (m_currentBoosterCount < 1)
            {
                m_currentBoosterCount = 0;
                m_btnBoosterRandomShape.interactable = false;
            }    
            Pref.BoosterCount = m_currentBoosterCount;
            UpdateCountBoosterTxt();
        }    
    }
}

