using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class PoolManager : MonoBehaviour, ISingleton
    {
        public static PoolManager Ins;

        [Header("Cell")]
        [SerializeField] private Cell m_cellPrefab;
        [SerializeField] private int m_cellPoolSize;

        [Header("Square")]
        [SerializeField] private Square m_squarePrefab;
        [SerializeField] private int m_squarePoolSize;

        [Header("Shape")]
        [SerializeField] private Shape m_shapePrefab;
        [SerializeField] private int m_shapePoolSize;

        private readonly Queue<Cell> m_cellPools = new();
        private readonly Queue<Square> m_squarePools = new();
        private readonly Queue<Shape> m_shapePools = new();

        private void Awake()
        {
            MakeSingleton();
            InitCellPool();
            InitSquarePool();
            InitShapePool();
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        private void InitCellPool()
        {
            for (int i = 0; i < m_cellPoolSize; i++)
            {
                Cell cell = Instantiate(m_cellPrefab, transform);
                cell.gameObject.SetActive(false);
                m_cellPools.Enqueue(cell);
            }
        }
        private void InitSquarePool()
        {
            for (int i = 0; i < m_squarePoolSize; i++)
            {
                Square square = Instantiate(m_squarePrefab, transform);
                square.gameObject.SetActive(false);
                m_squarePools.Enqueue(square);
            }
        }
        private void InitShapePool()
        {
            for (int i = 0; i < m_shapePoolSize; i++)
            {
                Shape shape = Instantiate(m_shapePrefab, transform);
                shape.gameObject.SetActive(false);
                m_shapePools.Enqueue(shape);
            }
        }
        public Cell GetCell()
        {
            Cell cell = m_cellPools.Count > 0 ? m_cellPools.Dequeue() : Instantiate(m_cellPrefab, transform);
            cell.gameObject.SetActive(true);
            return cell;
        }
        public Square GetSquare()
        {
            Square square = m_squarePools.Count > 0 ? m_squarePools.Dequeue() : Instantiate(m_squarePrefab, transform);
            square.gameObject.SetActive(true);
            return square;
        }
        public Shape GetShape()
        {
            Shape shape = m_shapePools.Count > 0 ? m_shapePools.Dequeue() : Instantiate(m_shapePrefab, transform);
            shape.gameObject.SetActive(true);
            return shape;
        }
        //public void ReturnCell(Cell c)
        //{
        //    if (c == null)
        //    {
        //        return;
        //    }
        //    c.gameObject.SetActive(false);
        //    c.transform.SetParent(transform);
        //    c.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //    c.transform.localScale = Vector3.one;
        //    m_cellPools.Enqueue(c);
        //}    
        public void ReturnSquare(Square square)
        {
            square.gameObject.SetActive(false);
            square.transform.SetParent(transform);
            square.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            square.transform.localScale = Vector3.one;
            m_squarePools.Enqueue(square);
        }
        public void ReturnShape(Shape shape)
        {
            if (shape == null)
            {
                return;
            }
            shape.ClearShape();
            shape.gameObject.SetActive(false);
            shape.transform.SetParent(transform);
            shape.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            shape.transform.localScale = Vector3.one;
            m_shapePools.Enqueue(shape);
        }
    }
}