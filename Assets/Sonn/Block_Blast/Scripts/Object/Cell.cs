using UnityEngine;

namespace Sonn.BlockBlast
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Vector2Int m_cellPosOnGrid;

        private Square m_occupiedSquare;
        private bool m_isOccupied;

        public Vector2Int CellPosOnGrid => m_cellPosOnGrid;
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
    }
}
