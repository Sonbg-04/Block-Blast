using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class GameManager : MonoBehaviour, ISingleton, IComponentChecking
    {
        private int m_score;
        private static Dictionary<Type, MonoBehaviour> m_ins;

        public static T GetIns<T>() where T : MonoBehaviour
        {
            if (m_ins.TryGetValue(typeof(T), out var ins))
            {
                return ins as T;
            }
            return null;
        }
        public int Score { get => m_score; }

        private void Awake()
        {
            m_ins = new();
            MakeSingleton();
        }  
        public void AddScore(int score)
        {
            if (IsComponentNull())
            {
                return;
            }

            m_score += score;
            Pref.Score = m_score;
            GUIManager.GetIns<GUIManager>().UpdateScore(m_score);
        }
        public void GameOver()
        {
            if (IsComponentNull())
            {
                return;
            }    
            Pref.currentState = GameState.GameOver;
            if (Pref.Score > Pref.BestScore)
            {
                Pref.hasBestScore = true;
                Pref.BestScore = Pref.Score;
                GUIManager.GetIns<GUIManager>().UpdateBestScore(Pref.BestScore);
            }
        }    
        public void MakeSingleton()
        {
            var key = GetType();
            if (!m_ins.ContainsKey(key) || m_ins[key] == null)
            {
                m_ins[key] = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public bool IsComponentNull()
        {
            bool check = GUIManager.GetIns<GUIManager>() == null;
            if (check)
            {
                Debug.LogWarning("Có component bị null. Vui lòng kiểm tra lại!");
            }
            return check;
        }
    }
}
