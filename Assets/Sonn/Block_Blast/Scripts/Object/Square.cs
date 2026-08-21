using UnityEngine;

namespace Sonn.BlockBlast
{
    public class Square : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_sr;

        public void SetSprite(Sprite sp)
        {
            m_sr.sprite = sp;
        }    
        public void AddOrderInLayer(int order)
        {
            m_sr.sortingOrder += order;
        }
    }
}