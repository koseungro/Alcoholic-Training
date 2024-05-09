using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{

    public class RTest : MonoBehaviour
    {
        private IEnumerator m_recordWaiting_Routine;

        public void CallRecordBoard()
        {
            if (m_recordWaiting_Routine != null)
                StopCoroutine(m_recordWaiting_Routine);
            m_recordWaiting_Routine = RecordWaiting_Routine();
            StartCoroutine(m_recordWaiting_Routine);
        }

        private IEnumerator RecordWaiting_Routine()
        {
            //패널을 보여줍니다.
            FNI_Record.Instance.Show(); //기본 RecordType.Default
                                        //다른 타입 예시
                                        //FNI_Record.Instance.Show(RecordType.TimeLimit, 180);
                                        //FNI_Record.Instance.Show(RecordType.SoftTimeLimit, 180);
                                        //FNI_Record.Instance.Show(RecordType.AutoStart, 0, true, true);

            //Show의 옵션에 isAcitveStart를 True로 만들면 호출 할 필요가 없을 수도 있습니다.
            FNI_Record.Instance.RecordReady();

            //녹음 시작하기를 기다립니다.
            while (!FNI_Record.Instance.IsRecording)
                yield return null;
            //녹음이 시작되면 녹음 끝나기를 기다립니다.
            while (FNI_Record.Instance.IsRecording)
                yield return null;

            //녹음이 끝난 후 오디오 클립을 저장하고 저장한 경로를 받아옵니다.
            //오디오 클립명은 시간으로 자동생성되도록 되어 있습니다.
            string fullPath = FNI_Record.Instance.SaveClip(Application.streamingAssetsPath + "/MiddlePath/");

            //녹음이 끝나면 숨깁니다.
            //옵션에 따라 필요 없을 수도 있습니다.
            FNI_Record.Instance.Hide();

            //녹음 시작과 종료는 보여지는 레코드 판넬에 있으며 따로 컨트롤 하지 않아도 됩니다.
        }

        private IEnumerator m_replay_Routine;
        public void CallReplayBoard()
        {
            if (m_replay_Routine != null)
                StopCoroutine(m_replay_Routine);
            m_replay_Routine = Replay_Routine();
            StartCoroutine(m_replay_Routine);
        }
        /// <summary>
        /// 다시 듣기 사용 법
        /// </summary>
        /// <returns></returns>
        private IEnumerator Replay_Routine()
        {
            //녹음 패널을 보여준다.
            FNI_Record.Instance.Show(ReplayType.Custom, 2);
            //다른 타입 예시
            //FNI_Record.Instance.Show(ReplayType.First);
            //FNI_Record.Instance.Show(ReplayType.Last);

            //다시 듣기 버튼을 활성화 한다.
            FNI_Record.Instance.RecordReady(RecordType.Replay);

            //재생이 시작하기를 기다립니다.
            while (!FNI_Record.Instance.IsPlaying)
                yield return null;
            //재생 시작되면 재생 끝나기를 기다립니다.
            while (FNI_Record.Instance.IsPlaying)
                yield return null;

            //녹음 패널을 숨긴다.
            //재생이 완료 되면 자동으로 숨겨짐
            //Todo: 종료 후 해야 할 일 할 것
        }
    }
}
