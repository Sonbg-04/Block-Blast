using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class UIGameplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_scoreTxt;
        [SerializeField] private TextMeshProUGUI m_bestScoreTxt;
        [SerializeField] private Button m_btnSettings;
        [SerializeField] private Button m_btnReturn;
        [SerializeField] private SettingsDialog m_settingsUI;

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
            m_btnSettings.onClick.RemoveAllListeners();
            m_btnSettings.onClick.AddListener(ShowUISettings);
            m_btnReturn.onClick.RemoveAllListeners();
            m_btnReturn.onClick.AddListener(ReturnToMainMenu);
        }
        private void UpdateScoreTxt(int score)
        {
            m_scoreTxt.text = score.ToString("0000");
        }    
        private void UpdateBestScoreTxt(int score)
        {
            m_bestScoreTxt.text = score.ToString("0000");
        }    
        private void ShowUISettings()
        {
            m_settingsUI.Show();
        }    
        private void ReturnToMainMenu()
        {
            UIManager.Ins.ChangeState(UIType.Mainmenu);
        }    
    }
}

