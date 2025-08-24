using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class SettingDialog : Dialog, IComponentChecking
    {
        public Sprite toggleSwitchOn, toggleSwitchOff;
        public Button toggleMusic, toggleSound;

        private bool m_isMusicOn, m_isSoundOn;

        private void Awake()
        {
            m_isMusicOn = Pref.GetBool(GamePref.IsMusicOn.ToString(), true);
            m_isSoundOn = Pref.GetBool(GamePref.IsSoundOn.ToString(), true);
        }
        private void Start()
        {
            LoadSetting();
        }
        private void LoadSetting()
        {
            UpdateMusicUI();
            UpdateSoundUI();
        }    
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null;
            if (check)
            {
                Debug.LogWarning("Có component bị null. Vui lòng kiểm tra lại!");
            }
            return check;
        }
        public override void Show(bool isShow)
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            base.Show(isShow);
            Time.timeScale = 0;
        }
        public override void Close()
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            base.Close();
            Time.timeScale = 1;
        }
        public void ToggleMusic()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);

            m_isMusicOn = !m_isMusicOn;
            Pref.SetBool(GamePref.IsMusicOn.ToString(), m_isMusicOn);

            UpdateMusicUI();
        }
        public void ToggleSound()
        {
            if (IsComponentNull())
            {
                return;
            }

            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);

            m_isSoundOn = !m_isSoundOn;
            Pref.SetBool(GamePref.IsSoundOn.ToString(), m_isSoundOn);

            UpdateSoundUI();
        }
        private void UpdateMusicUI()
        {
            toggleMusic.image.sprite = m_isMusicOn ? toggleSwitchOn : toggleSwitchOff;
            AudioManager.Ins.musicSource.mute = !m_isMusicOn;
        }
        private void UpdateSoundUI()
        {
            toggleSound.image.sprite = m_isSoundOn ? toggleSwitchOn : toggleSwitchOff;
            AudioManager.Ins.btnClickSource.mute = !m_isSoundOn;
        }
    }
}
