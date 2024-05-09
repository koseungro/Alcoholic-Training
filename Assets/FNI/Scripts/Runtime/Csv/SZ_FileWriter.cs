/// 작성자: 고승로
/// 작성일: 2021-01-13
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace FNI
{
     public static class SZ_FileWriter
    {

        /// <summary>
        /// 현재 시간을 지정한 형식의 문자열 형태로 가져온다.
        /// </summary>
        public static string TimeNow
        {
            get
            {
                //return System.DateTime.Now.ToString("HH:mm:ss.fff");
                return System.DateTime.Now.ToString("HH시mm분ss초유저");
            }
        }

        /// <summary>
        /// 로그가 저장될 기본위치
        /// </summary>
        private static string GetLogPath
        {
            get
            {
                return string.Format("TestData");
            }
        }

        /// <summary>
        /// 오늘의 폴더명
        /// </summary>
        private static string GetTodayFolder
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMdd");
            }
        }


        /// <summary>
        /// 파일을 생성해주는 함수
        /// </summary>
        /// <param name="fileName">생성할 파일 이름 (ex: test.csv)</param>
        /// <param name="data">생성할 파일에 저장할 데이터</param>
        /// <returns></returns>
        public static bool FileWriter(string fileName, string data)
        {
            string path = GetLogPath + "/" + GetTodayFolder;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileStream fs = new FileStream(path +"/"+fileName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);


            //StreamWriter sw = new StreamWriter(new FileStream(path+"/"+fileName, FileMode.Create));

            sw.Write(data);
            sw.Close();
            fs.Close();

            return true;
        }

    }
}