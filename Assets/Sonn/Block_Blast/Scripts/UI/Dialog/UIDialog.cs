using UnityEngine;

namespace Sonn.BlockBlast
{
    public class UIDialog : MonoBehaviour
    {
        public virtual void Show() 
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide() 
        {
            gameObject.SetActive(false);
        }
    }
}
