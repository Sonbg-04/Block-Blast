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
        public static bool Music
        {
            get => PlayerPrefs.GetInt(Const.MUSIC_VOLUME_KEY, 1) == 1;
            set => PlayerPrefs.SetInt(Const.MUSIC_VOLUME_KEY, value ? 1 : 0);
        }
        public static bool Sound
        {
            get => PlayerPrefs.GetInt(Const.SOUND_VOLUME_KEY, 1) == 1;
            set => PlayerPrefs.SetInt(Const.SOUND_VOLUME_KEY, value ? 1 : 0);
        }
        public static int BoosterCount
        {
            get => PlayerPrefs.GetInt(Const.BOOSTER_COUNT_KEY, 3);
            set => PlayerPrefs.SetInt(Const.BOOSTER_COUNT_KEY, value);
        }
        public static string GameSaveJson
        {
            get => PlayerPrefs.GetString(Const.GAME_SAVE_KEY, string.Empty);
            set => PlayerPrefs.SetString(Const.GAME_SAVE_KEY, value);
        }
        public static void ClearGameSave()
        {
            PlayerPrefs.DeleteKey(Const.GAME_SAVE_KEY);
        }
    }
}
