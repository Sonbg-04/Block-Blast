using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class SettingsDialog : UIDialog
    {
        [SerializeField] private Button m_btnClose;
        [SerializeField] private Button[] m_btnStates; 
        [SerializeField] private Sprite[] m_bgBtnStates;

        public override void Show()
        {
            base.Show();
            m_btnClose.onClick.RemoveAllListeners();
            m_btnClose.onClick.AddListener(Hide);
            m_btnStates[0].onClick.RemoveAllListeners();
            m_btnStates[0].onClick.AddListener(OnClickMusicButton);
            m_btnStates[1].onClick.RemoveAllListeners();
            m_btnStates[1].onClick.AddListener(OnClickSoundButton);
            RefreshButtonState(m_btnStates[0], Pref.Music);
            RefreshButtonState(m_btnStates[1], Pref.Sound);
        }
        public override void Hide()
        {
            base.Hide();
        }
        private void OnClickMusicButton()
        {
            bool newState = !Pref.Music;
            Pref.Music = newState;
            RefreshButtonState(m_btnStates[0], newState);
        }
        private void OnClickSoundButton()
        {
            bool newState = !Pref.Sound;
            Pref.Sound = newState;
            RefreshButtonState(m_btnStates[1], newState);
        }
        private void RefreshButtonState(Button btn, bool isOn)
        {
            Image bg = btn.image;
            if (bg == null || m_bgBtnStates == null || m_bgBtnStates.Length < 2)
            {
                return;
            }
            bg.sprite = isOn ? m_bgBtnStates[0] : m_bgBtnStates[1];
        }
    }
}