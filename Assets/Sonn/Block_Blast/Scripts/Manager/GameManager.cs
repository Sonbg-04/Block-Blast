using UnityEngine;

namespace Sonn.BlockBlast
{
    public class GameManager : MonoBehaviour, ISingleton
    {
        public static GameManager Ins;

        private int m_score;
        private int m_comboCount;
        private float m_lastClearTime = -999f;

        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            MakeSingleton();
            m_score = Pref.Score;
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        public void HandleShapePlaced(Shape shape)
        {
            int placementScore = shape.ActiveSquares.Count;
            AddScore(placementScore);
            int clearedLines =  GridManager.Ins.CheckAndClearLines();
            if (clearedLines > 0)
            {
                ProcessLineClear(placementScore, clearedLines);
            }    
            else
            {
                TryResetComboIfTimeOut();
            }    
            ShapeManager.Ins.OnShapePlaced(shape);
            CheckGameOver();
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
            IsGameOver = true;
            Debug.Log("Game over...!");
        }
        private void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            m_score += amount;
            Pref.Score = m_score;
        }   
        private void TryResetComboIfTimeOut()
        {
            if (m_comboCount > 0 && Time.time - m_lastClearTime > Const.COMBO_TIMEOUT)
            {
                m_comboCount = 0;
            }    
        }    
        private void ProcessLineClear(int placementScore, int totalLine)
        {
            TryResetComboIfTimeOut();
            m_comboCount++;
            m_lastClearTime = Time.time;
            int clearScore = Const.CLEAR_SCORE_UNIT * totalLine * (totalLine + 1) / 2;
            int comboBonus = (m_comboCount - 1) * Const.COMBO_BONUS_UNIT;
            AddScore(placementScore + clearScore + comboBonus);
            if (GridManager.Ins.IsGridEmpty())
            {
                AddScore(placementScore + clearScore + comboBonus + Const.PERFECT_CLEAR_BONUS);
            }    
        }
    }
}
