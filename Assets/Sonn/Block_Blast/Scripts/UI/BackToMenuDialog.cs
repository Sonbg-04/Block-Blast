using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BlockBlast
{
    public class BackToMenuDialog : Dialog, IComponentChecking
    {
        public override void Show(bool isShow)
        {
            if (IsComponentNull())
            {
                return;
            }

            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            base.Show(isShow);
            Time.timeScale = 0;
        }
        public override void Close()
        {
            if (IsComponentNull())
            {
                return;
            }

            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            base.Close();
            Time.timeScale = 1;
        }
        public void OnClickYes()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.btnClickSource);
            SceneManager.LoadScene(Const.MAIN_MENU_SCENE);
            Time.timeScale = 1;

            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(Const.GAME_PLAY_TAG);
            foreach (var obj in gameObjects)
            {
                Destroy(obj);
            }

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
    }
}
