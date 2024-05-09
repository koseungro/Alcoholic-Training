/// 작성자: 고승로
/// 작성일: 2020-08-24
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

namespace FNI
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Scene Data", menuName = "FNI/Scene Data")]
    public class SceneData : ScriptableObject
    {
        public string sceneID;
        public List<CutData> cutDataList = new List<CutData>();
        public SceneData nextScene=null;
    }

    [System.Serializable]
    public class CutData
    {
        public VisualType visualType;
        public bool isFold = false;
        public bool VR3D_UIActive;
        public bool FNI_Record;

        public float waitTime;
        public NarrationOption narrationOption;
        public UIOption uiOption;
        public FadeOption fadeOption;
        public VideoOption videoOption;
        public FocusOption focusOption;
        public LayOption layOption;
        public MainEPOption epOption;
        public RuleOption scoreOption;
    }

    [System.Serializable]
    public class RuleOption
    {
        public EPType epType;
        public ScoreManagerFuncType funcType;
        public SceneData nextScene;
        public bool isTimeOut;
        public float finishTime;
        public int finishCount;
    }


    [System.Serializable]
    public class MainEPOption
    {
        public EPType epType;
        public SceneData succeedSequence;
        public SceneData failSequence;
        public EP04Option ep04;
        public EP05Option ep05;
        public EP06Option ep06;
    }

    [System.Serializable]
    public class EP04Option
    {
        public int gameIndex = 1;
        public Sprite[] frame = new Sprite[4];
        public Sprite[] buttons = new Sprite[4];
    }

    [System.Serializable]
    public class EP05Option
    {
        public E05ButtonType buttonType;
    }

    [System.Serializable]
    public class EP06Option
    {
        public E06ButtonType buttonType;
    }

    [System.Serializable]
    public class LayOption
    {
        public bool isOn;
    }

    [System.Serializable]
    public class FocusOption
    {
        public bool isOn;
    }


    [System.Serializable]
    public class UIOption
    {
        public UIType uiType = UIType.None;
        public string objPath;
        public string recordName;
        public bool isActive;
        public SceneData nextScene = null;
        public UIAnimationOption uiAnimationOption = null;
        public Vector3 Position;
        public Vector3 Rotation;
    }


    [System.Serializable]
    public class NarrationOption
    {
        public AudioClip clip;
        public AudioClip clipM;
        public bool isGender;
    }


    [System.Serializable]
    public class FadeOption
    {
        public UICanvasType uiCanvasType;
        public float startAlpha;
        public float endAlpha;
        public float time;
    }

    [System.Serializable]
    public class UIAnimationOption
    {
        public UIAnimationType aniType;
        public float time;
        public Vector3 startPos;
        public Vector3 endPos;
        public float startA;
        public float endA;
        public Sprite changeSprite;
        public string changeText;
    }


    [System.Serializable]
    /// <summary>
    /// 비디오 명령을 셋팅하고 집어 넣으면 됩니다.
    /// </summary>
    public class VideoOption
    {
        public VideoType videoType;
        public VideoState state;
        public SeekState seekState;
        public bool isGender;
        public string urlPath;
        public string urlWpath;
        public string urlMpath;
        public double sTime;
        public double eTime;
        public double jumpLenth;
        public Vector3 rotation;
        public Vector3 position;
    }

}