/// 작성자: 고승로
/// 작성일: 2021-01-12
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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FNI
{
    public class GenderButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {

        public void OnPointerEnter(PointerEventData eventData)
        {
            EffectAudioManager.Instance.PlaySoundEffect(EffectAudioClip.ButtonHover);
        }

        public void OnPointerClickEvent()
        {
            if(gameObject.name  == "m")
            {
                GlobalStorage.userGenderType = GenderType.Man;
            }
            else
            {
                GlobalStorage.userGenderType = GenderType.Woman;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickEvent();
            EffectAudioManager.Instance.PlaySoundEffect(EffectAudioClip.ButtonClick);
        }
    }
}