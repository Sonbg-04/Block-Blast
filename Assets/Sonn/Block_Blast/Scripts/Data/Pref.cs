using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public enum GameMode
    {
        TimeMode,
        ScoreMode
    }
    public enum GamePref
    {
        IsMusicOn,
        IsSoundOn
    }
    public static class Pref
    {
        public static GameMode currentMode;
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.HasKey(key)
                 ? PlayerPrefs.GetInt(key) == 1 ? true : false
                 : defaultValue;
        }
        public static void SetBool(string key, bool value)
        {
            if (value)
            {
                PlayerPrefs.SetInt(key, 1);
            }
            else
            {
                PlayerPrefs.SetInt(key, 0);
            }    
        }
        public static int BestScore
        {
            set
            {
                int currentScore = PlayerPrefs.GetInt(Const.BEST_SCORE, 0);
                if (currentScore < value)
                {
                    PlayerPrefs.SetInt(Const.BEST_SCORE, value);
                }
            }
            get => PlayerPrefs.GetInt(Const.BEST_SCORE, 0);
        }
        public static int Score
        {
            get => PlayerPrefs.GetInt(Const.SCORE, 0);
            set => PlayerPrefs.SetInt(Const.SCORE, value);
        }
    }
}
