/// 작성자: 고승로
/// 작성일: 2020-08-25
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    /// <summary>
    /// UI 관련 씬 데이터를 처리하는 클래스
    /// </summary>
    public class UIForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type { get => VisualType.UI; }
        public bool IsFinish { get => true; }

        private GameObject curUI;
        [SerializeField] private GameObject FNI_Record;
        
        private GameObject recordButton;

        /// <summary>
        /// 데이터에 맞게 해당 UI 셋팅
        /// </summary>
        /// <param name="option"></param>
        public void Active(CutData option)
        {
            if (option.uiOption.objPath == "")
            {
                //Debug.Log("<color=red> UI 경로가 입력되지 않았습니다. </color>");
            }

            curUI = gameObject.transform.Find(option.uiOption.objPath).gameObject;

            switch (option.uiOption.uiType)
            {
                case UIType.None:
                    break;
                case UIType.Button:
                    ButtonActiveEvent(option.uiOption.nextScene);
                    break;
                case UIType.Active:
                    ActiveEvent(option.uiOption.isActive);
                    break;
                case UIType.AllInactive:
                    AllInactiveEvent();
                    break;
                case UIType.Animation:
                    AnimationEvent(option.uiOption.uiAnimationOption);
                    break;
                case UIType.Transform:
                    TransformEvent(option.uiOption.Position, option.uiOption.Rotation);
                    break;
                case UIType.Record_Button:
                    recordButton = FNI_Record.transform.Find(option.uiOption.recordName).gameObject;
                    RecordButtonActiveEvent(option.uiOption.nextScene);
                    break;
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

        #region UI Event

        private void TransformEvent(Vector3 position, Vector3 rotation)
        {
            curUI.GetComponent<RectTransform>().anchoredPosition3D = position;
            curUI.transform.eulerAngles = rotation;
        }

        /// <summary>
        /// UI 버튼에 다음 씬 넘어가는 이벤트 달아주는 함수
        /// </summary>
        /// <param name="nextScene"></param>
        private void ButtonActiveEvent(SceneData nextScene)
        {
            ButtonSub btn = curUI.GetComponent<ButtonSub>();
            btn.AddNextSceneEvent(nextScene);
        }
        /// <summary>
        /// FNI_Record 에 있는 버튼에 씬 넘어가는 이벤트 달아주는 함수
        /// </summary>
        /// <param name="nextScene"></param>
        private void RecordButtonActiveEvent(SceneData nextScene)
        {
            ButtonSub btn = recordButton.GetComponent<ButtonSub>();
            btn.AddNextSceneEvent(nextScene);
        }

        /// <summary>
        /// 해당 UI 키거나 꺼주는 함수
        /// </summary>
        /// <param name="isActive"></param>
        private void ActiveEvent(bool isActive)
        {
            curUI.SetActive(isActive);
        }

        /// <summary>
        /// 해당 UI 바로 아래 자식들까지 꺼주는 함수
        /// </summary>
        private void AllInactiveEvent()
        {
            foreach (Transform child in curUI.transform)
            {
                child.gameObject.SetActive(false);
            }

        }

        private void AnimationEvent(UIAnimationOption aniOption)
        {
            switch (aniOption.aniType)
            {
                case UIAnimationType.Move:
                    StartCoroutine(UIAnimations.Instance.UpAnimation(curUI.GetComponent<RectTransform>(), aniOption));
                    break;
                case UIAnimationType.TextFadeInOut:
                    StartCoroutine(UIAnimations.Instance.AlphaAnimation(aniOption.startA, aniOption.endA, curUI.GetComponent<Text>(), aniOption.time));
                    break;
                case UIAnimationType.ImageFadeInOut:
                    StartCoroutine(UIAnimations.Instance.AlphaAnimation(aniOption.startA, aniOption.endA, curUI.GetComponent<Image>(), aniOption.time));
                    break;
                case UIAnimationType.CanvasFadeInOut:
                    StartCoroutine(UIAnimations.Instance.AlphaAnimation(aniOption.startA, aniOption.endA, curUI.GetComponent<CanvasGroup>(), aniOption.time));
                    break;
                case UIAnimationType.ChangeImage:
                    UIAnimations.Instance.ChangeSprite(curUI.GetComponent<Image>(), aniOption.changeSprite);
                    break;
                case UIAnimationType.ChangeText:
                    UIAnimations.Instance.ChangeText(curUI.GetComponent<Text>(), aniOption.changeText);
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}