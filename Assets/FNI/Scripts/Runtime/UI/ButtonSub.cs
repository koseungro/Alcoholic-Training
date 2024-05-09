/// 작성자: 고승로
/// 작성일: 2020-09-04
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
    public class ButtonSub : MonoBehaviour
    {
        private Button myButton;
        private Button MyButton
        {
            get
            {
                if (myButton == null)
                    myButton = GetComponent<Button>();

                return myButton;
            }
        }

        private Main myMain;
        private Main MyMain
        {
            get
            {
                if(myMain ==null)
                    myMain = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();

                return myMain;
            }
        }
               
        private bool SceneMoveFunc = false;

        private void OnDisable()
        {
            SceneMoveFunc = false;
        }

        /// <summary>
        /// 이벤트가 들어있지 않다면 이벤트 추가
        /// </summary>
        /// <param name="nextScene">넘어갈 씬</param>
        public void AddNextSceneEvent(SceneData nextScene)
        {
            if(!SceneMoveFunc)
            {
                MyButton.onClick.AddListener(delegate { MyMain.OnButtonNextSequence(nextScene); });
                SceneMoveFunc = true;
            }
        }
    }
}