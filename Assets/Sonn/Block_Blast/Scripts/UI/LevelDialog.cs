using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BlockBlast
{
    public class LevelDialog : Dialog, IComponentChecking
    {
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
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            base.Show(isShow);
        }
        public override void Close()
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            base.Close();
        }
        public void PlayTimeMode()
        {
            if (IsComponentNull())
            {
                return;
            }
            Pref.currentMode = GameMode.TimeMode;
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            SceneManager.LoadScene(Const.TIME_MODE_SCENE);


        }
        public void PlayScoreMode()
        {
            if (IsComponentNull())
            {
                return;
            }
            Pref.currentMode = GameMode.ScoreMode;
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);

        }

    }
}
