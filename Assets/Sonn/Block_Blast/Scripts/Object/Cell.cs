using UnityEngine;

namespace Sonn.BlockBlast
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_highlightSr;
        [SerializeField] private Vector2Int m_cellPosOnGrid;
        [SerializeField, Range(0f, 1f)] private float m_highLightAlpha;

        private Square m_occupiedSquare;
        private bool m_isOccupied;

        public bool IsOccupied => m_isOccupied; 

        public void SetCellPosOnGrid(Vector2Int pos)
        {
            m_cellPosOnGrid = pos;
        }
        public void SetOccupied(bool occupied, Square square = null)
        {
            m_isOccupied = occupied;
            m_occupiedSquare = square;
        }
        public void SetHighLight(bool active, Color color)
        {
            if (active)
            {
                color.a = m_highLightAlpha;
                m_highlightSr.color = color;
            }
            m_highlightSr.gameObject.SetActive(active);
        }    
    }
}
