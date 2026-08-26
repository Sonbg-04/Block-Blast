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
        private readonly List<Square> m_highlightedSquares = new();

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
            HighlightLinesAboutToClear(targetCells, color);
        }    
        private void HighlightLinesAboutToClear(List<Cell> pendingCells, Color color)
        {
            for (int r = 0; r < m_gridSize; r++)
            {
                if (WouldRowBeFull(r, pendingCells))
                {
                    HighlightExistingSquaresInRow(r, color);
                }
            }
            for (int c = 0; c < m_gridSize; c++)
            {
                if (WouldColBeFull(c, pendingCells))
                {
                    HighlightExistingSquaresInCol(c, color);
                }
            }
        }    
        private bool WouldRowBeFull(int row, List<Cell> cells)
        {
            for (int c = 0; c < m_gridSize; c++)
            {
                Cell cell = m_cells[row, c];
                if (!cell.IsOccupied && !cells.Contains(cell))
                {
                    return false;
                }
            }
            return true;
        }
        private bool WouldColBeFull(int col, List<Cell> pendingCells)
        {
            for (int r = 0; r < m_gridSize; r++)
            {
                Cell cell = m_cells[r, col];
                if (!cell.IsOccupied && !pendingCells.Contains(cell))
                {
                    return false;
                }
            }
            return true;
        }
        private void HighlightExistingSquaresInRow(int row, Color color)
        {
            for (int c = 0; c < m_gridSize; c++)
            {
                TryHighlightOccupiedSquare(m_cells[row, c], color);
            }
        }
        private void HighlightExistingSquaresInCol(int col, Color color)
        {
            for (int r = 0; r < m_gridSize; r++)
            {
                TryHighlightOccupiedSquare(m_cells[r, col], color);
            }
        }
        private void TryHighlightOccupiedSquare(Cell cell, Color color)
        {
            if (!cell.IsOccupied || cell.OccupiedSquare == null || m_highlightedSquares.Contains(cell.OccupiedSquare))
            {
                return;
            }
            cell.OccupiedSquare.SetHighLight(true, color);
            m_highlightedSquares.Add(cell.OccupiedSquare);
        }
        public void ClearHighLights()
        {
            for (int i = 0; i < m_highlightedCells.Count; i++)
            {
                m_highlightedCells[i].SetHighLight(false, Color.clear);
            }
            m_highlightedCells.Clear();
            for (int i = 0; i < m_highlightedSquares.Count; i++)
            {
                m_highlightedSquares[i].SetHighLight(false, Color.clear);
            }
            m_highlightedSquares.Clear();
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
        public int CheckAndClearLines()
        {
            List<int> fullRows = new();
            List<int> fullCols = new();
            for (int r = 0; r < m_gridSize; r++)
            {
                if (IsRowFull(r))
                {
                    fullRows.Add(r);
                }
            }
            for (int c = 0; c < m_gridSize; c++)
            {
                if (IsColFull(c))
                {
                    fullCols.Add(c);
                }
            }
            int totalLines = fullRows.Count + fullCols.Count;
            if (totalLines == 0)
            {
                return 0;
            }
            HashSet<Cell> cellsToClear = new();
            for (int i = 0; i < fullRows.Count; i++)
            {
                for (int c = 0; c < m_gridSize; c++)
                {
                    cellsToClear.Add(m_cells[fullRows[i], c]);
                }
            }
            for (int i = 0; i < fullCols.Count; i++)
            {
                for (int r = 0; r < m_gridSize; r++)
                {
                    cellsToClear.Add(m_cells[r, fullCols[i]]);
                }
            }
            PlayClearEffect(fullRows, fullCols, cellsToClear);
            return totalLines;
        }
        private bool IsRowFull(int row)
        {
            for (int c = 0; c < m_gridSize; c++)
            {
                if (!m_cells[row, c].IsOccupied)
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsColFull(int col)
        {
            for (int r = 0; r < m_gridSize; r++)
            {
                if (!m_cells[r, col].IsOccupied)
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanShapeFitAnywhere(Shape shape)
        {
            if (shape == null || shape.CellOffsets.Count == 0)
            {
                return false;
            }
            for (int r = 0; r < m_gridSize; r++)
            {
                for (int c = 0; c < m_gridSize; c++)
                {
                    if (CanPlaceOffsetsAt(shape.CellOffsets, r, c))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CanShapeDataFitAnywhere(ShapeData shapeData)
        {
            if (shapeData == null)
            {
                return false;
            }
            List<Vector2Int> offsets = GetOffsets(shapeData);
            if (offsets.Count == 0)
            {
                return false;
            }
            for (int r = 0; r < m_gridSize; r++)
            {
                for (int c = 0; c < m_gridSize; c++)
                {
                    if (CanPlaceOffsetsAt(offsets, r, c))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private List<Vector2Int> GetOffsets(ShapeData shapeData)
        {
            List<Vector2Int> offsets = new();
            if (shapeData.Grid == null)
            {
                return offsets;
            }
            for (int row = 0; row < shapeData.Rows; row++)
            {
                for (int col = 0; col < shapeData.Columns; col++)
                {
                    if (shapeData.Grid[row].Column[col])
                    {
                        offsets.Add(new Vector2Int(col, row));
                    }
                }
            }
            return offsets;
        }
        public bool HasAnyValidMove(IEnumerable<Shape> shapes)
        {
            if (shapes == null)
            {
                return false;
            }
            foreach (Shape shape in shapes)
            {
                if (shape != null && CanShapeFitAnywhere(shape))
                {
                    return true;
                }
            }
            return false;
        }
        private bool CanPlaceOffsetsAt(IReadOnlyList<Vector2Int> offsets, int originRow, int originCol)
        {
            for (int i = 0; i < offsets.Count; i++)
            {
                Cell cell = GetCellAt(originRow + offsets[i].y, originCol + offsets[i].x);
                if (cell == null || cell.IsOccupied)
                {
                    return false;
                }
            }
            return true;
        }
        private float ComputeClearDelay(Vector2Int pos, List<int> fullRows, List<int> fullCols, float value)
        {
            float delay = 0f;
            bool assigned = false;
            if (fullRows.Contains(pos.y))
            {
                delay = pos.x * value;
                assigned = true;
            }    
            if (fullCols.Contains(pos.x))
            {
                float colDelay = pos.y * value;
                delay = assigned ? Mathf.Min(delay, colDelay) : colDelay;
            }
            return delay;
        }    
        private void PlayClearEffect(List<int> fullRows, List<int> fullCols, HashSet<Cell> cellsToClear)
        {
            foreach (var c in cellsToClear)
            {
                Square s = c.OccupiedSquare;
                Vector2Int pos = c.CellPosOnGrid;
                c.SetOccupied(false, null);
                if (s == null)
                {
                    continue;
                }
                float delay = ComputeClearDelay(pos, fullRows, fullCols, 0.02f);
                s.PlayClearEffect(() =>
                {
                    PoolManager.Ins.ReturnSquare(s);
                }, delay);
            }    
        }    
        public bool IsGridEmpty()
        {
            for (int r = 0; r < m_gridSize; r++)
            {
                for (int c = 0; c < m_gridSize; c++)
                {
                    if (m_cells[r, c].IsOccupied)
                    {
                        return false;
                    }    
                }    
            }
            return true;
        }    
    }
}
