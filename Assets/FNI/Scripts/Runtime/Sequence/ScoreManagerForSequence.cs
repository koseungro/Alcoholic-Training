/// 작성자: 고승로
/// 작성일: 2021-01-13
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
    public class ScoreManagerForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.ScoreManager;

        public bool IsFinish => true;

        [SerializeField] private Main main = null;

        private string answerWriter = "";
        private float playTime = 0.0f;
        private SceneData nextScene = null;

        private string score1 = "";
        private string score2 = "";

        private EPType writeType = EPType.E01;
        private EPType curType = EPType.E01;

        public void Active(CutData option)
        {
            curType = option.scoreOption.epType;

            switch (option.scoreOption.funcType)
            {
                case ScoreManagerFuncType.SetInfoInit:
                    InitData();
                    break;
                case ScoreManagerFuncType.SetGameOver:
                    SetGameScore(option.scoreOption);
                    break;
                case ScoreManagerFuncType.WriteScore:
                    if (curType == writeType)
                        WriteScore(option.scoreOption);
                    break;
                case ScoreManagerFuncType.WriteCsv:
                    WriteCsv();
                    break;
                case ScoreManagerFuncType.Quit:
                    QuitProgram();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 프로젝트 시작시 한번만 초기화
        /// </summary>
        public void Init()
        {
            GlobalStorage.userName = SZ_FileWriter.TimeNow;
            answerWriter = "";
            answerWriter = "미션명,점수1,점수2,합산,소요시간(s)\n";
            InitData();
            writeType = EPType.E01;
        }

        public void MyUpdate()
        {
            
        }


        private void QuitProgram()
        {
            Application.Quit();
        }

        /// <summary>
        /// 데이터 초기화
        /// </summary>
        private void InitData()
        { 
            playTime = 0.0f;
            score1 = "";
            score2 = "";

            GlobalStorage.myScore.epType = EPType.E01;
            GlobalStorage.myScore.score1 = 100;
            GlobalStorage.myScore.score2 = 100;
            GlobalStorage.myScore.totalScore = 0;
            GlobalStorage.myScore.count = 0;

            StopAllCoroutines();
        }

        /// <summary>
        /// 퀴즈 시작전 셋팅 (반복하지 않는 씬에 미리 적용해주기)
        /// </summary>
        private void SetGameScore(RuleOption ruleOption)
        {    
            nextScene = ruleOption.nextScene;

            GlobalStorage.myScore.epType = ruleOption.epType;
            GlobalStorage.myScore.count = ruleOption.finishCount;

            switch (ruleOption.epType)
            {
                case EPType.E01:
                    GlobalStorage.myScore.isFirst = true;
                    DefaultScoreSetting("놀이기구 태우기");
                    break;
                case EPType.E02:
                    DefaultScoreSetting("간식먹이기");
                    break;
                case EPType.E03:
                    DefaultScoreSetting("놀이기구 태우기 recall");
                    break;
                case EPType.E04:
                    E04ScoreSetting();
                    break;
                case EPType.E05:
                    DefaultScoreSetting("친구집찾기");
                    break;
                case EPType.E06:
                    DefaultScoreSetting("모자찾기");
                    break;
                default:
                    break;
            }

            StopAllCoroutines();
            StartCoroutine(TimerCoroutine());
            StartCoroutine(GameOverCoroutine(ruleOption));
        }
        
        /// <summary>
        /// 퀴즈 끝난 후, 점수 계산 및 기록 (성공씬이나, 완전히 실패후 그냥 넘어갈때)
        /// </summary>
        private void WriteScore(RuleOption ruleOption)
        {
            switch (ruleOption.epType)
            {
                case EPType.E01:
                    E01ScoreResult();
                    break;
                case EPType.E02:
                    E02ScoreResult();
                    break;
                case EPType.E03:
                    E01ScoreResult();
                    break;
                case EPType.E04:
                    E04ScoreResult();
                    break;
                case EPType.E05:
                    break;
                case EPType.E06:
                    break;
                default:
                    break;
            }

            if (GlobalStorage.myScore.score1 == 100)
                score1 = "-";
            else
                score1 = GlobalStorage.myScore.score1.ToString();

            if (GlobalStorage.myScore.score2 == 100)
                score2 = "-";
            else
                score2 = GlobalStorage.myScore.score2.ToString();

            answerWriter += GlobalStorage.myScore.name + "," +
                            score1 + "," + score2 + "," +
                            GlobalStorage.myScore.totalScore + "," +
                            ((int)playTime).ToString() + "\n";


            Debug.Log(answerWriter);

            InitData();
            writeType++;
        }

        /// <summary>
        /// CSV 작성
        /// </summary>
        private void WriteCsv()
        {
            SZ_FileWriter.FileWriter($"{GlobalStorage.userName}.csv", answerWriter);
        }


        private IEnumerator TimerCoroutine()
        {
            while (true)
            {
                playTime += Time.deltaTime;
                yield return null;
            }
        }
        
        private IEnumerator GameOverCoroutine(RuleOption ruleOption)
        {
            //while (true)
            //{
            //    //시간으로 다음 장면 넘어갈 시
            //    if (ruleOption.isTimeOut)
            //    {
            //        if(ruleOption.finishTime <= playTime)
            //        {
            //            if(curType == writeType)
            //                WriteScore(ruleOption);

            //            playTime = ruleOption.finishTime;
            //            main.OnButtonNextSequence(nextScene);
            //        }
            //    }
            //    else //횟수로 다음 장면 넘어갈 시
            //    {
            //        if(ruleOption.epType == EPType.E04)
            //        {
            //            if(E04Button.gameIndex==1)
            //            {
            //                if(GlobalStorage.myScore.score1 == 0)
            //                {
            //                    main.OnButtonNextSequence(nextScene);
            //                    StopAllCoroutines();
            //                }
            //            }
            //            else if(E04Button.gameIndex ==2)
            //            {
            //                if (GlobalStorage.myScore.score2 == 0)
            //                {
            //                    main.OnButtonNextSequence(nextScene);

            //                    if (curType == writeType)
            //                        WriteScore(ruleOption);
            //                    else
            //                        StopAllCoroutines();
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (GlobalStorage.myScore.count == 0)
            //            {
            //                if (curType == writeType)
            //                    WriteScore(ruleOption);
            //                else
            //                    StopAllCoroutines();

            //                main.OnButtonNextSequence(nextScene);
            //            }
            //        }
                    
            //    }

            //    yield return null;
            //}
            yield return null;
        }

        private void DefaultScoreSetting(string episodeName)
        {
            GlobalStorage.myScore.name = episodeName;
            GlobalStorage.myScore.totalScore = 0;
            GlobalStorage.myScore.score1 = 100;
            GlobalStorage.myScore.score2 = 100;
        }

        private void E04ScoreSetting()
        {
            GlobalStorage.myScore.name = "꽃찾기";
            GlobalStorage.myScore.totalScore = 0;

            //if(E04Button.gameIndex == 1)
            //{
            //    GlobalStorage.myScore.score1 = 2;
            //    GlobalStorage.myScore.score2 = 2;
            //}
            
        }



        private void E01ScoreResult()
        {
                
        }

        private void E02ScoreResult()
        {
            GlobalStorage.myScore.totalScore = GlobalStorage.myScore.count;
        }

        private void E04ScoreResult()
        {
            GlobalStorage.myScore.totalScore = GlobalStorage.myScore.score1 + GlobalStorage.myScore.score2;
        }
    }
}