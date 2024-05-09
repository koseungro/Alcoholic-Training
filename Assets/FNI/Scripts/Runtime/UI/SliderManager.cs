/// 작성자: 고승로
/// 작성일: 2021-03-12
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력



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
using UnityEngine.EventSystems;

namespace FNI
{
    public class SliderManager : MonoBehaviour
    {
        private static SliderManager _instance;
        public static SliderManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SliderManager>();
                }
                return _instance;
            }
        }

        [SerializeField] private Slider slider;
        [SerializeField] private Button OKButton;
        private AudioSource audioSource = null;
        [SerializeField] private AudioClip checkAudio = null;

        private int calCheck;
        public int CalCheck { get => calCheck; }
        private float resultScore;
        public float ResultScore { get => resultScore; }

        private void OnEnable()
        {
            Init();
        }
        private void OnDisable()
        {
            Init();
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Init()
        {
            OKButton.interactable = false;
            slider.value = 0;
        }

        public void AssessmentComplete()
        {
            Score score = new Score();

            score.inputTime = DateTime.Now;
            score.ID = Main.curSceneID;
            score.score = slider.value;

            //Debug.Log("Scene ID : " + score.ID);
            //Debug.Log("Score : " + score.score);
            DBManager.Instance.AddScore(score);
            DBManager.Instance.ScoreCheckSceneID();
        }

        public void ScoreChanged()
        {
            audioSource.clip = checkAudio;
            audioSource.Play();

            if (!OKButton.interactable)
            {
                OKButton.interactable = true;
            }
        }

    }
}