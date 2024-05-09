/// 작성자: 고승로
/// 작성일: 2020-09-03
/// 수정일: 2020-10-23
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    public static class GlobalStorage
    {
        /// <summary>
        /// 현재 유저이름(지금은 시작한 시간 저장)
        /// </summary>
        public static string userName = "TEST";

        /// <summary>
        /// 현재 유저 성별
        /// </summary>
        public static GenderType userGenderType = GenderType.Woman;

        public static ScoreData myScore = new ScoreData();
    }

    public class ScoreData
    {
        public EPType epType;
        public string name;
        public int score1;
        public int score2;
        public int totalScore;
        public int count;
        public bool isFirst;
    }

}