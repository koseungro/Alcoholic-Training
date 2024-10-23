/// 작성자: 고승로
/// 작성일: 2021-03-12
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace FNI
{
    public class DBManager : MonoBehaviour
    {

        private static DBManager _instance;
        public static DBManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<DBManager>();
                }
                return _instance;
            }
        }
        private string dataFolderName = "";
        public string DataFolderName { get => dataFolderName; }

        private string scoreData = "";

        private List<Score> scoreList = new List<Score>();
        public List<Score> ScoreList { get => scoreList; }

        private List<Duration> durationList = new List<Duration>();

        private int[] num = new int[16];

        private void Start()
        {
            for (int i = 0; i < num.Length; i++)
            {
                num[i] = 1;
            }
        }

        /// <summary>
        /// 사전검사 CSV파일을 생성합니다.
        /// </summary>
        /// <param name="path">.csv 문자를 포함한 경로</param>
        public void WritePreCSV(string path, string sceneID)
        {
            StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8);

            writer.WriteLine("이름 (코드), 설명, 참고 1, 검사 시간");
            //writer.WriteLine("\n\r");

            writer.Write(GetScoreToCSV(sceneID));

            writer.Close();
            writer.Dispose();
        }

        /// <summary>
        /// 생성된 사전검사 CSV파일에 사후검사 정보를 덮어씁니다.
        /// </summary>
        /// <param name="path">.csv 문자를 포함한 경로</param>
        public void WritePostCSV(string path, string sceneID)
        {
            StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8);

            writer.Write(CollectData(sceneID));

            writer.Close();
            writer.Dispose();
        }

        /// <summary>
        /// CSV파일에 저장될 데이터
        /// </summary>
        /// <returns></returns>
        private string CollectData(string sceneID)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(GetScoreToCSV(sceneID));
            builder.Append("\n\r");

            // 음성녹음파일 관련 데이터
            for (int _cnt = 0; _cnt < 4; _cnt++)
            {
                List<Duration> durations = durationList.FindAll(x => x.ID.Contains($"{sceneID}-O{_cnt + 1}"));

                for (int __cnt = 0; __cnt < durations.Count; __cnt++)
                {
                    builder.Append(durations[__cnt].ToCSV(__cnt + 1));
                    builder.Append("\n\r");
                }

                builder.Append("\n\r");
            }

            return builder.ToString();
        }

        /// <summary>
        /// CSV파일에 저장될 갈망감 척도 점수 데이터
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        private string GetScoreToCSV(string sceneID)
        {
            StringBuilder builder = new StringBuilder();

            Score pre = scoreList.Find(x => x.ID.Contains($"{sceneID} PRE"));
            Score post = scoreList.Find(x => x.ID.Contains($"{sceneID} POST"));

            switch (scoreList.Count)
            {
                case 1:
                    builder.Append(pre.ToCSV());
                    builder.Append("\n\r");
                    break;
                case 2:
                    builder.Append(post.ToCSV());
                    builder.Append("\n\r");

                    string _scoreChange;
                    float changingValue = pre.score - post.score;
                    float _changingValue = Math.Abs(changingValue);

                    if (pre.score > post.score)
                        _scoreChange = "감소";
                    else if (post.score > pre.score)
                        _scoreChange = "증가";
                    else
                        _scoreChange = "";

                    string preMinusPost = $"{sceneID} PRE-POST SCORE, {sceneID} 갈망감 사전-사후 평가 점수 변화값, {_changingValue} {_scoreChange}";

                    builder.Append(preMinusPost);
                    builder.Append("\n\r");
                    break;
            }
            return builder.ToString();
        }

        // 스코어 리스트에 사용자 점수를 추가
        public void AddScore(Score add)
        {
            scoreList.Add(add);

            //foreach (var item in scoreList)
            //{
            //    item.Log();
            //}
        }

        public void AddDuration(Duration add)
        {
            durationList.Add(add);
        }

        // 사용자 데이터를 초기화
        public void ResetData()
        {
            scoreList.Clear();
            durationList.Clear();

            for (int i = 0; i < num.Length; i++)
            {
                num[i] = 1;
            }
        }

        // 갈망감 점수가 저장될 경로와 이름을 지정해서 csv 파일을 생성합니다.
        public void ScoreCheckSceneID()
        {
            string testTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string _scoreData;

            switch (Main.curSceneID)
            {
                case "S1 PRE":
                    CreateDataFolder();
                    _scoreData = dataFolderName + "/Score/S1 Data_" + testTime + ".csv";
                    WritePreCSV(_scoreData, "S1");
                    scoreData = _scoreData;
                    break;
                case "S1 POST":
                    WritePostCSV(scoreData, "S1");
                    scoreData = "";
                    break;
                case "S2 PRE":
                    CreateDataFolder();
                    _scoreData = dataFolderName + "/Score/S2 Data_" + testTime + ".csv";
                    WritePreCSV(_scoreData, "S2");
                    scoreData = _scoreData;
                    break;
                case "S2 POST":
                    WritePostCSV(scoreData, "S2");
                    scoreData = "";
                    break;
                case "S3 PRE":
                    CreateDataFolder();
                    _scoreData = dataFolderName + "/Score/S3 Data_" + testTime + ".csv";
                    WritePreCSV(_scoreData, "S3");
                    scoreData = _scoreData;
                    break;
                case "S3 POST":
                    WritePostCSV(scoreData, "S3");
                    scoreData = "";
                    break;
                case "S4 PRE":
                    CreateDataFolder();
                    _scoreData = dataFolderName + "/Score/S4 Data_" + testTime + ".csv";
                    WritePreCSV(_scoreData, "S4");
                    scoreData = _scoreData;
                    break;
                case "S4 POST":
                    WritePostCSV(scoreData, "S4");
                    scoreData = "";
                    break;
            }
        }

        // 음성 녹음될 파일 이름 지정
        public string CheckSceneID()
        {
            string recordName = "";
            string name = Main.curSceneID + "-N";
            string testTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            switch (Main.curSceneID)
            {
                case "S1-O1":
                    recordName = name + num[0]++ + "_" + testTime;
                    break;
                case "S1-O2":
                    recordName = name + num[1]++ + "_" + testTime;
                    break;
                case "S1-O3":
                    recordName = name + num[2]++ + "_" + testTime;
                    break;
                case "S1-O4":
                    recordName = name + num[3]++ + "_" + testTime;
                    break;
                case "S2-O1":
                    recordName = name + num[4]++ + "_" + testTime;
                    break;
                case "S2-O2":
                    recordName = name + num[5]++ + "_" + testTime;
                    break;
                case "S2-O3":
                    recordName = name + num[6]++ + "_" + testTime;
                    break;
                case "S2-O4":
                    recordName = name + num[7]++ + "_" + testTime;
                    break;
                case "S3-O1":
                    recordName = name + num[8]++ + "_" + testTime;
                    break;
                case "S3-O2":
                    recordName = name + num[9]++ + "_" + testTime;
                    break;
                case "S3-O3":
                    recordName = name + num[10]++ + "_" + testTime;
                    break;
                case "S3-O4":
                    recordName = name + num[11]++ + "_" + testTime;
                    break;
                case "S4-O1":
                    recordName = name + num[12]++ + "_" + testTime;
                    break;
                case "S4-O2":
                    recordName = name + num[13]++ + "_" + testTime;
                    break;
                case "S4-O3":
                    recordName = name + num[14]++ + "_" + testTime;
                    break;
                case "S4-O4":
                    recordName = name + num[15]++ + "_" + testTime;
                    break;
            }
            return recordName;
        }

        // 사용자 Data가 저장될 폴더를 생성합니다.
        private void CreateDataFolder()
        {
            if (dataFolderName == "")
            {
                string name = Application.streamingAssetsPath + "/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                Directory.CreateDirectory(name);
                dataFolderName = name;

                // 갈망감 점수가 저장될 폴더를 생성합니다.
                if (!Directory.Exists(dataFolderName + "/Score"))
                {
                    Directory.CreateDirectory(dataFolderName + "/Score");
                }
            }
        }
    }

    [System.Serializable]

    public struct Score
    {
        public DateTime inputTime;
        public string ID;
        public float score;

        public string ToCSV()
        {
            return $"{ID}-SCORE, {ID} 갈망감 평가 점수, {ID} 에서 선택한 점수 : {score}, {inputTime.ToString("HH:mm:ss")}";
        }

        public void Log()
        {
            Debug.Log($"{inputTime} , {ID}, {score}");
        }
    }

    [System.Serializable]
    public struct Duration
    {
        public DateTime inputTime;
        public string ID;
        public string length;

        public string ToCSV(int num)
        {
            return $"{ID}-N{num} DURATION, 총 말한 시간, {length}, {inputTime.ToString("HH:mm:ss")}";
        }
    }
}