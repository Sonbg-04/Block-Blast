using DG.Tweening;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class GameManager : MonoBehaviour, ISingleton
    {
        public static GameManager Ins;

        private int m_score;
        private int m_comboCount;
        private float m_lastClearTime;

        public int Score => m_score;
        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            MakeSingleton();
            m_score = 0;
            m_lastClearTime = -999f;
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        public void HandleShapePlaced(Shape shape)
        {
            int placementScore = shape.ActiveSquares.Count;
            AddScore(placementScore);
            int clearedLines = GridManager.Ins.CheckAndClearLines();
            if (clearedLines > 0)
            {
                ProcessLineClear(clearedLines);
            }
            else
            {
                TryResetComboIfTimeOut();
            }
            ShapeManager.Ins.OnShapePlaced(shape);
            if (ShapeManager.Ins.HasAnyActiveShape())
            {
                CheckGameOver();
            }
            else
            {
                ShapeManager.Ins.TryRespawnIfNeeded();
                CheckGameOver();
            }
        }
        public void TriggerGameOver()
        {
            if (IsGameOver)
            {
                return;
            }
            IsGameOver = true;
            TrySaveBestScore();
            Debug.Log("Game over...!");
            DOVirtual.DelayedCall(1f, () =>
            {
                UIManager.Ins.GameplayUI.GameOverUI.Show();
            });
        }
        private void CheckGameOver()
        {
            if (IsGameOver)
            {
                return;
            }
            if (GridManager.Ins.HasAnyValidMove(ShapeManager.Ins.CurrentShapes))
            {
                return;
            }
            TriggerGameOver();
        }
        private void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            m_score += amount;
            UIEvent.OnScoreChanged?.Invoke(m_score);
        }   
        private void TryResetComboIfTimeOut()
        {
            if (m_comboCount > 0 && Time.time - m_lastClearTime > Const.COMBO_TIMEOUT)
            {
                m_comboCount = 0;
            }    
        }    
        private void ProcessLineClear(int totalLine)
        {
            TryResetComboIfTimeOut();
            m_comboCount++;
            m_lastClearTime = Time.time;
            int clearScore = Const.CLEAR_SCORE_UNIT * totalLine * (totalLine + 1) / 2;
            int comboBonus = (m_comboCount - 1) * Const.COMBO_BONUS_UNIT;
            AddScore(clearScore + comboBonus);
            if (GridManager.Ins.IsGridEmpty())
            {
                AddScore(Const.PERFECT_CLEAR_BONUS);
            }    
        }
        private void TrySaveBestScore()
        {
            if (m_score <= Pref.BestScore)
            {
                return;
            }
            Pref.BestScore = m_score;
        }
        private void OnApplicationQuit()
        {
            TrySaveBestScore();
        }
    }
}
