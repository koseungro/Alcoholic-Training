/// 작성자: 고승로
/// 작성일: 2021-03-19
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    public class ScoreChangeManager : MonoBehaviour
    {
        public Text[] Scores
        { 
            get
            {
                if (scores ==null || scores.Length == 0)
                    scores = gameObject.GetComponentsInChildren<Text>();

                return scores;
            }
        }
        [SerializeField] private Text[] scores;
        [SerializeField] private Slider slider;

        public void ScoreChange()
        {
            int score;
            score = Mathf.RoundToInt(slider.value);

            Scores[score].fontStyle = FontStyle.Bold;
            Scores[score].fontSize = 23;

            // 점수가 선택되지 않은 score들은 다시 normal로
            for (int i = 0; i < Scores.Length; i++)
            {
                if (Scores[i] != Scores[score])
                {
                    Scores[i].fontStyle = FontStyle.Normal;
                    Scores[i].fontSize = 21;
                }
            }
        }
    }
}