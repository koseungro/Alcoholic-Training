/// 작성자: 고승로
/// 작성일: 2020-08-28
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CurvedUI;

namespace FNI
{
    /// <summary>
    /// FadeInOut 관련 씬 데이터를 처리하는 클래스
    /// </summary>
    public class FadeInOutForSequence : MonoBehaviour, IVisualObject
    {
        private static FadeInOutForSequence _instance;
        public static FadeInOutForSequence Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FadeInOutForSequence>();
                }
                return _instance;
            }
        }



        public VisualType Type { get => VisualType.FadeInOut; }

        private bool isFinish = false;
        public bool IsFinish { get => isFinish; }

        public AudioSource N_audioSource;

        public CanvasGroup[] canvasGroups = new CanvasGroup[3];


        /// <summary>
        /// FadeInOut 별로 각자 값 초기화
        /// </summary>
        /// <param name="option"></param>
        public void Active(CutData option)
        {
            // 나레이션이 재생중이면 Stop
            if(N_audioSource.isPlaying)
            {
                N_audioSource.Stop();
                N_audioSource.clip = null;
            }

            StartCoroutine(AlphaAnimation(option.fadeOption));
            isFinish = true;
        }

        public void Init()
        {
        }

        /// <summary>
        /// FadeInOut이 끝나면 isFinish true 변경 후 다음 컷씬
        /// </summary>
        public void MyUpdate()
        {

        }

        public void HMDStop()
        {
            canvasGroups[1].alpha = 1.0f;
        }

        public IEnumerator AlphaAnimation(FadeOption fadeOption)
        {
            float checktime = 0.0f;
            float maxTime = fadeOption.time;
            CanvasGroup curCanvas = canvasGroups[(int)fadeOption.uiCanvasType];

            while (checktime < 1.0f)
            {
                checktime += Time.deltaTime / maxTime;
                curCanvas.alpha = Mathf.Lerp(fadeOption.startAlpha, fadeOption.endAlpha, checktime);

                canvasGroups[1].interactable = false;
                yield return null;
            }
            
            // UI Canvas가 완전 보이고, 배경이 모두 투명할 때만 UI Canvas의 버튼 클릭 가능하도록
            if (canvasGroups[1].alpha == 1 && canvasGroups[2].alpha == 0)
            {
                canvasGroups[1].interactable = true;
            }
        }
    }
}