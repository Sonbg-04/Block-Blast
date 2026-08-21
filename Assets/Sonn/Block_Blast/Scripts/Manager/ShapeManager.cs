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
        private int m_nextSpawnOrderInLayer;

        private void Awake()
        {
            MakeSingleton();
        }
        private void Start()
        {
            m_currentShapes = new Shape[m_slots.Length];
            for (int i = 0; i < m_slots.Length; i++)
            {
                SpawnShapeAtSlot(i);
            }
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        public void SpawnShapeAtSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= m_slots.Length)
            {
                return;
            }
            ShapeData shapeData = GetRandomShapeData();
            Sprite sprite = GetRandomSprite();
            Shape shape = PoolManager.Ins.GetShape();
            shape.transform.SetParent(m_slots[slotIndex].transform, false);
            shape.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            shape.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            shape.SetShape(shapeData, sprite);
            m_currentShapes[slotIndex] = shape;
        }
        public void ClearSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= m_currentShapes.Length)
            {
                return;
            }
            Shape shape = m_currentShapes[slotIndex];
            if (shape == null)
            {
                return;
            }
            PoolManager.Ins.ReturnShape(shape);
            m_currentShapes[slotIndex] = null;
        }
        public void OnShapePlaced(Shape shape)
        {
            int slotIndex = GetSlotIndexOfShape(shape);
            if (slotIndex >= 0)
            {
                m_currentShapes[slotIndex] = null;
            }
            AddOrderInLayerForSlotShapes(1, shape);
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
            for (int i = 0; i < m_slots.Length; i++)
            {
                SpawnShapeAtSlot(i);
                m_currentShapes[i].AddOrderInLayer(m_nextSpawnOrderInLayer);
                m_nextSpawnOrderInLayer++;
            }
        }
        public void AddOrderInLayerForSlotShapes(int delta, Shape excludeShape = null)
        {
            if (m_currentShapes == null)
            {
                return;
            }
            for (int i = 0; i < m_currentShapes.Length; i++)
            {
                Shape shape = m_currentShapes[i];
                if (shape == null || shape == excludeShape)
                {
                    continue;
                }
                shape.AddOrderInLayer(delta);
            }
        }
        private ShapeData GetRandomShapeData()
        {
            if (m_allShape == null || m_allShape.Length == 0)
            {
                return null;
            }
            return m_allShape[Random.Range(0, m_allShape.Length)];
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