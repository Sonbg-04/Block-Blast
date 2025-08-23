using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class CellSlot : MonoBehaviour
    {
        public Vector2Int cellPosOnGrid;
        public bool isBlockOnCell;

        private readonly List<GameObject> m_squareOnCells = new();
        private readonly Dictionary<GameObject, (Sprite sprite, Color color)> m_originalStateHover = new();

        public void SetHighLight(Sprite sp, float alpha)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null && sp != null)
            {
                sr.sprite = sp;
                var c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }
        public void ResetHighlight()
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            var grid = GridManager.GetIns<GridManager>();
            var blockSlot = grid.blockSlotPrefab;
            var spriteBlockslot = blockSlot.GetComponentInChildren<SpriteRenderer>().sprite;

            if (sr != null)
            {
                sr.sprite = spriteBlockslot;
                sr.color = Color.white;
            }

        }
        public void SetHighLightSquares(GameObject square, float alpha)
        {
            if (square == null)
            {
                return;
            }

            var srclone = square.GetComponentInChildren<SpriteRenderer>();
            if (srclone == null)
            {
                return;
            }

            foreach (var s in m_squareOnCells)
            {
                if (s == null)
                {
                    continue;
                }

                var sr = s.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    if (!m_originalStateHover.ContainsKey(s))
                    {
                        m_originalStateHover[s] = (sr.sprite, sr.color);
                    }

                    sr.sprite = srclone.sprite;
                    var cl = sr.color;
                    cl.a = alpha;
                    sr.color = cl;
                }
            }
        }
        public void ResetHighlightSquares()
        {
            foreach (var kvp in m_originalStateHover)
            {
                var s = kvp.Key;
                if (s == null)
                {
                    continue;
                }

                var sr = s.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    var (originalSprite, originalColor) = kvp.Value;
                    sr.sprite = originalSprite;
                    sr.color = originalColor;
                }
            }

            m_originalStateHover.Clear();
        }
        public void SetSquareOnCell(GameObject square)
        {
            if (square == null)
            {
                return;
            }

            if (!m_squareOnCells.Contains(square))
            {
                m_squareOnCells.Add(square);
            }
        }
        public void ClearSquareOnCell()
        {
            if (m_squareOnCells.Count == 0)
            {
                return;
            }

            foreach (var square in m_squareOnCells)
            {
                if (square != null)
                {
                    square.SetActive(false);
                }
            }

            m_squareOnCells.Clear();
            m_originalStateHover.Clear();
        }

    }
}
