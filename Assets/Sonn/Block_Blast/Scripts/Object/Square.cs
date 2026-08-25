using UnityEngine;
using UnityEngine.Rendering;

namespace Sonn.BlockBlast
{
    public class Square : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_sr;
        [SerializeField] private SpriteRenderer m_highLightSr;

        private SortingGroup m_squareSort;
        private Color m_squareColor;

        private void Awake()
        {
            m_squareSort = GetComponent<SortingGroup>();
        }
        public void SetSprite(Sprite sp)
        {
            m_sr.sprite = sp;
        }
        public void SetOrderInLayer(int order)
        {
            m_squareSort.sortingOrder = order;
        }
        public void SetSquareColor(Color color)
        {
            m_squareColor = color;
        }    
        public void SetHighLight(bool active, Color cl)
        {
            if (active)
            {
                m_highLightSr.color = cl;
            }
            m_highLightSr.gameObject.SetActive(active);
        }    
    }
}