using UnityEngine;

namespace Sonn.BlockBlast
{
    public class AudioManager : MonoBehaviour, ISingleton
    {
        public static AudioManager Ins;

        private void Awake()
        {
            MakeSingleton();
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
    }
}
