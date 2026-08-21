using UnityEngine;

namespace Sonn.BlockBlast
{
    public class UIManager : MonoBehaviour, ISingleton
    {
        public static UIManager Ins;

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

