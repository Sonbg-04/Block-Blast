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
        public System.Action<Shape> OnPlaced;

        private List<GameObject> m_currentShapes;
        private Vector3 m_offset,
                        m_startShapePos;
        private bool m_isDragging = false;
        private List<CellSlot> m_hoveredSlots;
        private int m_currentSortingOrder;
        private List<SpriteRenderer> m_spriteRenderers;

        public Sprite HoverSprite { get; set; }

        private void Awake()
        {
            m_currentShapes = new();
            m_hoveredSlots = new();
            m_spriteRenderers = new();
            transform.localScale = BeforeShapeScale;
            m_startShapePos = transform.localPosition;
            m_currentSortingOrder = SquarePrefab.GetComponentInChildren<SpriteRenderer>().sortingOrder;

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
                    m_offset = transform.localPosition - mousePos;

                    IncreaseSortingOrder(10);
                }

            }
            if (m_isDragging)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pos.z = 0;

                    transform.localPosition = pos + m_offset;

                    HandleHoverHighlight();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    m_isDragging = false;

                    if (m_hoveredSlots.Count > 0)
                    {
                        ClearHighlight();
                    }

                    HandleDrop();
                }
            }
        }
        private Bounds GetShapeBounds()
        {
            var cls = GetComponentsInChildren<Collider2D>();
            if (cls == null || cls.Length <= 0)
            {
                return new Bounds(transform.localPosition, Vector3.zero);
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
            {
                return;
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;

            var grid = GridManager.GetIns<GridManager>();
            grid.GridBounds(out var minWorld, out var maxWorld);

            Transform nearestChild = null;
            var minDist = float.MaxValue;
            foreach (var col in GetComponentsInChildren<Collider2D>())
            {
                var d = Vector2.Distance(pos, col.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearestChild = col.transform;
                }
            }

            if (nearestChild != null)
            {
                var nearestCell = grid.GetNearestCell(nearestChild.position);

                if (nearestCell != null)
                {
                    Vector3 offset = nearestChild.position - transform.position;
                    transform.position = nearestCell.transform.position - offset;

                    transform.localScale = AfterShapeScale;

                    bool hasCollision = false, allInside = true;

                    foreach (var col in GetComponentsInChildren<Collider2D>())
                    {
                        Vector3 worldPos = col.transform.position;
                        var slot = grid.GetNearestCell(worldPos);

                        var colBounds = col.bounds;

                        if (colBounds.min.x < minWorld.x || colBounds.max.x > maxWorld.x ||
                            colBounds.min.y < minWorld.y || colBounds.max.y > maxWorld.y)
                        {
                            allInside = false;
                            break;
                        }

                        if (slot == null || slot.isBlockOnCell)
                        {
                            hasCollision = true;
                            break;
                        }
                    }

                    if (allInside && !hasCollision)
                    {
                        var index = 0;
                        foreach (var col in GetComponentsInChildren<Collider2D>())
                        {
                            Vector3 worldPos = col.transform.position;
                            var slot = grid.GetNearestCell(worldPos);
                            if (slot != null)
                            {
                                slot.isBlockOnCell = true;
                                var sq = m_currentShapes[index];
                                slot.SetSquareOnCell(sq);
                                index++;
                            }
                        }

                        enabled = false;
                        ResetSortingOrder();
                        OnPlaced?.Invoke(this);
                        grid.CheckClearLines();
                        return;
                    }
                }
            }

            transform.localPosition = m_startShapePos;
            transform.localScale = BeforeShapeScale;
            ResetSortingOrder();

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

                var srcl = shapeClone.GetComponentInChildren<SpriteRenderer>();
                if (srcl != null)
                {
                    m_spriteRenderers.Add(srcl);
                }

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
                    {
                        number++;
                    }
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
        public void HandleHoverHighlight()
        {
            if (IsComponentNull())
            {
                return;
            }

            if (m_hoveredSlots.Count > 0)
            {
                ClearHighlight();
            }

            var grid = GridManager.GetIns<GridManager>();
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;

            List<CellSlot> tempSlots = new();
            bool hasCollision = false, allInside = true;

            grid.GridBounds(out var minWorld, out var maxWorld);

            foreach (var col in GetComponentsInChildren<Collider2D>())
            {
                Vector3 worldPos = col.transform.position;
                var slot = grid.GetNearestCell(worldPos);

                var colBounds = col.bounds;
                if (colBounds.min.x < minWorld.x || colBounds.max.x > maxWorld.x ||
                    colBounds.min.y < minWorld.y || colBounds.max.y > maxWorld.y)
                {
                    allInside = false;
                    break;
                }

                if (slot == null || slot.isBlockOnCell)
                {
                    hasCollision = true;
                }

                tempSlots.Add(slot);
            }

            if (allInside && !hasCollision && m_currentShapes.Count > 0)
            {
                var sampleShape = m_currentShapes[0];
                var previewedSlots = grid.PreviewClearLines(sampleShape, tempSlots);

                float alpha = 0.3f;
                foreach (var slot in tempSlots)
                {
                    slot.SetHighLight(HoverSprite, alpha);
                    m_hoveredSlots.Add(slot);
                }

                foreach (var slot in previewedSlots)
                {
                    slot.SetHighLight(HoverSprite, alpha);
                    m_hoveredSlots.Add(slot);
                }
            }
        }
        private void ClearHighlight()
        {
            foreach (var slot in m_hoveredSlots)
            {
                slot.ResetHighlight();
                slot.ResetHighlightSquares();
            }

            m_hoveredSlots.Clear();
        }
        private void IncreaseSortingOrder(int order)
        {
            foreach (var sr in m_spriteRenderers)
            {
                sr.sortingOrder = order;
            }
        }    
        private void ResetSortingOrder()
        {
            foreach (var sr in m_spriteRenderers)
            {
                sr.sortingOrder = m_currentSortingOrder;
            }
        }
    }
}
