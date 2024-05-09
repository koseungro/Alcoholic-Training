/// 작성자: 고승로
/// 작성일: 2020-08-25
/// 수정일: 2020-09-03
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace FNI
{
    /// <summary>
    /// Visual Type 컷 데이터 관련 커스텀 에디터 클래스
    /// </summary>
    [CustomEditor(typeof(SceneData))]
    public class SZ_SceneEditor : Editor
    {
        SceneData _sceneData;

        private void OnEnable()
        {
            _sceneData = target as SceneData;
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.LabelField("컷 데이터");

            _sceneData.sceneID = EditorGUILayout.TextField("Scene ID", _sceneData.sceneID);
            EditorGUI.BeginChangeCheck();
            {

                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                {
                    _sceneData.cutDataList = ListController(_sceneData.cutDataList);

                    if (_sceneData.cutDataList.Count == 0)
                        EditorGUILayout.Space();

                    for (int cnt = 0; cnt < _sceneData.cutDataList.Count; cnt++)
                    {
                        if (_sceneData.cutDataList.Count <= cnt) return;

                        CutData cutData = _sceneData.cutDataList[cnt];

                        if (cutData == null) return;
                        if (cnt == 0)
                            EditorGUILayout.Space();

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUILayout.BeginHorizontal();
                            {

                                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(10));
                                {
                                    cutData.isFold = EditorGUILayout.Toggle(cutData.isFold, EditorStyles.foldout, GUILayout.MaxWidth(10));
                                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(2));
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(110));
                                {
                                    ListController(_sceneData.cutDataList, cnt, false);

                                    EditorGUILayout.LabelField($"[{cnt}] Cut", GUILayout.MaxWidth(50));

                                }
                                EditorGUILayout.EndHorizontal();

                                cutData.visualType = (VisualType)EditorGUILayout.EnumPopup(cutData.visualType);
                            }
                            EditorGUILayout.EndHorizontal();

                            if (_sceneData.cutDataList.Count <= cnt) return;
                            if (_sceneData.cutDataList[cnt].isFold)
                            {
                                switch (cutData.visualType)
                                {
                                    case VisualType.Line:
                                        GUI.color = Color.red;
                                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                                        EditorGUILayout.LabelField(" ");
                                        EditorGUILayout.EndHorizontal();
                                        GUI.color = Color.white;
                                        break;
                                    case VisualType.Narration:
                                        Narration(cutData);
                                        break;
                                    case VisualType.Wait:
                                        WaitTime(cutData);
                                        break;
                                    case VisualType.UI:
                                        UI(cutData);
                                        break;
                                    case VisualType.FadeInOut:
                                        FadeInOut(cutData);
                                        break;
                                    case VisualType.Data:
                                        Data(cutData);
                                        break;
                                    case VisualType.Video:
                                        Video(cutData);
                                        break;
                                    case VisualType.Lay:
                                        Lay(cutData);
                                        break;
                                    case VisualType.EPButtonManager:
                                        EPManager(cutData);
                                        break;
                                    case VisualType.ScoreManager:
                                        ScoreManager(cutData);
                                        break;
                                    case VisualType.Custom:
                                        VR3D_UI(cutData);
                                        break;
                                    case VisualType.Record:
                                        RecordActive(cutData);
                                        break;
                                    default:
                                        break;
                                }

                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.LabelField("자동 다음 씬");
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                {
                    _sceneData.nextScene = EditorGUILayout.ObjectField(_sceneData.nextScene, typeof(SceneData), false) as SceneData;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Wait 총 시간 : ", GUILayout.MaxWidth(80));
                    EditorGUILayout.LabelField(TotalWaitTime().ToString() + "초");
                }
                EditorGUILayout.EndHorizontal();

            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                EditorUtility.SetDirty(_sceneData);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        /// <summary>
        /// Visual Type 컷 데이터 관련 기능 구현 함수 목록
        /// </summary>
        #region Narration
        private void Narration(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("성별 차이 유무", GUILayout.MaxWidth(100));
                    cData.narrationOption.isGender = EditorGUILayout.Toggle(cData.narrationOption.isGender);
                }
                EditorGUILayout.EndHorizontal();

                if (cData.narrationOption.isGender)
                {
                    EditorGUILayout.LabelField("남자 : ", GUILayout.MaxWidth(70));
                    cData.narrationOption.clipM = EditorGUILayout.ObjectField(cData.narrationOption.clipM, typeof(AudioClip), false) as AudioClip;
                }

                float clipLength = 0;
                EditorGUILayout.BeginHorizontal();
                {
                    if (cData.narrationOption.isGender)
                        EditorGUILayout.LabelField("여자 : ", GUILayout.MaxWidth(70));
                    else
                        EditorGUILayout.LabelField("오디오", GUILayout.MaxWidth(70));

                    cData.narrationOption.clip = EditorGUILayout.ObjectField(cData.narrationOption.clip, typeof(AudioClip), false) as AudioClip;

                    if (cData.narrationOption.clip != null)
                    {
                        EditorGUILayout.LabelField(cData.narrationOption.clip.length.ToString() + "초", GUILayout.MaxWidth(80));
                        clipLength = cData.narrationOption.clip.length;
                    }
                }
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndVertical();

        }
        #endregion
        #region WaitTime
        private void WaitTime(CutData cData)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Wait", GUILayout.MaxWidth(50));
                cData.waitTime = EditorGUILayout.FloatField(cData.waitTime);
                if (cData.waitTime == 0)
                    cData.waitTime = 1;
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
        #region UI
        private void UI(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("해당 UI 위치", GUILayout.MaxWidth(100));
                    cData.uiOption.objPath = EditorGUILayout.TextField(cData.uiOption.objPath);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                        cData.uiOption.uiType = (UIType)EditorGUILayout.EnumPopup(cData.uiOption.uiType);
                    }
                    EditorGUILayout.EndHorizontal();

                    switch (cData.uiOption.uiType)
                    {
                        case UIType.None:
                            break;
                        case UIType.Button:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("클릭 시 이동 씬", GUILayout.MaxWidth(100));
                                cData.uiOption.nextScene = EditorGUILayout.ObjectField(cData.uiOption.nextScene, typeof(SceneData), false) as SceneData;
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case UIType.Active:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("isActive", GUILayout.MaxWidth(50));
                                cData.uiOption.isActive = EditorGUILayout.Toggle(cData.uiOption.isActive, GUILayout.MaxWidth(30));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case UIType.Animation:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("애니메이션 타입 : ", GUILayout.MaxWidth(100));
                                cData.uiOption.uiAnimationOption.aniType = (UIAnimationType)EditorGUILayout.EnumPopup(cData.uiOption.uiAnimationOption.aniType);
                            }
                            EditorGUILayout.EndHorizontal();

                            switch (cData.uiOption.uiAnimationOption.aniType)
                            {
                                case UIAnimationType.Move:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("Time : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.time = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.time);
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    cData.uiOption.uiAnimationOption.startPos = EditorGUILayout.Vector3Field("startPos", cData.uiOption.uiAnimationOption.startPos);
                                    cData.uiOption.uiAnimationOption.endPos = EditorGUILayout.Vector3Field("endPos", cData.uiOption.uiAnimationOption.endPos);

                                    break;
                                case UIAnimationType.ImageFadeInOut:
                                case UIAnimationType.TextFadeInOut:
                                case UIAnimationType.CanvasFadeInOut:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("Time : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.time = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.time);
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("start alpha : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.startA = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.startA);

                                        EditorGUILayout.LabelField("end alpha : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.endA = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.endA);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                case UIAnimationType.ChangeImage:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("교환이미지 : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.changeSprite = EditorGUILayout.ObjectField(cData.uiOption.uiAnimationOption.changeSprite, typeof(Sprite), false) as Sprite;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                case UIAnimationType.ChangeText:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("바꿀Text : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.changeText = EditorGUILayout.TextField(cData.uiOption.uiAnimationOption.changeText);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case UIType.Transform:
                            cData.uiOption.Position = EditorGUILayout.Vector3Field("Position", cData.uiOption.Position);
                            cData.uiOption.Rotation = EditorGUILayout.Vector3Field("Rotation", cData.uiOption.Rotation);
                            break;
                        case UIType.Record_Button:
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.LabelField("Record UI 위치", GUILayout.MaxWidth(100));
                                cData.uiOption.recordName = EditorGUILayout.TextField(cData.uiOption.recordName);

                                EditorGUILayout.LabelField("클릭 시 이동 씬", GUILayout.MaxWidth(100));                                
                                cData.uiOption.nextScene = EditorGUILayout.ObjectField(cData.uiOption.nextScene, typeof(SceneData), false) as SceneData;
                                
                            }
                            EditorGUILayout.EndVertical();
                            break;
                        default:
                            break;
                    }
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();
        }

        #endregion
        #region Fade
        private void FadeInOut(CutData cData)
        {
            cData.fadeOption.uiCanvasType = (UICanvasType)EditorGUILayout.EnumPopup(cData.fadeOption.uiCanvasType);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField(" start alpha 0~1 : ", GUILayout.MaxWidth(100));
                cData.fadeOption.startAlpha = EditorGUILayout.FloatField(cData.fadeOption.startAlpha, GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField(" end alpha : ", GUILayout.MaxWidth(70));
                cData.fadeOption.endAlpha = EditorGUILayout.FloatField(cData.fadeOption.endAlpha, GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField(" time : ", GUILayout.MaxWidth(50));
                cData.fadeOption.time = EditorGUILayout.FloatField(cData.fadeOption.time);
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
        #region Data
        private void Data(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("현재 씬", GUILayout.MaxWidth(50));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion
        #region Video
        private void Video(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("비디오 종류", GUILayout.MaxWidth(70));
                        cData.videoOption.videoType = (VideoType)EditorGUILayout.EnumPopup(cData.videoOption.videoType);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(70));
                        cData.videoOption.state = (VideoState)EditorGUILayout.EnumPopup(cData.videoOption.state);
                    }
                    EditorGUILayout.EndHorizontal();

                    switch (cData.videoOption.state)
                    {
                        case VideoState.Load:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("성별 여부", GUILayout.MaxWidth(100));
                                cData.videoOption.isGender = EditorGUILayout.Toggle(cData.videoOption.isGender);
                            }
                            EditorGUILayout.EndHorizontal();

                            if (cData.videoOption.isGender)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("남자 url 경로", GUILayout.MaxWidth(100));
                                    cData.videoOption.urlMpath = EditorGUILayout.TextField(cData.videoOption.urlMpath);
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("여자 url 경로", GUILayout.MaxWidth(100));
                                    cData.videoOption.urlWpath = EditorGUILayout.TextField(cData.videoOption.urlWpath);
                                }
                                EditorGUILayout.EndHorizontal();

                            }
                            else
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("url 경로", GUILayout.MaxWidth(100));
                                    cData.videoOption.urlPath = EditorGUILayout.TextField(cData.videoOption.urlPath);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            break;
                        case VideoState.Play:
                            EditorGUILayout.LabelField("영상을 재생합니다.");
                            break;
                        case VideoState.Pause:
                            EditorGUILayout.LabelField("영상을 일시정지합니다.");
                            break;
                        case VideoState.Stop:
                            EditorGUILayout.LabelField("영상을 정지합니다.");
                            break;
                        case VideoState.Seek:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("이동할 시간(s)", GUILayout.MaxWidth(100));
                                cData.videoOption.sTime = EditorGUILayout.DoubleField(cData.videoOption.sTime);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case VideoState.Repeat:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("반복시작 시간(s)", GUILayout.MaxWidth(100));
                                cData.videoOption.sTime = EditorGUILayout.DoubleField(cData.videoOption.sTime);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("반복종료 시간(s)", GUILayout.MaxWidth(100));
                                cData.videoOption.eTime = EditorGUILayout.DoubleField(cData.videoOption.eTime);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case VideoState.UnRepeat:
                            EditorGUILayout.LabelField("구간반복을 종료합니다.");
                            break;
                        case VideoState.SeekAndRepeat:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Seek 시작 시간", GUILayout.MaxWidth(100));
                                cData.videoOption.sTime = EditorGUILayout.DoubleField(cData.videoOption.sTime);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("반복 종료 시간", GUILayout.MaxWidth(100));
                                cData.videoOption.eTime = EditorGUILayout.DoubleField(cData.videoOption.eTime);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case VideoState.Jump:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("점프 값", GUILayout.MaxWidth(100));
                                cData.videoOption.jumpLenth = EditorGUILayout.DoubleField(cData.videoOption.jumpLenth);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case VideoState.Loop:
                            EditorGUILayout.LabelField("영상을 반복합니다.");
                            break;
                        case VideoState.UnLoop:
                            EditorGUILayout.LabelField("영상반복을 종료합니다.");
                            break;
                        case VideoState.Rotation:
                            EditorGUILayout.BeginHorizontal();
                            {
                                cData.videoOption.rotation = EditorGUILayout.Vector3Field("VR3D rotation", cData.videoOption.rotation);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case VideoState.Position:
                            EditorGUILayout.BeginHorizontal();
                            {
                                cData.videoOption.position = EditorGUILayout.Vector3Field("VR3D position", cData.videoOption.position);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        default:
                            break;
                    }
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();
        }
        #endregion
        #region Lay
        private void Lay(CutData cData)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("컨트롤러 Lay (체크가 ON) : ", GUILayout.MaxWidth(180));
                cData.layOption.isOn = EditorGUILayout.Toggle(cData.layOption.isOn);
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
        #region VR3D
        private void VR3D_UI(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("VR3D_UI Active상태", GUILayout.MaxWidth(130));
                    cData.VR3D_UIActive = EditorGUILayout.Toggle(cData.VR3D_UIActive, GUILayout.MaxWidth(30));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

        }
        #endregion
        #region etc

        private void RecordActive(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("isActive", GUILayout.MaxWidth(50));
                    cData.FNI_Record = EditorGUILayout.Toggle(cData.FNI_Record, GUILayout.MaxWidth(30));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }


        private void ScoreManager(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("에피소드 타입", GUILayout.MaxWidth(100));
                    cData.scoreOption.epType = (EPType)EditorGUILayout.EnumPopup(cData.scoreOption.epType);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("사용할 기능", GUILayout.MaxWidth(100));
                    cData.scoreOption.funcType = (ScoreManagerFuncType)EditorGUILayout.EnumPopup(cData.scoreOption.funcType);
                }
                EditorGUILayout.EndHorizontal();

                switch (cData.scoreOption.funcType)
                {
                    case ScoreManagerFuncType.SetInfoInit:
                        EditorGUILayout.LabelField("ScoreData 초기화", GUILayout.MaxWidth(100));
                        break;
                    case ScoreManagerFuncType.SetGameOver:
                        EditorGUILayout.LabelField("< 게임 오버 셋팅 >", GUILayout.MaxWidth(100));
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("시간이면 체크", GUILayout.MaxWidth(100));
                            cData.scoreOption.isTimeOut = EditorGUILayout.Toggle(cData.scoreOption.isTimeOut, GUILayout.MaxWidth(15));
                        }
                        EditorGUILayout.EndHorizontal();

                        if (cData.scoreOption.isTimeOut)
                        {
                            EditorGUILayout.LabelField("지금 시점부터 카운트", GUILayout.MaxWidth(200));

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("제한시간 :", GUILayout.MaxWidth(100));
                                cData.scoreOption.finishTime = EditorGUILayout.FloatField(cData.scoreOption.finishTime);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("반복횟수 :", GUILayout.MaxWidth(100));
                                cData.scoreOption.finishCount = EditorGUILayout.IntField(cData.scoreOption.finishCount);
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("실패후 다음 컷 : ", GUILayout.MaxWidth(100));
                            cData.scoreOption.nextScene = EditorGUILayout.ObjectField(cData.scoreOption.nextScene, typeof(SceneData), false) as SceneData;
                        }
                        EditorGUILayout.EndHorizontal();

                        break;
                    case ScoreManagerFuncType.WriteScore:
                        EditorGUILayout.LabelField("WriteScore 점수 작성", GUILayout.MaxWidth(200));
                        break;
                    case ScoreManagerFuncType.WriteCsv:
                        EditorGUILayout.LabelField("Csv 파일 작성", GUILayout.MaxWidth(200));
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        [Obsolete]
        private void EPManager(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("에피소드 타입", GUILayout.MaxWidth(50));
                    cData.epOption.epType = (EPType)EditorGUILayout.EnumPopup(cData.epOption.epType);
                }
                EditorGUILayout.EndHorizontal();

                switch (cData.epOption.epType)
                {
                    case EPType.E01:
                    case EPType.E02:
                    case EPType.E03:
                        EPManagerDefault(cData);
                        break;
                    case EPType.E04:
                        EPManagerDefault(cData);

                        EditorGUILayout.LabelField("게임인덱스(1 또는 2)", GUILayout.MaxWidth(100));
                        EditorGUILayout.BeginHorizontal();
                        {
                            cData.epOption.ep04.gameIndex = EditorGUILayout.IntField(cData.epOption.ep04.gameIndex);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.LabelField("프레임 이미지", GUILayout.MaxWidth(100));
                        EditorGUILayout.BeginHorizontal();
                        {
                            for (int i = 0; i < 4; i++)
                                cData.epOption.ep04.frame[i] = EditorGUILayout.ObjectField(cData.epOption.ep04.frame[i], typeof(Sprite), GUILayout.MaxWidth(100)) as Sprite;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField("버튼 이미지", GUILayout.MaxWidth(100));
                        EditorGUILayout.BeginHorizontal();
                        {
                            for (int i = 0; i < 4; i++)
                                cData.epOption.ep04.buttons[i] = EditorGUILayout.ObjectField(cData.epOption.ep04.buttons[i], typeof(Sprite), GUILayout.MaxWidth(100)) as Sprite;
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case EPType.E05:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("버튼타입", GUILayout.MaxWidth(50));
                            cData.epOption.ep05.buttonType = (E05ButtonType)EditorGUILayout.EnumPopup(cData.epOption.ep05.buttonType);
                        }
                        EditorGUILayout.EndHorizontal();
                        EPManagerDefault(cData);
                        break;
                    case EPType.E06:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("버튼타입", GUILayout.MaxWidth(50));
                            cData.epOption.ep06.buttonType = (E06ButtonType)EditorGUILayout.EnumPopup(cData.epOption.ep06.buttonType);
                        }
                        EditorGUILayout.EndHorizontal();
                        EPManagerDefault(cData);
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        private void EPManagerDefault(CutData cData)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("성공 컷", GUILayout.MaxWidth(50));
                cData.epOption.succeedSequence = EditorGUILayout.ObjectField(cData.epOption.succeedSequence, typeof(SceneData), false) as SceneData;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("실패 컷", GUILayout.MaxWidth(50));
                cData.epOption.failSequence = EditorGUILayout.ObjectField(cData.epOption.failSequence, typeof(SceneData), false) as SceneData;
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion


        #region Other
        private List<T> ListController<T>(List<T> list, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(120));
            {
                EditorGUILayout.LabelField($"Total[{list.Count}]", GUILayout.MaxWidth(60));


                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    if (list.Count != 0)
                    {
                        if (EditorUtility.DisplayDialog("경고", "초기화 하겠습니까?\n복구 불가능", "네", "아니오"))
                            list = new List<T>();
                    }
                    else
                        EditorUtility.DisplayDialog("경고", "초기화 할 데이터가 없습니다.", "닫기");
                }
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    list.Add(default);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "갯수를 줄이겠습니까?\n줄이면 값이 사라집니다.", "네", "아니오"))
                    {
                        if (list.Count != 0)
                            list.RemoveAt(list.Count - 1);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
        private List<T> ListController<T>(List<T> list, int num, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();
            if (list.Count == 0)
                list.Add(default);

            GUI.color = Color.yellow;
            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(60));
            {
                if (GUILayout.Button("D", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "삭제 하겠습니까??\n복구가 불가능합니다.", "네", "아니오"))
                        list.RemoveAt(num);
                }

                if (GUILayout.Button("C", GUILayout.Width(20)))
                {
                    list.Insert(num + 1, default);
                }

                EditorGUI.BeginDisabledGroup(!(0 < num));
                {
                    if (GUILayout.Button("△", GUILayout.Width(20)))
                    {
                        list.Insert(num - 1, list[num]);
                        list.RemoveAt(num + 1);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!(num < list.Count - 1));
                {
                    if (GUILayout.Button("▽", GUILayout.Width(20)))
                    {
                        list.Insert(num + 2, list[num]);
                        list.RemoveAt(num);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;

            return list;
        }

        private float TotalWaitTime()
        {
            float TotalWaitTime = 0;

            for (int cnt = 0; cnt < _sceneData.cutDataList.Count; cnt++)
            {
                if (_sceneData.cutDataList.Count <= cnt) return TotalWaitTime;

                CutData cutData = _sceneData.cutDataList[cnt];

                if (cutData.visualType == VisualType.Wait)
                {
                    TotalWaitTime += cutData.waitTime;

                }
            }

            return TotalWaitTime;
        }


        #endregion

    }
}