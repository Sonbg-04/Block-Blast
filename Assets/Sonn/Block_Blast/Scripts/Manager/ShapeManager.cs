using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class ShapeManager : MonoBehaviour, ISingleton
    {
        public static ShapeManager Ins;

        [SerializeField] private GameObject[] m_slots;
        [SerializeField] private Sprite[] m_squareVisuals;
        [SerializeField] private ShapeData[] m_allShape;

        private Shape[] m_currentShapes;

        public IReadOnlyList<Shape> CurrentShapes => m_currentShapes;

        private void Awake()
        {
            MakeSingleton();
        }
        private void Start()
        {
            m_currentShapes = new Shape[m_slots.Length];
            SpawnDistinctShapesForAllSlots();
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        private void SpawnShapeAtSlot(int slotIndex, ShapeData shapeData)
        {
            if (slotIndex < 0 || slotIndex >= m_slots.Length)
            {
                return;
            }
            Sprite sprite = GetRandomSprite();
            Shape shape = PoolManager.Ins.GetShape();
            shape.transform.SetParent(m_slots[slotIndex].transform, false);
            shape.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            shape.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            shape.SetShape(shapeData, sprite);
            m_currentShapes[slotIndex] = shape;
        }
        public void OnShapePlaced(Shape shape)
        {
            int slotIndex = GetSlotIndexOfShape(shape);
            if (slotIndex >= 0)
            {
                m_currentShapes[slotIndex] = null;
            }
            if (AreAllSlotsEmpty())
            {
                RespawnAllSlots();
            }
        }
        private int GetSlotIndexOfShape(Shape shape)
        {
            for (int i = 0; i < m_currentShapes.Length; i++)
            {
                if (m_currentShapes[i] == shape)
                {
                    return i;
                }
            }
            return -1;
        }
        private bool AreAllSlotsEmpty()
        {
            for (int i = 0; i < m_currentShapes.Length; i++)
            {
                if (m_currentShapes[i] != null)
                {
                    return false;
                }
            }
            return true;
        }
        private void RespawnAllSlots()
        {
            SpawnDistinctShapesForAllSlots();
        }
        private void SpawnDistinctShapesForAllSlots()
        {
            List<ShapeData> shapeDataForSlots = GetDistinctRandomShapeData(m_slots.Length);
            for (int i = 0; i < m_slots.Length; i++)
            {
                SpawnShapeAtSlot(i, shapeDataForSlots[i]);
            }
        }
        private List<ShapeData> GetDistinctRandomShapeData(int count)
        {
            List<ShapeData> result = new(count);
            if (m_allShape == null || m_allShape.Length == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    result.Add(null);
                }
                return result;
            }
            List<ShapeData> pool = new();
            for (int i = 0; i < count; i++)
            {
                if (pool.Count == 0)
                {
                    pool.AddRange(m_allShape);
                    ShuffleList(pool);
                }
                int lastIndex = pool.Count - 1;
                result.Add(pool[lastIndex]);
                pool.RemoveAt(lastIndex);
            }
            return result;
        }
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        private Sprite GetRandomSprite()
        {
            if (m_squareVisuals == null || m_squareVisuals.Length == 0)
            {
                return null;
            }
            return m_squareVisuals[Random.Range(0, m_squareVisuals.Length)];
        }
    }
}