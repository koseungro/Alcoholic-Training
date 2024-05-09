/// 작성자: 백인성 
/// 작성일: 2018-08-10 
/// 수정일: 2018-08-27
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: 해드센서 감지
/// 수정이력 
/// 
/// 2018-08-27 백인성
/// 상속가능 하도록 수정
/// 인스턴스를 변경
/// 
/// 2018-09-06 백인성
/// Start()
/// {
///		m_First = false;추가
/// }
/// 처음시작 때 착용하고 있으면 정상 작동하지 않는 문제 수정
/// 
/// 2018-09-13 백인성
/// SetCanvasCamera(Camera cam)추가
///		켄버스의 카메라를 설정해주는 함수
/// SetImages(HMDStateImage state)
///		hmd착용 및 해제 했을 때 안내 문구
/// struct HMDStateImage 추가
///		SetImages를 위한 이미지 묶음 스트럭트

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using UnityEngine.Events;
using Valve.VR;

/// <summary>
/// HMD를 착용했는지 벗었는지 체크하는 클래스입니다.
/// 착용상태에 따라 hmdPauseAction, hmdPlayAction에서 신호를 줍니다.
/// </summary>
public class FNIVR_HMDManager : MonoBehaviour
{
    private static FNIVR_HMDManager _instance;
    /// <summary>
    /// FNIVR_GazePointerRing 인스턴스를 만든다.
    /// </summary>
    public static FNIVR_HMDManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FNIVR_HMDManager>();
            }
            return _instance;
        }
    }
    /// <summary>
    /// HMD를 벗었을 때 호출 됩니다.
    /// </summary>
    public static UnityAction hmdPauseAction;
    /// <summary>
    /// HMD를 착용했을 때 호출 됩니다.
    /// </summary>
    public static UnityAction hmdPlayAction;

	public SteamVR_Action_Boolean hmdAction;

	/// <summary>
	/// 프로그램 종료 신호를 줍니다.
	/// </summary>
	public bool IsEnd { set { m_Finished = value; } }
	/// <summary>
	/// 헤드셋 디바이스를 가져옵니다. 모든 VR은 0번이 HMD입니다.
	/// </summary>
	protected bool Headset { get { return hmdAction.GetState(SteamVR_Input_Sources.Head); } }
	/// <summary>
	/// 정지시 시간의 흐름입니다.
	/// </summary>
	public float PAUSE_TIMESCALE = 0.0001f;
	/// <summary>
	/// 해드셋을 착용상태 입니다.
	/// </summary>
	protected bool m_headSetOn = false;
	/// <summary>
	/// 해드셋 착용상태 변경시 1번 호출하기 위해서 입니다.
	/// </summary>
	protected bool m_onceCall_headSetOn = false;
	/// <summary>
	/// 프로그램 종료시
	/// </summary>
	protected bool m_Finished = false;
	/// <summary>
	/// 최초 실행시 초기화를 1번만 하기 위해서 사용된다.
	/// </summary>
	protected bool m_First = true;
	//[2018.09.13_백인성]수정 - imageState를 hmdStates로 변경
	/// <summary>
	/// HMD착용상태에 따른 이미지입니다.[0]InitHMD [1]PlayHMD [2]HMDExit 입니다.
	/// </summary>
	[Tooltip("[0]초기 이미지, [1]플레이 중 이미지, [2]종료시 이미지")]
	public HMDStateImage[] hmdStates;

	/// <summary>
	/// 디스플레이에 표시될 서포터입니다.
	/// </summary>
	private FNIVR_HMDManagerSupport supportDisplay;
	/// <summary>
	/// HMD에 표시 될 서포터 입니다.
	/// </summary>
	private FNIVR_HMDManagerSupport supportHMD;

	/// <summary>
	/// 센서의 인풋 신호입니다.
	/// </summary>
	protected EVRButtonId sensor = EVRButtonId.k_EButton_ProximitySensor;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        m_onceCall_headSetOn = false;
	}
	
    protected void Start()
    {
		supportDisplay = transform.Find("Canvas_Display").GetComponent<FNIVR_HMDManagerSupport>();
		supportHMD = transform.Find("Canvas_HMD").GetComponent<FNIVR_HMDManagerSupport>();

		m_Finished = false;
		m_headSetOn = Headset;// headset.GetPress(sensor);
		if (m_headSetOn)
		{
			HandleHMDMounted();
			//[2018.09.06_백인성]수정 -  처음시작 때 착용하고 있으면 정상 작동하지 않는 문제 수정
			m_First = false;
		}
		else
			HandleHMDUnmounted();
    }
    /// <summary>
    /// 다른 레벨이 로드되면 canvas의 타겟을 잃게 되므로 다시 한번 찾아준다.
    /// </summary>
    /// <param name="level"></param>
    protected void OnLevelWasLoaded(int level)
    {
        if (supportHMD.MyCanvas.worldCamera == null)
        {
			SetCanvasCamera();
		}
    }
    
    protected void Update()
    {
		m_headSetOn = Headset;//headset.GetPress(sensor);
		if (m_headSetOn)
        {
            if (m_onceCall_headSetOn == false)
                HandleHMDMounted();
        }
        else
        {
            if (m_onceCall_headSetOn == true)
                HandleHMDUnmounted();
        }
    }
        
    /// <summary>
    /// HMD를 착용했을 때 호출 합니다. 1번만 호출되어야 합니다.
    /// </summary>
    protected virtual void HandleHMDMounted()
    {
        m_onceCall_headSetOn = true;
        //FNIVR_Device.Instance.SetHandActive(FNIVR_Device.HandActiveState.Both, true);
		
		//OpenVR.System.ResetSeatedZeroPose();

		supportDisplay.MyParent.SetActive(false);
		supportHMD.MyParent.SetActive(false);

		Pause(false);

        if (hmdPlayAction != null)
            hmdPlayAction.Invoke();     //동작();
    }

    /// <summary>
    /// HMD를 벗었을 때 호출 합니다. 1번만 호출되어야 합니다.
    /// </summary>
    protected virtual void HandleHMDUnmounted()
    {
        m_onceCall_headSetOn = false;
        //FNIVR_Device.Instance.SetHandActive(FNIVR_Device.HandActiveState.Both, false);

        Pause(true);

        if (m_Finished == false)
        {
            if (m_First)
            {
                InitHMD();
                m_First = false;
            }
            else
                PlayHMD();

            if (hmdPauseAction != null)
                hmdPauseAction.Invoke();

            //Debug.Log("HandleHMDUnmounted");
        }
        else
        {
            Debug.Log("ㅡㅡㅡ 끝");
            Application.Quit();
        }
    }
	/// <summary>
	/// 콘텐츠 시작에 연결
	/// </summary>
	public virtual void InitHMD()
	{
		SetImages(hmdStates[0]);

		SetCanvasCamera();
	}
	/// <summary>
	/// 콘텐츠 시작에 연결
	/// </summary>
	public virtual void PlayHMD()
	{
		SetImages(hmdStates[1]);

		SetCanvasCamera();
	}
	/// <summary>
	/// 콘텐츠 끝에 연결, 외부에서 호출해야 함.
	/// </summary>
	public virtual void HMDExit()
	{
		SetImages(hmdStates[2]);

		SetCanvasCamera();
		
		m_Finished = true;
	}
	/// <summary>
	/// 일시정지합니다.
	/// </summary>
	/// <param name="paused"></param>
	public virtual void Pause(bool paused)
    {
        if (paused)
        {
            Time.timeScale = PAUSE_TIMESCALE;
        }
        else if (!paused)
        {
            Time.timeScale = 1;
        }
    }
	/// <summary>
	/// 카메라를 설정해줍니다.
	/// </summary>
	/// <param name="cam"></param>
	private void SetCanvasCamera()
	{
		supportDisplay.MyCanvas.worldCamera = FNIVR_Device.Instance.DisplayCamera;
		supportDisplay.MyCanvas.sortingLayerName = "HMD Mount";
		supportDisplay.MyPivot.anchoredPosition = Vector2.zero;
		supportDisplay.MyParent.SetActive(true);
		
		supportHMD.MyCanvas.worldCamera = FNIVR_Device.Instance.UICamera;
		supportHMD.MyCanvas.sortingLayerName = "Fade";
		supportHMD.MyPivot.anchoredPosition = Vector2.zero;
		supportHMD.MyParent.SetActive(true);
	}
	/// <summary>
	/// 이미지를 설정합니다.
	/// </summary>
	/// <param name="state"></param>
	private void SetImages(HMDStateImage state)
	{
		supportDisplay.MyTextImage.sprite = state.text;
		supportDisplay.MyCenterImage.sprite = state.center;
		supportDisplay.MyTextImage.SetNativeSize();
		supportDisplay.MyCenterImage.SetNativeSize();

		supportHMD.MyTextImage.sprite = state.text;
		supportHMD.MyCenterImage.sprite = state.center;
		supportHMD.MyTextImage.SetNativeSize();
		supportHMD.MyCenterImage.SetNativeSize();
	}
}

[System.Serializable]
public struct HMDStateImage
{
	public Sprite text;
	public Sprite center;
}