using TMPro;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class UIGameplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_scoreTxt;
        [SerializeField] private TextMeshProUGUI m_bestScoreTxt;

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
        }
        private void UpdateScoreTxt(int score)
        {
            m_scoreTxt.text = score.ToString("0000");
        }    
        private void UpdateBestScoreTxt(int score)
        {
            m_bestScoreTxt.text = score.ToString("0000");
        }    

    }
}

