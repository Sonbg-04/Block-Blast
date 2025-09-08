using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class GUIManager : MonoBehaviour, ISingleton
    {
        public TextMeshProUGUI scoreTxt, bestscoreTxt, 
            timingTxt, rotateCountTxt, comboTxt;
        public Image greatImg;

        private float m_countdownTime = 120f;
        private static Dictionary<Type, MonoBehaviour> m_ins;
        private Coroutine m_coroutineComboTxt;

        public static T GetIns<T>() where T : MonoBehaviour
        {
            if (m_ins.TryGetValue(typeof(T), out var ins))
            {
                return ins as T;
            }
            return null;
        }

        private void Awake()
        {
            m_ins = new();
            MakeSingleton();
        }
        private void Start()
        {
            RandomRequireScore();
        }
        private void Update()
        {
            UpdateTimer();
        }
        private void RandomRequireScore()
        {
            if (bestscoreTxt)
            {
                int randomScore = UnityEngine.Random.Range(399, 501);
                bestscoreTxt.text = randomScore.ToString("0000");
            }    
        }    
        private void UpdateTimer()
        {
            if (m_countdownTime > 0)
            {
                m_countdownTime -= Time.deltaTime;

                if (m_countdownTime < 0)
                {
                    m_countdownTime = 0;
                }

                int seconds = Mathf.FloorToInt(m_countdownTime);
                int fractions = Mathf.FloorToInt((m_countdownTime - seconds) * 60f);

                if (timingTxt)
                {
                    timingTxt.text = string.Format("{0:00}:{1:00}", seconds, fractions);
                }
            }
        }
        public void UpdateCombo(int combo)
        {
            if (comboTxt)
            {
                if (combo >= 1)
                {
                    comboTxt.text = $"Combo x{combo}";

                    if (m_coroutineComboTxt != null)
                    {
                        StopCoroutine(m_coroutineComboTxt);
                    }

                    m_coroutineComboTxt = StartCoroutine(ComboTxtActiveCoroutine());
                }    
                else
                {
                    comboTxt.text = "";
                    comboTxt.gameObject.SetActive(false);
                }
            }    
        }    
        public void ShowGreatTxtImg()
        {
            if (greatImg)
            {
                StartCoroutine(GreatTxtImgActiveCoroutine());
            }    
        }
        IEnumerator GreatTxtImgActiveCoroutine()
        {
            greatImg.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            greatImg.gameObject.SetActive(false);
        }
        IEnumerator ComboTxtActiveCoroutine()
        {
            comboTxt.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.667f);
            comboTxt.gameObject.SetActive(false);
        }    
        public void UpdateScore(int score)
        {
            if (scoreTxt)
            {
                scoreTxt.text = score.ToString();
            }    
        }   
        public void UpdateBestScore(int bestScore)
        {
            if (bestscoreTxt)
            {
                bestscoreTxt.text = bestScore.ToString();
            }
        }
        public void UpdateRotateCount(int rotateCount)
        {
            if (rotateCountTxt)
            {
                rotateCountTxt.text = rotateCount.ToString();
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
    }
}

