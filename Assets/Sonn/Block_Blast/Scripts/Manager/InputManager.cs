using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonn.BlockBlast
{
    public class InputManager : MonoBehaviour, ISingleton
    {
        public static InputManager Ins;

        [SerializeField] private LayerMask m_shapeLayer;
        [SerializeField] private float m_dragUpOffset;

        private Shape m_draggingShape;
        private Camera m_camera;
        private Transform m_dragStartParent;
        private Vector3 m_dragStartLocalPos;
        private Vector3 m_pointerToShapeOffset;
        private bool m_isDragging;

        private void Awake()
        {
            MakeSingleton();
            m_camera = Camera.main;
        }
        private void Update()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            HandleTouchInput();
#else
            HandleMouseInput();
#endif
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                TryBeginDrag(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0) && m_isDragging)
            {
                ContinueDrag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0) && m_isDragging)
            {
                EndDrag();
            }
        }
        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
            {
                return;
            }
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        break;
                    }
                    TryBeginDrag(touch.position);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (m_isDragging)
                    {
                        ContinueDrag(touch.position);
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (m_isDragging)
                    {
                        EndDrag();
                    }
                    break;
            }
        }
        private void TryBeginDrag(Vector2 screenPos)
        {
            if (m_isDragging)
            {
                return;
            }
            if (GameManager.Ins.IsGameOver)
            {
                return;
            }
            Vector3 worldPos = ScreenToWorld(screenPos);
            Collider2D hit = Physics2D.OverlapPoint(worldPos, m_shapeLayer);
            if (hit == null)
            {
                return;
            }
            Shape shape = hit.GetComponentInParent<Shape>();
            if (shape == null || !shape.CanBeDragged)
            {
                return;
            }
            m_draggingShape = shape;
            m_dragStartParent = shape.transform.parent;
            m_dragStartLocalPos = shape.transform.localPosition;
            m_pointerToShapeOffset = shape.transform.position - worldPos;
            m_isDragging = true;
            m_draggingShape.OnBeginDrag();
        }
        private void ContinueDrag(Vector2 screenPos)
        {
            if (m_draggingShape == null)
            {
                return;
            }
            Vector3 worldPos = ScreenToWorld(screenPos);
            Vector3 targetPos = worldPos + m_pointerToShapeOffset + Vector3.up * m_dragUpOffset;
            m_draggingShape.transform.position = targetPos;
            GridManager.Ins.ShowPlacementPreview(m_draggingShape);
        }
        private void EndDrag()
        {
            Shape shape = m_draggingShape;
            m_isDragging = false;
            m_draggingShape = null;
            GridManager.Ins.ClearHighLights();
            if (shape == null)
            {
                return;
            }
            bool placed = false;
            if (GridManager.Ins.TryGetPlacementCells(shape, out var targetCells))
            {
                GridManager.Ins.PlaceShapeIntoCells(shape, targetCells);
                placed = true;
            }
            if (placed)
            {
                GameManager.Ins.HandleShapePlaced(shape);
                shape.OnPlacedSuccessfully();
            }
            else
            {
                shape.transform.SetParent(m_dragStartParent, true);
                shape.transform.localPosition = m_dragStartLocalPos;
                shape.OnDragCancelled();
            }
        }
        private Vector3 ScreenToWorld(Vector2 screenPos)
        {
            Vector3 screen = new(screenPos.x, screenPos.y, -m_camera.transform.position.z);
            return m_camera.ScreenToWorldPoint(screen);
        }
    }
}