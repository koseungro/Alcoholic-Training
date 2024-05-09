/// 작성자: 고승로
/// 작성일: 2020-08-24
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    /// <summary>
    /// 컷 종류 타입
    /// </summary>
    public enum VisualType
    {
        Line,
        Narration,
        Wait,
        FadeInOut,
        UI,
        Backgroung,
        Custom,
        Record,
        Data,
        Video,
        Lay,
        EPButtonManager,
        ScoreManager,
        Quit
    }

    /// <summary>
    /// 시퀀스 상태
    /// </summary>
    public enum SequenceState
    {
        Setting,
        Start,
        Update,
        Check,
        End
    }

    /// <summary>
    /// VisualType.UI -> UI 타입
    /// </summary>
    public enum UIType
    {
        None,
        Button,
        Active,
        AllInactive,
        Animation,
        Transform,
        Record_Button
    }

    public enum ScoreManagerFuncType
    {
        SetInfoInit,
        SetGameOver,
        WriteScore,
        WriteCsv,
        Quit
    }

    /// <summary>
    /// UI 애니메이션 타입
    /// </summary>
    public enum UIAnimationType
    {
        Move,
        TextFadeInOut,
        ImageFadeInOut,
        CanvasFadeInOut,
        ChangeImage,
        ChangeText
    }

    /// <summary>
    /// Audio 설정 타입
    /// </summary>
    public enum AudioSettingType
    {
        Play,
        Stop,
        Volume,
        AllStop
    }



    public enum VideoState
    {
        Load,
        Play,
        Pause,
        Stop,
        Seek,
        Repeat,
        UnRepeat,
        Jump,
        Loop,
        UnLoop,
        Rotation,
        Position,
        SeekAndRepeat
    }

    public enum VideoType
    {
        Main
    }

    public enum SeekState
    {
        Down,
        Drag,
        Up,
        Order,
        Complete
    }

    public enum LoadState
    {
        Fail,
        Succeed,
        Loading
    }

    public enum UICanvasType
    {
        NomalBackground,
        UICanvas,
        FadeInOutCanvas
    }

    public enum GenderType
    {
        Man,
        Woman
    }

    public enum EPType
    {
        E01,
        E02,
        E03,
        E04,
        E05,
        E06
    }

    public enum E05ButtonType
    {
        Place,
        Left,
        Right
    }

    public enum E06ButtonType
    {
        Place,
        APT,
        Flowerbed,
        Playground
    }
}