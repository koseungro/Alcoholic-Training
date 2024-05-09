/// 작성자: 고승로
/// 작성일: 2020-08-24
/// 수정일: 2020-08-27
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

/// ┌────────────────컷진행도─────────────────────────────────────────────────────┐
/// │                                                                            │
/// │start (한번): 신규 컷을 실행 합니다.                                          │
/// │update(반복): 컷이 Finish 될때까지 반복됩니다.                                │
/// │Check  (한번): 다음 컷이 있는지 확인하고 있으면 Start 없으면 End로 넘어갑니다.  │ 
/// │End (한번): 다음 시퀀스가 존재하면 Start 로 이동합니다.                        │
/// │                                                                            │
/// │        ┌──────────◁────────┐                                              │
/// │     ┌──┴───┐  ┌──────┐  ┌───┴───┐  ┌──────┐                                │
/// │     │Start ├▷┤Update├▷┤ Check ├▷┤  End │                                │
/// │     └──┬───┘  └─────┘   └───────┘   └──────┘                               │
/// │        └────────────◁─────────────────┘                                   │
/// └────────────────────────────────────────────────────────────────────────────┘

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FNI
{
    public class Main : MonoBehaviour
    {
        public static string curSceneID;
        /// <summary>
        /// 현재 시퀀스 데이터
        /// </summary>
        [SerializeField] private SceneData curSequence;
        public SceneData CurSequence { get => curSequence; }
        [SerializeField] private SceneData returnSceneData;
        [SerializeField] private GameObject vr3D_UI;
        [SerializeField] private GameObject fni_Record;
        private GameObject VREventSystem;

        /// <summary>
        /// 시퀀스 데이터에 맞게 실행 시켜줄 오브젝트 리스트
        /// </summary>
        private List<IVisualObject> cutObject_List = new List<IVisualObject>();

        [Space(20)]
        [SerializeField]
        private float hmdResetTime = 10.0f;
        [SerializeField]
        private SceneData hmdResetSceneData;
        private float hmdResetCheckTime = 0.0f;
        private float PAUSE_TIMESCAPLE = 1.0f;
        private bool isHmdPlay;
        private bool onceCheck = false;

        /// <summary>
        /// 현재 시퀀스 상태
        /// </summary>
        private SequenceState sequenceState;


        /// <summary>
        /// 현재 컷 Index
        /// </summary>
        private int cutIndex = 0;
        public int CutIndex { get => cutIndex; }
        /// <summary>
        /// Wait관련 시간변수
        /// </summary>
        private float CheckTime = 0;

        /// <summary>
        /// 현재 진행중인 컷 데이터
        /// </summary>
        private CutData cur_CutData;
        /// <summary>
        /// 현재 컷 오브젝트
        /// </summary>
        private IVisualObject cur_VisualObj;


        private void Awake()
        {
            sequenceState = SequenceState.Setting;

            Setting();

            sequenceState = SequenceState.Start;
        }


        private void Update()
        {
            HmdCheckUpdate();

            if (isHmdPlay)
            {
                switch (sequenceState)
                {
                    case SequenceState.Start: CutStart(); break;
                    case SequenceState.Update: CutUpdate(); break;
                    case SequenceState.Check: Check(); break;
                    case SequenceState.End: End(); break;
                    default: break;
                }
            }

            // 오른쪽 방향키를 누르면 다음 컷으로 넘어갑니다.
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // 영상이 재생 중이 아닐 때는 다음 컷으로
                if (!SZ_VideoPlayer.Instance.MyVideoPlayer.isPlaying)
                {
                    //Debug.Log($"<color=yellow> {cutIndex} </color>");
                    sequenceState = SequenceState.Check;
                }
                // 영상이 재생 중일때는 다음 컷(제의)으로
                else
                {
                    if (fni_Record.activeSelf == true)
                    {
                        // 만약 녹음이 진행중일 경우 Stop
                        if (VoiceRecorder.Instance.IsRecording)
                        {
                            VoiceRecorder.Instance.StopRecording();
                        }
                        // 녹음기만 켜져있는 경우
                        else
                        {
                            int cnt = curSequence.cutDataList.Count - 3;
                            CutData cData = curSequence.cutDataList[cnt];

                            SZ_VideoPlayer.Instance.MyVideoPlayer.Pause();
                            ChangeScene(cData.uiOption.nextScene);
                        }
                    }
                    // 녹음기가 켜져있지 않고 영상만 진행중일 때
                    else
                    {
                        int cnt = curSequence.cutDataList.Count - 3;
                        CutData cData = curSequence.cutDataList[cnt];

                        SZ_VideoPlayer.Instance.MyVideoPlayer.Pause();
                        ChangeScene(cData.uiOption.nextScene);
                    }                    
                }
            }

            // ESC 키를 누르면 상황선택화면으로 돌아갑니다.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 만약 영상이 재생중일 경우 Stop
                if (SZ_VideoPlayer.Instance.MyVideoPlayer.isPlaying)
                {
                    SZ_VideoPlayer.Instance.MyVideoPlayer.Pause();
                }

                // 만약 녹음이 진행중일 경우 Stop
                if (fni_Record.activeSelf == true)
                {
                    if (VoiceRecorder.Instance.IsRecording)
                        VoiceRecorder.Instance.StopRecording();
                }

                // 만약 저장한 데이터가 있을 경우 초기화
                DBManager.Instance.ResetData();

                if (curSequence != returnSceneData)
                {
                    ChangeSequence(returnSceneData);
                    cutIndex = 0;

                    sequenceState = SequenceState.Start;
                }

            }
        }

        #region Cut Loop

        /// <summary>
        /// 컷 처음 시작할때 1회 실행하는 함수입니다. 
        /// </summary>
        private void CutStart()
        {
            cur_CutData = GetCutData(cutIndex);
            if (cur_CutData.visualType != VisualType.Wait &&
                cur_CutData.visualType != VisualType.Line &&
                cur_CutData.visualType != VisualType.Data &&
                cur_CutData.visualType != VisualType.Record &&
                cur_CutData.visualType != VisualType.Quit &&
                cur_CutData.visualType != VisualType.Custom)
            {
                cur_VisualObj = cutObject_List.Find(x => x.Type == cur_CutData.visualType);
                cur_VisualObj.Active(cur_CutData);
            }

            sequenceState = SequenceState.Update;
            CheckTime = 0;
        }

        /// <summary>
        /// Start 이후 반복적으로 Update 하는 함수입니다.
        /// </summary>
        private void CutUpdate()
        {
            if (cur_CutData.visualType == VisualType.Wait)
            {
                Wait();
            }
            else if (cur_CutData.visualType == VisualType.Data)
            {
                Data();
            }
            // UI 배경 역할을 하는 VR3D_UI를 숨깁니다.
            else if (cur_CutData.visualType == VisualType.Custom)
            {
                VR3D_UIState(cur_CutData.VR3D_UIActive);
            }
            // 녹음 기능을 키고 끕니다.
            else if (cur_CutData.visualType == VisualType.Record)
            {
                FNI_RecordState(cur_CutData.FNI_Record);
            }
            // 콘텐츠를 종료합니다.
            else if (cur_CutData.visualType == VisualType.Quit)
            {
                Application.Quit();
            }
            else if (!cur_VisualObj.IsFinish)
            {
                cur_VisualObj.MyUpdate();
            }
            else
                sequenceState = SequenceState.Check;

        }

        /// <summary>
        /// 다음 컷이 있는지 체크합니다. 있으면 -> Start / 없다면 -> End
        /// </summary>
        private void Check()
        {
            if (cutIndex < curSequence.cutDataList.Count - 1)
            {
                cutIndex++;
            }
            else
            {
                sequenceState = SequenceState.End;
                return;
            }
            sequenceState = SequenceState.Start;
        }

        /// <summary>
        /// 다음 시퀀스가 있는지 체크합니다. 있으면 -> 새로운 시퀀스로 start 
        /// </summary>
        private void End()
        {
            if (curSequence.nextScene != null)
            {
                ChangeSequence(curSequence.nextScene);
                cutIndex = 0;

                sequenceState = SequenceState.Start;
            }
        }

        /// <summary>
        /// 반복 중인 영상 멈추고 다음 컷으로
        /// </summary>
        public void EndCut()
        {
            sequenceState = SequenceState.Check;
            End(); // ?
        }

        #endregion

        #region Cut Function

        /// <summary>
        /// Wait Update 함수
        /// </summary>
        private void Wait()
        {
            CheckTime += Time.deltaTime;
            if (CheckTime > cur_CutData.waitTime)
            {
                CheckTime = 0.0f;
                sequenceState = SequenceState.Check;
            }
        }

        private void Data()
        {
            sequenceState = SequenceState.Check;
        }

        private void VR3D_UIState(bool isActive)
        {
            vr3D_UI.gameObject.SetActive(isActive);
            sequenceState = SequenceState.Check;
        }

        private void FNI_RecordState(bool isActive)
        {
            fni_Record.SetActive(isActive);
            sequenceState = SequenceState.Check;
        }
        #endregion

        #region Other Utility

        /// <summary>
        /// 필요한 데이터를 할당하고 초기화합니다.
        /// </summary>
        private void Setting()
        {
            FNIVR_HMDManager.hmdPlayAction += HmdPlayEvnet;
            FNIVR_HMDManager.hmdPauseAction += HmdPauseEvent;
            PAUSE_TIMESCAPLE /= FNIVR_HMDManager.Instance.PAUSE_TIMESCALE;

            VREventSystem = GameObject.Find("FNIVR_EventSystem");
            cutObject_List.Add(GameObject.Find("---------------UI").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/FadeInOutForSequence").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/Narration").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Video").GetComponent<IVisualObject>());
            cutObject_List.Add(GameObject.Find("---------------Scripts/LayForSequence").GetComponent<IVisualObject>());

            for (int i = 0; i < cutObject_List.Count; i++)
            {
                cutObject_List[i].Init();
            }
        }

        /// <summary>
        /// 현재 시퀀스 컷을 추출합니다./// </summary>
        /// <param name="cutIndex">추출할 컷 index</param>
        /// <returns></returns>
        public CutData GetCutData(int cutIndex)
        {
            return curSequence.cutDataList[cutIndex];
        }


        /// <summary>
        /// 버튼이벤트를 통한 다음 시퀀스
        /// </summary>
        /// <param name="scene"></param>
        public void OnButtonNextSequence(SceneData scene)
        {
            ChangeSequence(scene);
            cutIndex = 0;

            sequenceState = SequenceState.Start;
        }

        /// <summary>
        /// Hmd 일시정지 이벤트
        /// </summary>
        private void HmdPauseEvent()
        {
            isHmdPlay = false;
            hmdResetCheckTime = 0.0f;

            // HMD를 중간에 벗으면 Record Canvas 끄기
            if (fni_Record.gameObject.activeSelf == true)
            {
                FNI_Record.Instance.transform.GetChild(0).gameObject.SetActive(false);
            }

            if (VREventSystem.activeSelf == true && fni_Record.activeSelf == false)
            {
                VREventSystem.SetActive(false);
            }
        }

        /// <summary>
        /// Hmd 시작 이벤트
        /// </summary>
        private void HmdPlayEvnet()
        {
            isHmdPlay = true;

            // HMD를 다시 쓰면 Record Canvas 켜기
            if (fni_Record.gameObject.activeSelf == true)
            {
                FNI_Record.Instance.transform.GetChild(0).gameObject.SetActive(true);
            }

            if (VREventSystem.activeSelf == false && fni_Record.activeSelf == false)
            {
                VREventSystem.SetActive(true);
            }
        }

        private void HmdCheckUpdate()
        {
            if (!isHmdPlay)
            {
                if (onceCheck)
                    hmdResetCheckTime += Time.deltaTime * PAUSE_TIMESCAPLE;
                else
                    onceCheck = true;

                if (hmdResetCheckTime > hmdResetTime)
                {
                    NarrationForSequence narration = (NarrationForSequence)cutObject_List.Find(x => x.Type == VisualType.Narration);
                    narration.HMDStop();

                    FadeInOutForSequence fadeInOut = (FadeInOutForSequence)cutObject_List.Find(x => x.Type == VisualType.FadeInOut);
                    fadeInOut.HMDStop();

                    ChangeSequence(hmdResetSceneData);
                    cutIndex = 0;
                    sequenceState = SequenceState.Start;
                }
            }
        }
        #endregion
        private void ChangeSequence(SceneData data)
        {
            curSequence = data;
            curSceneID = curSequence.sceneID;
        }
        public void ChangeScene(SceneData scene)
        {
            ChangeSequence(scene);
            cutIndex = 0;

            sequenceState = SequenceState.Start;
        }
    }
}