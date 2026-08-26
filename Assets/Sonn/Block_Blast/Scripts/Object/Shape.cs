using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class Shape : MonoBehaviour
    {
        [SerializeField] private ShapeData m_currentShape;
        [SerializeField] private ColorData m_colorData;
        [SerializeField] private Sprite m_currentSprite;
        [SerializeField] private float m_squareSize;
        [SerializeField] private bool m_canBeDragged;

        private Vector3 m_originalLocalScale;
        private Color m_currentColor;
        private readonly List<Square> m_activeSquares = new();
        private readonly List<Vector2Int> m_cellOffsets = new();
        
        public Color CurrentColor => m_currentColor;
        public bool CanBeDragged => m_canBeDragged && m_currentShape != null;
        public IReadOnlyList<Square> ActiveSquares => m_activeSquares;
        public IReadOnlyList<Vector2Int> CellOffsets => m_cellOffsets;

        private void Start()
        {
            GenerateShape(m_currentShape, m_currentSprite);
        }
        public void SetShape(ShapeData shape, Sprite sprite)
        {
            GenerateShape(shape, sprite);
        }
        private void GenerateShape(ShapeData shape, Sprite sprite)
        {
            ClearCurrentSquares();
            m_currentShape = shape;
            m_currentSprite = sprite;
            m_currentColor = m_colorData.GetColorForSprite(sprite);
            m_cellOffsets.Clear();
            if (shape == null || shape.Grid == null)
            {
                return;
            }
            for (int row = 0; row < shape.Rows; row++)
            {
                for (int col = 0; col < shape.Columns; col++)
                {
                    if (!shape.Grid[row].Column[col])
                    {
                        continue;
                    }
                    m_cellOffsets.Add(new Vector2Int(col, row));
                    Square square = PoolManager.Ins.GetSquare();
                    if (square == null)
                    {
                        return;
                    }
                    square.transform.SetParent(transform, false);
                    square.ClearSquareEffect();
                    Vector3 localPos = GetLocalPosForCell(row, col, shape);
                    square.transform.SetLocalPositionAndRotation(localPos, Quaternion.identity);
                    ApplySprite(square, sprite);
                    m_activeSquares.Add(square);
                }
            }
        }
        private void ApplySprite(Square square, Sprite sprite)
        {
            if (square == null || sprite == null)
            {
                return;
            }
            square.SetSprite(sprite);
            square.SetSquareColor(m_currentColor);
        }
        private Vector3 GetLocalPosForCell(int row, int col, ShapeData shape)
        {
            float offsetX = -(shape.Columns - 1) * m_squareSize / 2f;
            float offsetY = (shape.Rows - 1) * m_squareSize / 2f;
            float x = offsetX + col * m_squareSize;
            float y = offsetY - row * m_squareSize;
            return new Vector3(x, y, 0f);
        }
        public void ClearShape()
        {
            ClearCurrentSquares();
            m_currentShape = null;
            m_currentSprite = null;
            m_cellOffsets.Clear();
            m_canBeDragged = true;
        }
        private void ClearCurrentSquares()
        {
            for (int i = 0; i < m_activeSquares.Count; i++)
            {
                PoolManager.Ins.ReturnSquare(m_activeSquares[i]);
            }
            m_activeSquares.Clear();
        }
        private void SetSquaresTempOrderInLayer(int extraOrder)
        {
            for (int i = 0; i < m_activeSquares.Count; i++)
            {
                m_activeSquares[i].SetTempOrderInLayer(extraOrder);
            }
        }
        private void RestoreSquaresOrderInLayer()
        {
            for (int i = 0; i < m_activeSquares.Count; i++)
            {
                m_activeSquares[i].RestoreOrderInLayer();
            }
        }
        public void OnBeginDrag()
        {
            m_canBeDragged = false;
            m_originalLocalScale = transform.localScale;
            transform.localScale = Vector3.one;
            SetSquaresTempOrderInLayer(1);
        }
        public void OnPlacedSuccessfully()
        {
            RestoreSquaresOrderInLayer();
            m_activeSquares.Clear();
            m_canBeDragged = false;
            PoolManager.Ins.ReturnShape(this);
        }
        public void OnDragCancelled()
        {
            m_canBeDragged = true;
            transform.localScale = m_originalLocalScale;
            RestoreSquaresOrderInLayer();
        }
    }
}