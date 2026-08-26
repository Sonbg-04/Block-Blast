using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonn.BlockBlast
{
    public class Square : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_sr;
        [SerializeField] private SpriteRenderer m_highLightSr;

        private SortingGroup m_squareSort;
        private Sequence m_clearSeq;
        private Color m_squareColor;
        private int m_originalOrderInLayer;

        private void Awake()
        {
            m_squareSort = GetComponent<SortingGroup>();
            m_originalOrderInLayer = m_squareSort.sortingOrder;
        }
        public void SetSprite(Sprite sp)
        {
            m_sr.sprite = sp;
        }
        public void SetTempOrderInLayer(int amount)
        {
            m_squareSort.sortingOrder = m_originalOrderInLayer + amount;
        }    
        public void RestoreOrderInLayer()
        {
            m_squareSort.sortingOrder = m_originalOrderInLayer;
        }    
        public void SetSquareColor(Color color)
        {
            m_squareColor = color;
        }
        public void SetHighLight(bool active, Color cl)
        {
            if (active)
            {
                m_highLightSr.color = cl;
            }
            m_highLightSr.gameObject.SetActive(active);
        }
        public void PlayClearEffect(Action onComplete, float delay = 0f)
        {
            m_clearSeq?.Kill();
            transform.localScale = Vector3.one;
            SetSrAlpha(1f);
            m_clearSeq = DOTween.Sequence();
            if (delay > 0f)
            {
                m_clearSeq.AppendInterval(delay);
            }
            m_clearSeq.Append(transform.DOPunchScale(Vector3.one * 0.2f, 0.15f, 6, 0.6f));
            m_clearSeq.Join(m_sr.DOFade(0f, 0.18f).SetDelay(0.05f));
            m_clearSeq.Append(transform.DOScale(0f, 0.12f).SetEase(Ease.InBack));
            m_clearSeq.OnComplete(() =>
            {
                m_clearSeq = null;
                onComplete?.Invoke();
            });
        }
        private void SetSrAlpha(float a)
        {
            if (m_sr == null)
            {
                return;
            }
            Color c = m_sr.color;
            c.a = a;
            m_sr.color = c;
        }
        public void ClearSquareEffect()
        {
            m_clearSeq?.Kill();
            m_clearSeq = null;
            transform.localScale = Vector3.one;
            SetSrAlpha(1f);
        }
    }
}