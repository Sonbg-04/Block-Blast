using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
        public void CheckClearLines()
        {
            var toClear = new HashSet<CellSlot>();

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
                    foreach (var c in colSlots)
                    {
                        toClear.Add(c);
                    }
                }
            }

            foreach (var slot in toClear)
            {
                slot.isBlockOnCell = false;
                slot.ClearSquareOnCell();
            }
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
    }
}
