/// 작성자: 고승로
/// 작성일: 2020-12-23
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
    public class EButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private AudioClip hoverClip = null;
        [SerializeField] private AudioClip clickClip = null;
        [SerializeField] protected Main main = null;

        private void Reset()
        {
#if UNITY_EDITOR            
            audioSource = GetComponent<AudioSource>();
            hoverClip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/EffectSound0.wav", typeof(AudioClip));
            clickClip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/FNI/Res/Sound/ButtonClick.mp3", typeof(AudioClip));
            main = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();
#endif
        }

        public virtual void Init(MainEPOption epOption)
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {

            // Fade 효과가 끝났을 때에만 버튼이 인터렉션 가능하도록
            if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1
                && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0)
            {
                audioSource.clip = hoverClip;
                audioSource.Play();
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (FadeInOutForSequence.Instance.canvasGroups[1].alpha == 1
                && FadeInOutForSequence.Instance.canvasGroups[2].alpha == 0)
            {
                audioSource.clip = clickClip;
                audioSource.Play();
            }
        }
    }
}