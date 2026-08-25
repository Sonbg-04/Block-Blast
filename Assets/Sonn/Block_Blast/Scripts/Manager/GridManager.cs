using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class GridManager : MonoBehaviour, ISingleton
    {
        public static GridManager Ins;

        [SerializeField] private Transform m_gridParent;
        [SerializeField] private int m_gridSize;
        [SerializeField] private float m_cellSize;

        private Cell[,] m_cells;
        private readonly List<Cell> m_highlightedCells = new();

        private void Awake()
        {
            MakeSingleton();
        }
        private void Start()
        {
            GenerateGrid();
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        private void GenerateGrid()
        {
            m_cells = new Cell[m_gridSize, m_gridSize];
            float step = m_cellSize; // Nếu có spacing thì cộng thêm spacing với kích cỡ ô
            float offset = (m_gridSize - 1) * step / 2;
            for (int r = 0; r < m_gridSize; r++)
            {
                for (int c = 0; c < m_gridSize; c++)
                {
                    Cell cell = PoolManager.Ins.GetCell();
                    if (cell == null)
                    {
                        return;
                    }
                    cell.gameObject.SetActive(true);
                    cell.transform.SetParent(m_gridParent, false);
                    cell.transform.SetLocalPositionAndRotation(new Vector3(c * step - offset, r * step - offset, 0f), Quaternion.identity);
                    cell.transform.localScale = Vector3.one;
                    cell.SetCellPosOnGrid(new Vector2Int(c, r));
                    m_cells[r, c] = cell;
                }    
            }    
        }    
        private Cell GetCellAt(int row, int col)
        {
            if (row < 0 || row >= m_gridSize || col < 0 || col >= m_gridSize)
            {
                return null;
            }
            return m_cells[row, col];
        }
        private Vector2Int? WorldToGridCoord(Vector3 worldPos)
        {
            Vector3 local = m_gridParent.InverseTransformPoint(worldPos);
            float step = m_cellSize;
            float offset = (m_gridSize - 1) * step / 2f;
            int col = Mathf.RoundToInt((local.x + offset) / step);
            int row = Mathf.RoundToInt((local.y + offset) / step);
            if (row < 0 || row >= m_gridSize || col < 0 || col >= m_gridSize)
            {
                return null;
            }
            return new Vector2Int(col, row);
        }
        public bool TryGetPlacementCells(Shape shape, out List<Cell> targetCells)
        {
            targetCells = new List<Cell>();
            IReadOnlyList<Square> squares = shape.ActiveSquares;
            for (int i = 0; i < squares.Count; i++)
            {
                Vector2Int? coord = WorldToGridCoord(squares[i].transform.position);
                if (coord == null)
                {
                    targetCells = null;
                    return false;
                }
                Cell cell = GetCellAt(coord.Value.y, coord.Value.x);
                if (cell == null || cell.IsOccupied || targetCells.Contains(cell))
                {
                    targetCells = null;
                    return false;
                }
                targetCells.Add(cell);
            }
            return true;
        }
        public void ShowPlacementPreview(Shape shape)
        {
            ClearHighLights();
            if (shape == null || !TryGetPlacementCells(shape, out var targetCells))
            {
                return;
            }
            Color color = shape.CurrentColor;
            for (int i = 0; i < targetCells.Count; i++)
            {
                targetCells[i].SetHighLight(true, color);
                m_highlightedCells.Add(targetCells[i]);
            }    
        }    
        public void ClearHighLights()
        {
            for (int i = 0; i < m_highlightedCells.Count; i++)
            {
                m_highlightedCells[i].SetHighLight(false, Color.clear);
            }
            m_highlightedCells.Clear();
        }    
        public void PlaceShapeIntoCells(Shape shape, List<Cell> targetCells)
        {
            IReadOnlyList<Square> squares = shape.ActiveSquares;
            for (int i = 0; i < squares.Count; i++)
            {
                Cell cell = targetCells[i];
                Square square = squares[i];
                square.transform.SetParent(cell.transform, true);
                square.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                square.transform.localScale = Vector3.one;
                cell.SetOccupied(true, square);
            }
        }
    }
}
