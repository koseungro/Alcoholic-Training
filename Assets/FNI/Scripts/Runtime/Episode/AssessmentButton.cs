
/// 작성자: 작성자 이름
/// 작성일: 2021-03-12
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
using UnityEngine.EventSystems;

namespace FNI
{
    public class AssessmentButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private Button button = null;
        [SerializeField] private AudioClip hoverClip = null;
        [SerializeField] private AudioClip clickClip = null;
        [SerializeField] protected Main main = null;

        private void Reset()
        {
#if UNITY_EDITOR
            button = GetComponent<Button>();
            audioSource = GetComponent<AudioSource>();
            hoverClip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/EffectSound0.wav", typeof(AudioClip));
            clickClip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/ButtonClick.mp3", typeof(AudioClip));
            main = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

            if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1
                && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0)
            {
                if (button.interactable)
                {
                    audioSource.clip = hoverClip;
                    audioSource.Play();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1
                && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0)
            {
                if (button.interactable)
                {
                    audioSource.clip = clickClip;
                    audioSource.Play();
                }
            }
        }
    }
}