using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Sonn.BlockBlast
{
    public class GridManager : MonoBehaviour, IComponentChecking, ISingleton
    {
        public GameObject blockSlotPrefab;
        public Vector2Int GridSize;
        public Vector2 Offset;
        public float cellDistance;

        private static Dictionary<Type, MonoBehaviour> m_ins;
        private List<CellSlot> m_cellSlots;

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
            MakeSingleton();
        }
        private void Start()
        {
            DrawGridMap();
        }
        public bool IsComponentNull()
        {
            throw new System.NotImplementedException();
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

    }
}
