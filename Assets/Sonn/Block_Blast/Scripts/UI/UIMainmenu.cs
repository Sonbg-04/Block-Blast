using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class UIMainmenu : MonoBehaviour
    {
        [SerializeField] private Button m_btnPlayGame;

        public void Show()
        {
            gameObject.SetActive(true);
            m_btnPlayGame.onClick.RemoveAllListeners();
            m_btnPlayGame.onClick.AddListener(OnPlayGameClick);
        }   
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        private void OnPlayGameClick()
        {
            if (!string.IsNullOrEmpty(Pref.GameSaveJson))
            {
                UIManager.Ins.ChangeState(UIType.Gameplay);
                GameManager.Ins.TryLoadProgress();
            }
            else
            {
                GameManager.Ins.ResetGame();
                UIManager.Ins.ChangeState(UIType.Gameplay);
            }
        }    
    }
}