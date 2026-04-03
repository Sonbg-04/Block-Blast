using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class GridManager : MonoBehaviour, ISingleton
    {
        public GameObject blockSlotPrefab;
        public Vector2Int GridSize;
        public Vector2 Offset;
        public float cellDistance;

        private static Dictionary<Type, MonoBehaviour> m_ins;
        private List<CellSlot> m_cellSlots;
        private CellSlot[,] m_cell;
        private int m_currentCombo = 0, m_turnComboClearCount = 0,
                    m_currentTurn = 1, m_shapePlacedThisTurn = 0;

        public static T GetIns<T>() where T : MonoBehaviour
        {
            if (m_ins.TryGetValue(typeof(T), out var ins))
            {
                return ins as T;
            }
            return null;
        }
        private void Awake()
        {
            m_ins = new();
            m_cellSlots = new();
            m_cell = new CellSlot[GridSize.x, GridSize.y];
            MakeSingleton();
        }
        private void Start()
        {
            DrawGridMap();
        }
        private void DrawGridMap()
        {
            for (int i = 0; i < GridSize.x; i++)
            {
                for (int j = 0; j < GridSize.y; j++)
                {
                    var blockSlotClone = Instantiate(blockSlotPrefab, Vector3.zero, Quaternion.identity);
                    blockSlotClone.name = $"Slot_{i}_{j}";
                    blockSlotClone.transform.localPosition = new(
                        i * cellDistance + Offset.x,
                        j * cellDistance + Offset.y,
                        0);
                    blockSlotClone.transform.SetParent(transform, false);

                    var cell = blockSlotClone.GetComponentInChildren<CellSlot>();
                    if (cell != null)
                    {
                        cell.cellPosOnGrid = new(i, j);
                        m_cell[i, j] = cell;
                        m_cellSlots.Add(cell);
                    }    

                }
            }
            Debug.Log($"Có {m_cellSlots.Count} ô !");
        }    
        public void MakeSingleton()
        {
            var key = GetType();
            if (!m_ins.ContainsKey(key) || m_ins[key] == null)
            {
                m_ins[key] = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }    
        }
        public void GridBounds(out Vector2 minWorld, out Vector2 maxWorld)
        {
            var gridBound = GetComponentInChildren<Collider2D>().bounds;

            minWorld = gridBound.min;
            maxWorld = gridBound.max;
        }
        public CellSlot GetNearestCell(Vector3 pos)
        {
            CellSlot c = null;
            var minDistance = float.MaxValue;

            foreach (var slot in m_cellSlots)
            {
                float distance = Vector2.Distance(pos, slot.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    c = slot;
                }
            }

            return c;
        }
        public int CheckClearLines()
        {
            var toClear = new HashSet<CellSlot>();
            var clearedLines = 0;

            for (int row = 0; row < GridSize.x; row++)
            {
                var fullRow = true;
                var rowSlots = new List<CellSlot>();

                for (int col = 0; col < GridSize.y; col++)
                {
                    var slot = m_cell[row, col];
                    rowSlots.Add(slot);
                    if (!slot.isBlockOnCell)
                    {
                        fullRow = false;
                    }    
                }

                if (fullRow)
                {
                    clearedLines++;
                    foreach (var r in rowSlots)
                    {
                        toClear.Add(r);
                    }    
                }    
            }

            for (int col = 0; col < GridSize.y; col++)
            {
                var fullCol = true;
                var colSlots = new List<CellSlot>();

                for (int row = 0; row < GridSize.x; row++)
                {
                    var slot = m_cell[row, col];
                    colSlots.Add(slot);
                    if (!slot.isBlockOnCell)
                    {
                        fullCol = false;
                    }
                }

                if (fullCol)
                {
                    clearedLines++;
                    foreach (var c in colSlots)
                    {
                        toClear.Add(c);
                    }
                }
            }
            if (toClear.Count > 0)
            {
                var index = 0;
                foreach (var slot in toClear)
                {
                    slot.isBlockOnCell = false;
                    StartCoroutine(ClearLineBlockDelay(slot, index * 0.05f));
                    index++;
                }
            }    
            return clearedLines;
        }
        IEnumerator ClearLineBlockDelay(CellSlot slot, float delay)
        {
            yield return new WaitForSeconds(delay);
            slot.ClearSquareOnCell();
        }
        public void ProcessAfterShapePlaced()
        {
            m_shapePlacedThisTurn++;
            int cleared;
            do
            {
                cleared = CheckClearLines();
                if (cleared > 0)
                {
                    if (m_turnComboClearCount == 0 && m_currentCombo == 0)
                    {
                        m_currentCombo = 1;
                    }
                    else
                    {
                        m_currentCombo++;
                        GUIManager.GetIns<GUIManager>().ShowGreatTxtImg();
                    }

                    var type = GetClearLineType(cleared, m_currentCombo);
                    var score = CalculateScore(cleared, m_currentCombo, type);
                    GameManager.GetIns<GameManager>().AddScore(score);
                    GUIManager.GetIns<GUIManager>().UpdateCombo(m_currentCombo);

                    m_turnComboClearCount++;
                }
            }
            while (cleared > 0);

            if (m_shapePlacedThisTurn == 2)
            {
                var remainingShapes = new List<ShapeData>();
                var shapes = FindObjectsOfType<Shape>();
                foreach (var shape in shapes)
                {
                    if (shape.enabled && shape.currentShapeData != null)
                    {
                        remainingShapes.Add(shape.currentShapeData);
                    }    
                }

                if (remainingShapes.Count == 1 && !HasValidMove(remainingShapes))
                {
                    Debug.Log("Shape cuối cùng không đặt được → Game Over");
                    return;
                }    
            }
            
            if (m_shapePlacedThisTurn >= 3)
            {
                EndTurn();
            }
        }
        private void EndTurn()
        {
            m_currentTurn++;
            m_shapePlacedThisTurn = 0;

            if (m_turnComboClearCount == 0)
            {
                m_currentCombo = 0;
            }

            m_turnComboClearCount = 0;
                
        }
        private ClearLineType GetClearLineType(int linesCleared, int combo)
        {
            if (linesCleared == 1 && combo == 1)
            {
                return ClearLineType.Normal;
            }
            else if (linesCleared > 1 && combo == 1)
            {
                return ClearLineType.Multiline;
            }
            else if (linesCleared == 1 && combo > 1)
            {
                return ClearLineType.Combo;
            }
            else if (linesCleared > 1 && combo > 1)
            {
                return ClearLineType.ComboMultiline;
            }

            return ClearLineType.Normal;
        }
        private int CalculateScore(int linesCleared, int combo, ClearLineType type)
        {
            return type switch
            {
                ClearLineType.Normal => 10 * linesCleared,
                ClearLineType.Multiline => (10 * linesCleared + 5 * linesCleared) + (10 * combo),
                ClearLineType.Combo => (10 * combo),
                ClearLineType.ComboMultiline => (10 * linesCleared * combo) + (10 * combo),
                _ => 0,
            };
        }
        public List<CellSlot> PreviewClearLines(GameObject square, List<CellSlot> hoveredSlots)
        {
            var previeweds = new List<CellSlot>();
            if (square == null || hoveredSlots == null)
            {
                return previeweds;
            }

            foreach (var slot in m_cellSlots)
            {
                slot.ResetHighlightSquares();
            }

            var toHighLight = new HashSet<CellSlot>();

            for (int row = 0; row < GridSize.x; row++)
            {
                bool almostFull = true;
                var rowSlots = new List<CellSlot>();

                for (int col = 0; col < GridSize.y; col++)
                {
                    var slot = m_cell[row, col];
                    rowSlots.Add(slot);

                    if (!slot.isBlockOnCell && !hoveredSlots.Contains(slot))
                    {
                        almostFull = false;
                    }
                }

                if (almostFull)
                {
                    foreach (var s in rowSlots)
                    {
                        toHighLight.Add(s);
                    }
                }
            }

            for (int col = 0; col < GridSize.y; col++)
            {
                bool almostFull = true;
                var colSlots = new List<CellSlot>();

                for (int row = 0; row < GridSize.x; row++)
                {
                    var slot = m_cell[row, col];
                    colSlots.Add(slot);

                    if (!slot.isBlockOnCell && !hoveredSlots.Contains(slot))
                    {
                        almostFull = false;
                    }
                }

                if (almostFull)
                {
                    foreach (var s in colSlots)
                    {
                        toHighLight.Add(s);
                    }
                }
            }

            foreach (var slot in toHighLight)
            {
                slot.SetHighLightSquares(square, 0.6f);
                previeweds.Add(slot);
            }

            return previeweds;
        }
        public bool HasValidMove(List<ShapeData> shapeDatas)
        {
            if (shapeDatas == null || shapeDatas.Count <= 0)
            {
                return false;
            }
            foreach (var item in shapeDatas)
            {
                for (int startRow = 0; startRow <= (GridSize.x - item.Rows); startRow++)
                {
                    for (int startCol = 0; startCol <= (GridSize.y - item.Columns); startCol++)
                    {
                        if (CanPlaceShapeAt(item, startRow, startCol))
                        {
                            return true;
                        }
                    }
                }    
            }   
            return false;
        }
        private bool CanPlaceShapeAt(ShapeData sd, int startRow, int startCol)
        {
            for (int r = 0; r < sd.Rows; r++)
            {
                for (int c = 0; c < sd.Columns; c++)
                {
                    if (!sd.Grid[r].Column[c])
                    {
                        continue;
                    }

                    var x = startRow + r;
                    var y = startCol + c;

                    if (x < 0 || x >= GridSize.x || y < 0 || y >= GridSize.y)
                    {
                        return false;
                    }

                    if (m_cell[x, y].isBlockOnCell)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
