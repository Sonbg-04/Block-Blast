using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

namespace Sonn.BlockBlast
{
    public class Shape : MonoBehaviour, IComponentChecking
    {
        public GameObject SquarePrefab;
        public ShapeData currentShapeData;
        public Vector3 BeforeShapeScale, AfterShapeScale;

        private List<GameObject> m_currentShapes;
        private Vector3 m_offset,
                        m_startShapePos;
        private bool m_isDragging = false;

        private void Awake()
        {
            m_currentShapes = new();
            transform.localScale = BeforeShapeScale;
            m_startShapePos = transform.position;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;

                var shapeBounds = GetShapeBounds();
                if (shapeBounds.Contains(mousePos))
                {
                    m_isDragging = true;
                    transform.localScale = AfterShapeScale;
                    m_offset = transform.position - mousePos;
                }
            }
            if (m_isDragging)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pos.z = 0;

                    transform.position = pos + m_offset;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    m_isDragging = false;
                    HandleDrop();
                }
            }

        }
        private Bounds GetShapeBounds()
        {
            var cls = GetComponentsInChildren<Collider2D>();
            if (cls == null || cls.Length <= 0)
            {
                return new Bounds(transform.position, Vector3.zero);
            }
            
            var bounds = cls[0].bounds;
            foreach (var cl in cls)
            {
                bounds.Encapsulate(cl.bounds);
            }    

            return bounds;
        }
        private void HandleDrop()
        {
            if (IsComponentNull())
                return;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;

            var grid = GridManager.GetIns<GridManager>();
            grid.GridBounds(out var minWorld, out var maxWorld);

            bool insideGrid = pos.x >= minWorld.x && pos.x <= maxWorld.x &&
                              pos.y >= minWorld.y && pos.y <= maxWorld.y;

            if (insideGrid)
            {
                // Tìm block con gần nhất với con trỏ
                Transform nearestChild = null;
                float minDist = float.MaxValue;
                foreach (var col in GetComponentsInChildren<Collider2D>())
                {
                    float d = Vector2.Distance(pos, col.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        nearestChild = col.transform;
                    }
                }

                if (nearestChild != null)
                {
                    // Cell gần nhất với block con đó
                    CellSlot nearestCell = grid.GetNearestCell(nearestChild.position);
                    if (nearestCell != null)
                    {
                        // offset giữa pivot và block con
                        Vector3 offset = nearestChild.position - transform.position;

                        // snap pivot sao cho block con khớp cell
                        transform.position = nearestCell.transform.position - offset;

                        transform.localScale = AfterShapeScale;

                        // check collision + set isBlockOnCell
                        bool hasCollision = false;
                        foreach (var col in GetComponentsInChildren<Collider2D>())
                        {
                            Vector3 worldPos = col.transform.position;
                            CellSlot slot = grid.GetNearestCell(worldPos);

                            if (slot == null || slot.isBlockOnCell)
                            {
                                hasCollision = true;
                                break;
                            }
                        }

                        if (!hasCollision)
                        {
                            foreach (var col in GetComponentsInChildren<Collider2D>())
                            {
                                Vector3 worldPos = col.transform.position;
                                CellSlot slot = grid.GetNearestCell(worldPos);
                                if (slot != null)
                                {
                                    slot.isBlockOnCell = true;
                                }
                            }
                            enabled = false;
                            return;
                        }
                    }
                }
            }

            // reset nếu không đặt được
            transform.position = m_startShapePos;
            transform.localScale = BeforeShapeScale;
        }
        public void CreateShape(ShapeData sd)
        {
            currentShapeData = sd;

            int totalSquareNumber = GetNumberOfSquares(sd);

            while (m_currentShapes.Count < totalSquareNumber)
            {
                var shapeClone = Instantiate(SquarePrefab, Vector3.zero, Quaternion.identity);
                shapeClone.transform.SetParent(transform, false);
                shapeClone.transform.localScale = new Vector3(0.95f, 0.95f, 1.0f);
                m_currentShapes.Add(shapeClone);
            }

            foreach (var sq in m_currentShapes)
            {
                sq.SetActive(false);
            }

            var sr = SquarePrefab.GetComponentInChildren<SpriteRenderer>();
            Vector2 squareSize = sr.bounds.size;
            int currentIndex = 0;

            for (int row = 0; row < sd.Rows; row++)
            {
                for (int col = 0; col < sd.Columns; col++)
                {
                    if (sd.Grid[row].Column[col])
                    {
                        m_currentShapes[currentIndex].SetActive(true);

                        float posX = col * squareSize.x;
                        float posY = -row * squareSize.y;

                        m_currentShapes[currentIndex].transform.localPosition = new Vector2(posX, posY);

                        currentIndex++;
                    }
                }
            }

            if (currentIndex > 0)
            {
                Vector3 min = m_currentShapes[0].transform.localPosition;
                Vector3 max = min;

                foreach (var sq in m_currentShapes)
                {
                    if (!sq.activeSelf)
                    {
                        continue;
                    }

                    Vector3 lp = sq.transform.localPosition;

                    min = Vector3.Min(min, lp);
                    max = Vector3.Max(max, lp);
                }

                Vector3 center = (min + max) / 2f;

                foreach (var sq in m_currentShapes)
                {
                    if (!sq.activeSelf)
                    {
                        continue;
                    }

                    sq.transform.localPosition -= center;
                }
            }
        }
        private int GetNumberOfSquares(ShapeData sd)
        {
            int number = 0;
            foreach (var rowData in sd.Grid)
            {
                foreach (var active in rowData.Column)
                {
                    if (active)
                        number++;
                }
            }
            return number;
        }
        public bool IsComponentNull()
        {
            bool check = GridManager.GetIns<GridManager>() == null;
            if (check)
            {
                Debug.LogWarning("Có component bị null. Vui lòng kiểm tra lại!");
            }
            return check;
        }
        
    }
}
