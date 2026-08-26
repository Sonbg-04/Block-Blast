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
            m_btnPlayGame.onClick.AddListener(OnClickPlayGame);
        }   
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        private void OnClickPlayGame()
        {
            UIManager.Ins.ChangeState(UIType.Gameplay);
        }    
    }
}