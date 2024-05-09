/// 작성자:고승로
/// 작성일: 2021-03-16
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace FNI
{
    public class VoiceRecorder : MonoBehaviour
    {
        private static VoiceRecorder _instance = null;

        public static VoiceRecorder Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VoiceRecorder>();
                }
                return _instance;
            }
        }

        private Main myMain;
        private Main MyMain
        {
            get
            {
                if (myMain == null)
                    myMain = GameObject.Find("---------------Scripts/Main").GetComponent<Main>();

                return myMain;
            }
        }

        private FNI_Record record = null;

        [SerializeField] private VideoPlayer video;
        [SerializeField] private GameObject VREventSystem;
        [SerializeField] private GameObject GameEventSystem;

        public Button[] button = new Button[3];

        // 현재 녹음 중인지를 확인합니다.
        private bool isRecording = false;
        public bool IsRecording { get => isRecording; }

        // 영상이 반복되도 OK
        private bool isRepeatOK = true;
        public bool IsRepeatOK { get => isRepeatOK; }

        // 한 제의에서 사용자가 녹음을 완료했는지를 확인합니다.
        private bool recordCheck = false;

        private CutData cData;

        private void OnEnable()
        {
            VREventSystem.gameObject.SetActive(false);
            GameEventSystem.gameObject.SetActive(true);
            RecordInit();
            CallRecordBoard();
        }

        private void OnDisable()
        {
            VREventSystem.gameObject.SetActive(true);
            GameEventSystem.gameObject.SetActive(false);
            RecordInit();
        }

        private void Start()
        {
            record = GetComponent<FNI_Record>();
        }

        private void LateUpdate()
        {
            // 녹음기가 켜져있을 때만 동작
            if (record.enabled == true)
            {
                // Space 키보드를 통하여 녹음 시작, 정지가 가능하도록 합니다.
                if (Input.GetKeyDown("space"))
                {
                    if (!recordCheck)
                    {
                        if (!isRecording)
                        {
                            FNI_Record.Instance.Start_Record();
                        }
                        else if (isRecording)
                        {
                            StopRecording();
                        }
                    }
                }

                // 녹음이 끝난 후 방향키를 누르면 다음 씬(제의)으로 넘어갑니다.
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (recordCheck)
                    {
                        int cnt = myMain.CurSequence.cutDataList.Count - 3;
                        cData = myMain.CurSequence.cutDataList[cnt];
                        myMain.ChangeScene(cData.uiOption.nextScene);
                    }
                }
            }
        }

        private void RecordInit()
        {
            isRecording = false;
            recordCheck = false;
            isRepeatOK = true;
            for (int i = 0; i < button.Length; i++)
            {
                button[i].interactable = false;
            }
        }

        // 녹음시 사용되는 코루틴
        private IEnumerator m_recordWaiting_Routine;

        // 녹음 기능 켜기
        public void CallRecordBoard()
        {
            if (m_recordWaiting_Routine != null)
                StopCoroutine(m_recordWaiting_Routine);
            m_recordWaiting_Routine = RecordWaiting_Routine();
            StartCoroutine(m_recordWaiting_Routine);
        }

        // 녹음 기능
        public IEnumerator RecordWaiting_Routine()
        {
            yield return new WaitForSeconds(0.2f);

            if (!isRecording)
            {
                //패널을 보여줍니다.
                FNI_Record.Instance.Show(RecordType.Default, 0, true, false, ReplayType.Last);
                FNI_Record.Instance.ReplayShow();

                //녹음 시작하기를 기다립니다.
                while (!FNI_Record.Instance.IsRecording)
                {
                    //Debug.Log("녹음을 시작해 주세요" + isRecording);
                    yield return null;
                }
            }
            //녹음이 시작되면 녹음 끝나기를 기다립니다.
            while (FNI_Record.Instance.IsRecording)
            {
                if (!isRecording)
                    isRecording = true;
                yield return null;
                //Debug.Log("녹음 중 입니다." + isRecording);
            }

            //녹음이 끝난 후 오디오 클립을 저장하고 저장한 경로를 받아옵니다.
            string fullPath = FNI_Record.Instance.SaveClip(DBManager.Instance.DataFolderName + "/Recording");
            //Debug.Log(fullPath);

            FNI_Record.Instance.RecordingTime.text
                = string.Format("{0:00}:{1:00}", FNI_Record.Instance.Progress.Minutes, FNI_Record.Instance.Progress.Seconds);
            //Debug.Log(FNI_Record.Instance.RecordingTime.text);

            button[0].interactable = true;
            button[1].interactable = true;
        }

        private IEnumerator m_replay_Routine;
        public void CallReplayBoard()
        {
            if (m_replay_Routine != null)
                StopCoroutine(m_replay_Routine);
            m_replay_Routine = Replay_Routine();
            StartCoroutine(m_replay_Routine);
        }

        // 다시듣기
        private IEnumerator Replay_Routine()
        {
            //다시 듣기 버튼을 활성화 한다.
            FNI_Record.Instance.RecordReady(RecordType.Replay);

            //재생이 시작하기를 기다립니다.
            while (!FNI_Record.Instance.IsPlaying)
            {
                yield return null;
                //Debug.Log("다시듣기 재생 대기 중");
            }
            //재생 시작되면 재생 끝나기를 기다립니다.
            while (FNI_Record.Instance.IsPlaying)
            {
                yield return null;
                //Debug.Log("다시듣기 재생 중");
            }

        }
        private void StartRecording()
        {
            Debug.Log("녹음 시작");
            FNI_Record.Instance.Start_Record();
        }

        public void StopRecording()
        {
            Duration duration = new Duration();

            FNI_Record.Instance.Stop_Record(DBManager.Instance.CheckSceneID());
            video.Pause();

            MyMain.EndCut();

            // 영상이 더 이상 반복되지 않도록
            isRepeatOK = false;
            SZ_VideoPlayer.Instance.IsRepeating = false;

            duration.inputTime = DateTime.Now;
            duration.ID = Main.curSceneID;
            duration.length = FNI_Record.Instance.RecordingTime.text;

            DBManager.Instance.AddDuration(duration);

            CallReplayBoard();
            recordCheck = true;
            Debug.Log("녹음 완료");
        }
    }
}