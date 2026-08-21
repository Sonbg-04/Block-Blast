using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class GameManager : MonoBehaviour, ISingleton
    {
        public static GameManager Ins;

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
