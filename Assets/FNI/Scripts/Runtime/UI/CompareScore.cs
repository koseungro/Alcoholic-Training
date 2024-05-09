/// 작성자: 고승로
/// 작성일: 2021-03-24
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 


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
    public class CompareScore : MonoBehaviour
    {
        [SerializeField] private Text[] score;
        [SerializeField] private Image[] graph;

        [SerializeField] private Transform preTr;
        [SerializeField] private Transform postTr;

        private LineRenderer line;

        private void OnEnable()
        {
            ShowPreScore(Main.curSceneID);
            ShowPostScore(Main.curSceneID);
            CreateLine();
        }
        private void OnDisable()
        {
            DBManager.Instance.ResetData();
            //Debug.Log("데이터를 초기화하였습니다.");
        }

        private void ShowPreScore(string SceneID)
        {
            string num = "";

            Score pre = DBManager.Instance.ScoreList.Find(x => x.ID.Contains($"{SceneID} PRE"));

            if (pre.ID != null)
            {
                num = pre.score.ToString();
                float height = GraphPosition(pre.score);
                preTr.localPosition = new Vector3(146, ScorePosition(pre.score), 0);

                graph[0].rectTransform.sizeDelta = new Vector2(35, height);
                graph[0].rectTransform.localPosition = new Vector3(33, height / 2 + 25.5f, 0);
            }
            else
                Debug.LogError("검사 점수를 찾을 수 없습니다.");

            score[0].text = num;
        }


        private void ShowPostScore(string SceneID)
        {
            string num = "";

            Score post = DBManager.Instance.ScoreList.Find(x => x.ID.Contains($"{SceneID} POST"));

            if (post.ID != null)
            {
                num = post.score.ToString();
                float height = GraphPosition(post.score);
                postTr.localPosition = new Vector3(384, ScorePosition(post.score), 0);

                graph[1].rectTransform.sizeDelta = new Vector2(35, height);
                graph[1].rectTransform.localPosition = new Vector3(33, height / 2 + 25.5f, 0);
            }
            else
                Debug.LogError("검사 점수를 찾을 수 없습니다.");

            score[1].text = num;
        }

        private float ScorePosition(float score)
        {
            float scoreSet = score - 10;
            float yPosition = scoreSet * 30 - 10;

            return yPosition;
        }

        private float GraphPosition(float score)
        {
            float height = score * 30;

            return height;
        }

        private void CreateLine()
        {
            Vector3 preVec = preTr.localPosition;
            Vector3 postVec = postTr.localPosition;
            Vector3 preLineVec = new Vector3(49, -30, -1);
            Vector3 postLineVec = new Vector3(6, -27, -1);

            if(line == null)
            {
                line = gameObject.AddComponent<LineRenderer>();
                line.useWorldSpace = false;
                line.receiveShadows = false;
                line.material = Resources.Load<Material>("Materials/LineMat");
                line.alignment = LineAlignment.TransformZ;
                line.positionCount = 2;
            }

            line.SetPosition(0, preVec + preLineVec);
            line.SetPosition(1, postVec + postLineVec);

            line.startWidth = 4f;
            line.endWidth = 4f;

            //Debug.Log("라인을 생성했습니다.");
        }
    }
}