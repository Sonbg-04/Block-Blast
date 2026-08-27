using System;
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
        private void SpawnShapeAtSlot(int slotIndex, ShapeData shapeData, Sprite sprite)
        {
            if (slotIndex < 0 || slotIndex >= m_slots.Length)
            {
                return;
            }
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
        }
        public void TryRespawnIfNeeded()
        {
            if (AreAllSlotsEmpty())
            {
                RespawnAllSlots();
            }
        }
        public bool HasAnyActiveShape()
        {
            for (int i = 0; i < m_currentShapes.Length; i++)
            {
                if (m_currentShapes[i] != null)
                {
                    return true;
                }
            }
            return false;
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
            List<ShapeData> shapeDataForSlots = GetValidShapeDataSet(m_slots.Length);
            List<Sprite> spritesForSlots = GetDistinctRandomSprites(m_slots.Length);
            for (int i = 0; i < m_slots.Length; i++)
            {
                SpawnShapeAtSlot(i, shapeDataForSlots[i], spritesForSlots[i]);
            }
        }
        private List<ShapeData> GetValidShapeDataSet(int count)
        {
            bool isGridEmpty = GridManager.Ins == null || GridManager.Ins.IsGridEmpty();
            for (int attempt = 0; attempt < Const.MAX_RETRY_SPAWN; attempt++)
            {
                List<ShapeData> candidate = GetDistinctRandomShapeData(count);
                if (HasAnyRotationPair(candidate))
                {
                    continue;
                }
                if (isGridEmpty || HasAnyFittingShape(candidate))
                {
                    return candidate;
                }
            }
            if (!isGridEmpty)
            {
                for (int attempt = 0; attempt < Const.MAX_RETRY_SPAWN; attempt++)
                {
                    List<ShapeData> candidate = GetDistinctRandomShapeData(count);
                    if (HasAnyFittingShape(candidate))
                    {
                        return candidate;
                    }
                }
            }
            return GetDistinctRandomShapeData(count);
        }
        private bool HasAnyFittingShape(List<ShapeData> shapes)
        {
            if (GridManager.Ins == null)
            {
                return true;
            }
            for (int i = 0; i < shapes.Count; i++)
            {
                if (GridManager.Ins.CanShapeDataFitAnywhere(shapes[i]))
                {
                    return true;
                }
            }
            return false;
        }
        private bool HasAnyRotationPair(List<ShapeData> shapes)
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                for (int j = i + 1; j < shapes.Count; j++)
                {
                    if (AreShapesRotationsOfEachOther(shapes[i], shapes[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool AreShapesRotationsOfEachOther(ShapeData a, ShapeData b)
        {
            if (a == null || b == null)
            {
                return false;
            }
            if (a == b)
            {
                return true;
            }
            bool[,] gridA = GetTrimmedGrid(a);
            bool[,] gridB = GetTrimmedGrid(b);
            if (gridA.Length == 0 || gridB.Length == 0)
            {
                return false;
            }
            if (AreGridsEqual(gridA, gridB))
            {
                return true;
            }
            bool[,] rot90 = Rotate90(gridA);
            if (AreGridsEqual(rot90, gridB))
            {
                return true;
            }
            bool[,] rot180 = Rotate90(rot90);
            if (AreGridsEqual(rot180, gridB))
            {
                return true;
            }
            bool[,] rot270 = Rotate90(rot180);
            if (AreGridsEqual(rot270, gridB))
            {
                return true;
            }
            return false;
        }
        private static bool[,] GetTrimmedGrid(ShapeData shape)
        {
            if (shape == null || shape.Grid == null || shape.Rows == 0 || shape.Columns == 0)
            {
                return new bool[0, 0];
            }
            int minRow = int.MaxValue;
            int maxRow = int.MinValue;
            int minCol = int.MaxValue;
            int maxCol = int.MinValue;
            for (int r = 0; r < shape.Rows; r++)
            {
                if (shape.Grid[r] == null || shape.Grid[r].Column == null)
                {
                    continue;
                }
                for (int c = 0; c < shape.Columns; c++)
                {
                    if (c < shape.Grid[r].Column.Length && shape.Grid[r].Column[c])
                    {
                        if (r < minRow) minRow = r;
                        if (r > maxRow) maxRow = r;
                        if (c < minCol) minCol = c;
                        if (c > maxCol) maxCol = c;
                    }
                }
            }
            if (minRow > maxRow || minCol > maxCol)
            {
                return new bool[0, 0];
            }
            int rows = maxRow - minRow + 1;
            int cols = maxCol - minCol + 1;
            bool[,] result = new bool[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int origR = minRow + r;
                    int origC = minCol + c;
                    result[r, c] = shape.Grid[origR].Column[origC];
                }
            }
            return result;
        }
        private static bool[,] Rotate90(bool[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            bool[,] rotated = new bool[cols, rows];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    rotated[c, rows - 1 - r] = matrix[r, c];
                }
            }
            return rotated;
        }
        private static bool AreGridsEqual(bool[,] a, bool[,] b)
        {
            int rowsA = a.GetLength(0);
            int colsA = a.GetLength(1);
            int rowsB = b.GetLength(0);
            int colsB = b.GetLength(1);
            if (rowsA != rowsB || colsA != colsB)
            {
                return false;
            }
            for (int r = 0; r < rowsA; r++)
            {
                for (int c = 0; c < colsA; c++)
                {
                    if (a[r, c] != b[r, c])
                    {
                        return false;
                    }
                }
            }
            return true;
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
        private List<Sprite> GetDistinctRandomSprites(int count)
        {
            List<Sprite> result = new(count);
            if (m_squareVisuals == null || m_squareVisuals.Length == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    result.Add(null);
                }
                return result;
            }
            List<Sprite> pool = new();
            for (int i = 0; i < count; i++)
            {
                if (pool.Count == 0)
                {
                    pool.AddRange(m_squareVisuals);
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
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        private void ClearAllSlots()
        {
            if (m_currentShapes == null)
            {
                m_currentShapes = new Shape[m_slots.Length];
                return;
            }
            for (int i = 0; i < m_currentShapes.Length; i++)
            {
                if (m_currentShapes[i] == null)
                {
                    continue;
                }
                PoolManager.Ins.ReturnShape(m_currentShapes[i]);
                m_currentShapes[i] = null;
            }
        }
        public void ResetShapes()
        {
            ClearAllSlots();
            SpawnDistinctShapesForAllSlots();
        }
        public int GetShapeDataIndex(ShapeData data) => Array.IndexOf(m_allShape, data);
        public ShapeData GetShapeDataByIndex(int index) => (index >= 0 && index < m_allShape.Length) ? m_allShape[index] : null;
        public int GetSpriteIndex(Sprite sprite) => Array.IndexOf(m_squareVisuals, sprite);
        public Sprite GetSpriteByIndex(int index) => (index >= 0 && index < m_squareVisuals.Length) ? m_squareVisuals[index] : null;
        public List<int> GetCurrentShapeIndices()
        {
            List<int> result = new();
            for (int i = 0; i < m_currentShapes.Length; i++)
            {
                result.Add(m_currentShapes[i] == null ? -1 : GetShapeDataIndex(m_currentShapes[i].CurrentShapeData));
            }
            return result;
        }
        public List<int> GetCurrentSpriteIndices()
        {
            List<int> result = new();
            for (int i = 0; i < m_currentShapes.Length; i++)
            {
                result.Add(m_currentShapes[i] == null ? -1 : GetSpriteIndex(m_currentShapes[i].CurrentSprite));
            }
            return result;
        }
        public void RestoreShapes(List<int> shapeIndices, List<int> spriteIndices)
        {
            ClearAllSlots();
            if (shapeIndices == null || spriteIndices == null)
            {
                return;
            }
            for (int i = 0; i < m_slots.Length && i < shapeIndices.Count; i++)
            {
                int shapeIdx = shapeIndices[i];
                if (shapeIdx < 0)
                {
                    continue;
                }
                ShapeData data = GetShapeDataByIndex(shapeIdx);
                Sprite sprite = i < spriteIndices.Count ? GetSpriteByIndex(spriteIndices[i]) : null;
                if (data != null)
                {
                    SpawnShapeAtSlot(i, data, sprite);
                }
            }
        }
    }
}