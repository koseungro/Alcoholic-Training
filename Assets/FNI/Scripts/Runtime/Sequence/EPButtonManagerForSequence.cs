/// 작성자: 고승로
/// 작성일: 2020-12-18
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

namespace FNI
{
    public class EPButtonManagerForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.EPButtonManager;

        public bool IsFinish => true;

        [SerializeField] private Transform parents = null;

        public void Active(CutData option)
        {
            switch (option.epOption.epType)
            {
                default:
                    break;
            }

        }

        public void Init()
        {
           
        }

        public void MyUpdate()
        {
        
        }

        private void DefaultActive(string epType, MainEPOption mainEpOption)
        {
            EButtonBase[] eButtons = parents.Find(epType+"/select").gameObject.GetComponentsInChildren<EButtonBase>();

            foreach (EButtonBase child in eButtons)
            {
                child.Init(mainEpOption);
            }
        }


        private void E05E06Active(string epType, string buttonType, MainEPOption mainEpOption)
        {
            EButtonBase[] eButtons = parents.Find(epType + "/select" + buttonType).gameObject.GetComponentsInChildren<EButtonBase>();

            foreach (EButtonBase child in eButtons)
            {
                child.Init(mainEpOption);
            }
        }
    }
}