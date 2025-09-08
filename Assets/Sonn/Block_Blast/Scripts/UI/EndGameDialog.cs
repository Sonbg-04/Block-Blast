using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class EndGameDialog : Dialog, IComponentChecking
    {
        public bool IsComponentNull()
        {
            bool check = GUIManager.GetIns<GUIManager>() == null ||
                         GameManager.GetIns<GameManager>() == null ||
                         GridManager.GetIns<GridManager>() == null;
            if (check)
            {
                Debug.LogWarning("Có component bị null. Vui lòng kiểm tra lại!");
            }
            return check;
        }

        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }
        public override void Close()
        {
            base.Close();
        }
    }
}
