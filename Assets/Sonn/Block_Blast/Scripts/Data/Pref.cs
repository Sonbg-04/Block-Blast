using UnityEngine;

namespace Sonn.BlockBlast
{
    public static class Pref
    {
        public static int Score
        {
            get => PlayerPrefs.GetInt(Const.SCORE_KEY, 0);
            set => PlayerPrefs.SetInt(Const.SCORE_KEY, value);
        }

    }
}
