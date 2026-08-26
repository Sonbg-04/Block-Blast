using UnityEngine;

namespace Sonn.BlockBlast
{
    public static class Pref
    {
        public static int BestScore
        {
            get => PlayerPrefs.GetInt(Const.BEST_SCORE_KEY, 0);
            set => PlayerPrefs.SetInt(Const.BEST_SCORE_KEY, value);
        }
    }
}
