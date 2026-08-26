using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class UILoading : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_loadingTxt;
        [SerializeField] private Image m_fillImg;

        private Sequence m_loadingSeq;
        private Sequence m_fillSeq;
        private float m_jumpTime;

        private void OnDisable()
        {
            m_loadingSeq?.Kill();
            m_fillSeq?.Kill();
        }
        public void Show()
        {
            gameObject.SetActive(true);
            PlayLoadingTextEffect();
            PlayFillProgressEffect();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        private void PlayLoadingTextEffect()
        {
            m_loadingSeq?.Kill();
            m_loadingTxt.ForceMeshUpdate();
            m_jumpTime = 0f;
            m_loadingSeq = DOTween.Sequence();
            m_loadingSeq.SetLoops(-1, LoopType.Restart);
            m_loadingSeq.SetUpdate(true);
            m_loadingSeq.OnUpdate(UpdateJumpText);
        }
        private void UpdateJumpText()
        {
            m_jumpTime += Time.deltaTime;
            m_loadingTxt.ForceMeshUpdate();
            TMP_TextInfo textInfo = m_loadingTxt.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                {
                    continue;
                }
                int matIndex = charInfo.materialReferenceIndex;
                int vertIndex = charInfo.vertexIndex;
                Vector3[] verts = textInfo.meshInfo[matIndex].vertices;
                float t = (m_jumpTime - i * 0.08f) % 1f;
                if (t < 0f)
                {
                    t += 1f;
                }
                float offsetY = 0f;
                if (t <= 0.25f)
                {
                    offsetY = Mathf.Sin((t / 0.25f) * Mathf.PI) * 20f;
                }
                Vector3 offset = new(0f, offsetY, 0f);
                verts[vertIndex + 0] += offset;
                verts[vertIndex + 1] += offset;
                verts[vertIndex + 2] += offset;
                verts[vertIndex + 3] += offset;
            }
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                m_loadingTxt.UpdateGeometry(meshInfo.mesh, i);
            }
        }
        private void StopLoadingTextEffect()
        {
            if (m_loadingSeq != null)
            {
                m_loadingSeq?.Kill();
                m_loadingSeq = null;
            }
            m_loadingTxt.ForceMeshUpdate();
        }    
        private void PlayFillProgressEffect()
        {
            m_fillSeq?.Kill();
            m_fillImg.fillAmount = 0f;
            m_fillSeq = DOTween.Sequence();
            m_fillSeq.Append(m_fillImg.DOFillAmount(1f, 3f).SetEase(Ease.Linear));
            m_fillSeq.OnComplete(() =>
            {
                StopLoadingTextEffect();
                DOVirtual.DelayedCall(1f, () =>
                {
                    UIManager.Ins.ChangeState(UIType.Mainmenu);
                });
            });
        }    
    }
}