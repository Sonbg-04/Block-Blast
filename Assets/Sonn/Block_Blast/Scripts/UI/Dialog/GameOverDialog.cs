using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class GameOverDialog : UIDialog
    {
        [SerializeField] private TextMeshProUGUI m_scoreAfterLoseTxt;
        [SerializeField] private TextMeshProUGUI m_bestScoreAfterLoseTxt;
        [SerializeField] private Button m_btnHome;
        [SerializeField] private Button m_btnReplay;

        public override void Show()
        {
            base.Show();
            m_btnHome.onClick.RemoveAllListeners();
            m_btnReplay.onClick.RemoveAllListeners();
            m_btnHome.onClick.AddListener(ReturnMainmenu);
            m_btnReplay.onClick.AddListener(OnReplayGame);
            UpdateText();
        }
        public override void Hide()
        {
            base.Hide();
        }
        private void UpdateText()
        {
            m_scoreAfterLoseTxt.text = GameManager.Ins.Score.ToString("0000");
            m_bestScoreAfterLoseTxt.text = Pref.BestScore.ToString("0000");
        }    
        private void ReturnMainmenu()
        {
            UIManager.Ins.ChangeState(UIType.Mainmenu);
            Hide();
        }    
        private void OnReplayGame()
        {

        }    
    }
}
