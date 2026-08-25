using UnityEngine;

namespace Sonn.BlockBlast
{
    public class Square : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_sr;

        private Color m_squareColor;

        public void SetSprite(Sprite sp)
        {
            m_sr.sprite = sp;
        }
        public void SetOrderInLayer(int order)
        {
            m_sr.sortingOrder = order;
        }
        public void SetSquareColor(Color color)
        {
            m_squareColor = color;
        }    
    }
}