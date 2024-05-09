/// 작성자: 백인성
/// 작성일: 2018-08-27
/// 수정일: 2018-08-27
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 페이드를 실행합니다. StartFade를 통해 컨트롤합니다.
/// Fade_Out은 Fade_In으로 UI_Out은 UI_In으로 컨트롤 해야 합니다.
/// </summary>
public class FNIVR_Fade : MonoBehaviour
{
    #region Singleton
    private static FNIVR_Fade _instance;
    public static FNIVR_Fade Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FNIVR_Fade>();
                if (_instance == null)
                    Debug.LogError("FNIVR_Fade를 찾을 수 없습니다. ");
            }
            return _instance;
        }
    }
    #endregion

    /// <summary>
    /// Fade나 UI의 기본 색상입니다. new Color(1, 1, 1, 0)
    /// </summary>
    public static Color White     { get { return new Color(1, 1, 1, 0);    } }
    /// <summary>
    /// Fade가 될 색상입니다. new Color(0, 0, 0, 1)
    /// </summary>
    public static Color Black     { get { return new Color(0, 0, 0, 1);    } }
	/// <summary>
	/// Fade가 될 색상입니다. new Color(0, 0, 0, 0)
	/// </summary>
	public static Color Clear { get { return new Color(0, 0, 0, 0); } }
	/// <summary>
	/// UI의 기본 색상입니다. new Color(0, 0, 0, 0.5f)
	/// </summary>
	public static Color HalfBlack { get { return new Color(0, 0, 0, 0.5f); } }

	/// <summary>
	/// 페이드의 부모입니다.
	/// </summary>
	private GameObject fadeParent;
    /// <summary>
    /// UI의 부모입니다.
    /// </summary>
    private GameObject uibgParent;
    /// <summary>
    /// 페이드의 이미지 입니다.
    /// </summary>
    private Image fadeImage;
    /// <summary>
    /// UI의 이미지 입니다.
    /// </summary>
    private Image uibgImage;
    /// <summary>
    /// 진행되고 있는 혹은 진행된 페이드의 상태
    /// </summary>
    private FadeFlag currentState = FadeFlag.NONE;
    /// <summary>
    /// 페이드 실행용
    /// </summary>
    private IEnumerator m_fade_Routine;
    /// <summary>
    /// 리버스 페이드 실행용
    /// </summary>
    private IEnumerator m_reversFade_Routine;

    #region Unity base method
    private void Awake ()
    {
        DontDestroyOnLoad(this);

        fadeParent = transform.Find("FNIVR_Canvas_Fade/Parent").gameObject;
        uibgParent = transform.Find("FNIVR_Canvas_UIBG/Parent").gameObject;

        fadeImage = fadeParent.transform.Find("OverBG").GetComponent<Image>();
        uibgImage = uibgParent.transform.Find("OverBG").GetComponent<Image>();
    }
#if UNITY_EDITOR
    //private void Update ()
    //{
    //    if (Input.GetKeyUp(KeyCode.A))
    //    {
    //        StartFade(FadeFlag.UI_Out, HalfBlack);
    //    }
    //    if (Input.GetKeyUp(KeyCode.S))
    //    {
    //        StartFade(FadeFlag.UI_In, White);
    //    }
    //    if (Input.GetKeyUp(KeyCode.D))
    //    {
    //        StartFade(FadeFlag.Fade_Out, Black);
    //    }
    //    if (Input.GetKeyUp(KeyCode.F))
    //    {
    //        StartFade(FadeFlag.Fade_In, White);
    //    }
    //}
#endif
    #endregion
    /// <summary>
    /// 페이드를 시작합니다.
    /// </summary>
    /// <param name="state">페이드를 실할할 조건</param>
    /// <param name="fadeColor">페이드 종료시 색상</param>
    /// <param name="fadeTime">페이드 시간</param>
    public void StartFade(FadeFlag state, Color fadeColor, float fadeTime = 1)
    {
        if (state != currentState && state != currentState)
        {
            if (m_fade_Routine != null)
                StopCoroutine(m_fade_Routine);
            if (m_reversFade_Routine != null)
                StopCoroutine(m_reversFade_Routine);

            bool curUI = StateCheck(currentState, FadeFlag.UI);
            bool isUI = StateCheck(state, FadeFlag.UI);

            if (currentState != FadeFlag.NONE &&
                curUI != isUI &&
                (currentState & FadeFlag.Out) == FadeFlag.Out)
            {
                //기존 페이드상태와 현재 새로 들어온 상태가 같지 않다면 기존 페이드와 반대되는 페이드를 실행한다.
                m_reversFade_Routine = ReversFade(fadeTime);
                StartCoroutine(m_reversFade_Routine);

                //새로 들어온 페이드는 잠시 기다렸다가 페이드를 실행한다.
                m_fade_Routine = Fade(state, fadeColor, fadeTime, fadeTime);
            }
            else//기존 페이드와 새로 들어온 페이드가 같다면 실행한다.
                m_fade_Routine = Fade(state, fadeColor, fadeTime);
            StartCoroutine(m_fade_Routine);
        }
    }
	private bool StateCheck(FadeFlag state1, FadeFlag state2)
	{
		return (state1 & state2) == state2;
	}
    /// <summary>
    /// 현재 조건과 반대 되는 상태를 반환한다.
    /// </summary>
    /// <param name="endTime">페이드 시간</param>
    /// <returns></returns>
    private IEnumerator ReversFade(float endTime)
    {
        switch (currentState)
        {
            case FadeFlag.Fade_In: return Fade(FadeFlag.Fade_Out, Black, endTime);
            case FadeFlag.Fade_Out: return Fade(FadeFlag.Fade_In, White, endTime);
            case FadeFlag.UI_In: return Fade(FadeFlag.UI_Out, HalfBlack, endTime);
            case FadeFlag.UI_Out: return Fade(FadeFlag.UI_In, White, endTime);
            default: return null;
        }
    }

    /// <summary>
    /// 페이드를 실행합니다. StartFade를 통해 컨트롤합니다.
    /// </summary>
    /// <param name="fadeFlag">페이드 상태</param>
    /// <param name="fadeColor">페이드 될 색상</param>
    /// <param name="endTime">페이드 시간</param>
    /// <returns></returns>
    private IEnumerator Fade(FadeFlag fadeFlag, Color fadeColor, float endTime, float lateStart = 0)
    {
        currentState = fadeFlag;

        yield return new WaitForSeconds(lateStart);

        bool isFadeIn = (fadeFlag & FadeFlag.In) == FadeFlag.In;
        bool isUI = (fadeFlag & FadeFlag.UI) == FadeFlag.UI;

        Image curImage;
        if (isUI)//curImage를 초기화 합니다.
        {
            curImage = uibgImage;

            uibgParent.SetActive(true);
            fadeParent.SetActive(false);
        }
        else
        {
            curImage = fadeImage;

            uibgParent.SetActive(false);
            fadeParent.SetActive(true);
        }
        
        //변수 초기화
        float curTime = 0;
        Color startColor = curImage.color;
        Color endColor = fadeColor;

        //curColor의 초기 값을 보정합니다.
        if (startColor == endColor)
        {
            startColor = isFadeIn ? (isUI ? HalfBlack : Black) : White;
        }

        //페이드를 실행합니다.
        while (curTime < endTime)
        {
            curTime += Time.deltaTime;

            curImage.color = Color.Lerp(startColor, endColor, curTime / endTime);
            yield return null;
        }

        //페이드 종료 후 값을 완전히 만들어주기 위해 색상을 다시 지정 해줍니다.
        curImage.color = endColor;

        //페이드 인일 때만 페이드 부모를 숨깁니다.
        if (isFadeIn)
        {
            if (isUI)
                uibgParent.SetActive(false);
            else
                fadeParent.SetActive(false);
        }
    }
}

/// <summary>
/// 페이드 상태 플레그 입니다.
/// </summary>
public enum FadeFlag
{
    /// <summary>
    /// 초기화 상태
    /// </summary>
    NONE = 0x0000,
    /// <summary>
    /// UI BG인가
    /// </summary>
    UI = 0x0001,
    /// <summary>
    /// 단순 페이드 인지
    /// </summary>
    Fade = 0x0002,
    /// <summary>
    /// 화면이 밝아집니다.
    /// </summary>
    In = 0x0004,
    /// <summary>
    /// 화면이 어두워집니다.
    /// </summary>
    Out = 0x0008,
    /// <summary>
    /// 페이드 아웃, 화면이 어두워짐
    /// </summary>
    Fade_Out = Fade | Out,
    /// <summary>
    /// 페이드 인, 화면이 밝아짐
    /// </summary>
    Fade_In = Fade | In,
    /// <summary>
    /// UI BG 아웃, UI배경이 어두워짐
    /// </summary>
    UI_Out = UI | Out,
    /// <summary>
    /// UI BG 인, UI배경이 밝아짐
    /// </summary>
    UI_In = UI | In,
}
