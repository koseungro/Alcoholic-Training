/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-08-28
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: VR장비 공용 트레킹 테스트용
/// 사용법: 실행시 FNIVR_GazePointerSurpport가 적용된 GameObject가 Hierarchy에 반드시 있어야 함
/// 
/// 수정이력 
/// 2018-08-28
///		손과 라인을 활성화 혹은 비활성화 하는데 필요한 옵션을 더 추가하였습니다.
/// 2019-04-03
///		SteamVR V2.0을 설치하고 그에 맞게 적용하였습니다.
///		불필요한 함수 및 변수를 삭제하여 최적화를 실시 했습니다.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// VR장비관련 상태체크 및 기본 세팅을 합니다.
/// </summary>
public class FNIVR_Device : MonoBehaviour
{
    #region Instance
    private static FNIVR_Device _instance;
    public static FNIVR_Device Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FNIVR_Device>();

                if (_instance == null)
                    Debug.LogError("FNIVR_Device를 찾을 수 없습니다. " +
                                   "FNIVR_Device를 GameObject(SteamVRObjects)에 적용하여 주세요");
            }
            return _instance;
        }
    }
    #endregion

    #region Statics
    /// <summary>
    /// 현재 어느 손을 사용하고 있는지 알려줍니다.
    /// </summary>
    public static HandState CurrentHandState;
    #endregion

    #region Property
    /// <summary>
    /// 기본 카메라입니다.
    /// </summary>
    public Camera VRCamera
    {
        get
        {
            if (m_headCam == null)
                m_headCam = GetCameras("VRCamera");
            return m_headCam;
        }
    }
    /// <summary>
    /// 기본 UI카메라입니다.
    /// </summary>
    public Camera UICamera
    {
        get
        {
            if (m_uiCam == null)
                m_uiCam = GetCameras("UICamera");
            return m_uiCam;
        }
    }
    /// <summary>
    /// 기본 UI카메라입니다.
    /// </summary>
    public Camera DisplayCamera
    {
        get
        {
            if (m_displayCam == null)
                m_displayCam = GetCameras("DisplayCamera");
            return m_displayCam;
        }
    }

	/// <summary>
	/// 왼손 사용 여부
	/// </summary>
	private bool LeftActive
	{
		get
		{
			return sbp_Left.poseAction.GetDeviceIsConnected(SteamVR_Input_Sources.LeftHand);
		}
	}
	/// <summary>
	/// 오른손 사용 여부
	/// </summary>
	private bool RightActive
	{
		get
		{
			return sbp_Right.poseAction.GetDeviceIsConnected(SteamVR_Input_Sources.RightHand);
		}
	}
	/// <summary>
	/// 현재 사용되고 있는 손의 레이 포인트를 반환합니다.
	/// </summary>
	public Transform CurrentRayPoint
	{
		get
		{
			switch (CurrentHandState)
			{
				case HandState.Left: return rayPoint_L;
				case HandState.Right: return rayPoint_R;
				//case HandState.Both: return rayPoint_R;
				default: return null;
			}
		}
	}
	#endregion

	#region public field
	/// <summary>
	/// 트레킹 시작 위치가 눈높이 인지 아니면 바닥인지 지정함
	/// </summary>
	[Header("Options")]
	public bool isEyeLevel;
	public bool initTransform = false;
	public HandState currentHandState;

	[Header("장치별 RayPoint 세팅")]
	public List<LocationByDevice> byDevices;
	#endregion

	#region protected / private field
	//Camera
	/// <summary>
	/// 기본 카메라입니다.
	/// </summary>
	private Camera m_headCam;
    /// <summary>
    /// 기본 UI카메라입니다.
    /// </summary>
    private Camera m_uiCam;
    /// <summary>
    /// 기본 모니터용 카메라입니다.
    /// </summary>
    private Camera m_displayCam;
	/// <summary>
	/// 모든 카메라 목록입니다.
	/// </summary>
	private Camera[] findCameras;
	//Hand
	/// <summary>
	/// 왼손의 기본 컨트롤러 입니다.
	/// </summary>
	private SteamVR_Behaviour_Pose sbp_Left;
	/// <summary>
	/// 오른손의 기본 컨트롤러 입니다.
	/// </summary>
	private SteamVR_Behaviour_Pose sbp_Right;
	private Transform rayPoint_L;
	private Transform rayPoint_R;
	#endregion

	#region Unity base method
	private void Awake()
	{
		findCameras = transform.GetComponentsInChildren<Camera>();

		SetCanvas();
		if (initTransform)
			InitTransform(transform);
    }

    private void Start()
    {
        SetTrackingSpace();
		sbp_Left = transform.Find("Controller (left)").GetComponent<SteamVR_Behaviour_Pose>();
		sbp_Right = transform.Find("Controller (right)").GetComponent<SteamVR_Behaviour_Pose>();
		rayPoint_L = sbp_Left.transform.Find("RayPoint");
		rayPoint_R = sbp_Right.transform.Find("RayPoint");
		SetLocationByDevice();
	}

    private void LateUpdate()
    {
        SetTrackingSpace();
		//GetHandState();
	}
	#endregion

	#region public Method
	public void ChangeActiveHand(HandState handState)
	{
		currentHandState = CurrentHandState = handState;
	}
	#endregion

	#region Private Method
	private void SetLocationByDevice()
	{
		Debug.Log("loadedDeviceName: " + UnityEngine.XR.XRDevice.model);

		LocationByDevice deviece = new LocationByDevice();
		for (int cnt = 0; cnt < byDevices.Count; cnt++)
		{
			if (UnityEngine.XR.XRDevice.model.Contains(byDevices[cnt].deviceContainName))
			{
				deviece = byDevices[cnt];
				break;
			}
		}

		if (deviece.deviceContainName != "")
		{
			rayPoint_R.localPosition = deviece.pos;
			rayPoint_R.localEulerAngles = deviece.rot;
			rayPoint_L.localPosition = new Vector3(deviece.pos.x * -1, deviece.pos.y, deviece.pos.z);
			rayPoint_L.localEulerAngles = deviece.rot;
		}
	}
    /// <summary>
    /// 하위에 있는 카메라를 찾아 각 변수에 넣어 줍니다.
    /// </summary>
    private Camera GetCameras(string name)
    {
        Camera findCamera = null;
		if (findCameras != null)
        //Debug.Log(findCameras.Length);
        for (int cnt = 0; cnt < findCameras.Length; cnt++)
        {
            Debug.Log(findCameras[cnt].name +" = "+ name + " [" + (findCameras[cnt].name == name) + "]");
            if (findCameras[cnt].name == name)
            {
                findCamera = findCameras[cnt];
                break;
            }
            else
                findCamera = null;
        }

        return findCamera;
    }
    /// <summary>
    /// 실행시 모든 켄버스의 이벤트 카메라를 설정해줍니다.
    /// </summary>
    private void SetCanvas()
    {
        Canvas[] canvas = FindObjectsOfType<Canvas>();
        for (int cnt = 0; cnt < canvas.Length; cnt++)
        {
            //켄버스의 이벤트 카메라를 설정합니다.
            if(canvas[cnt].name != "Canvas_HMD" && canvas[cnt].name != "Canvas_Display")
                canvas[cnt].worldCamera = UICamera;

            //FNIVR_GraphicRaycaster의 존재를 확인합니다.
            FNIVR_GraphicRaycaster graphicRaycaster = canvas[cnt].GetComponent<FNIVR_GraphicRaycaster>();
            if (graphicRaycaster != null)
                graphicRaycaster.pointer = FNIVR_GazePointer.instance.gameObject;
        }
    }
    /// <summary>
    /// VR의 트레킹 스페이스를 설정합니다.
    /// </summary>
    private void SetTrackingSpace()
	{
		if (isEyeLevel)
		{
			if (SteamVR.settings.trackingSpace != ETrackingUniverseOrigin.TrackingUniverseSeated)
			{
				SteamVR.settings.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;
			}
		}
		else
		{
			if (SteamVR.settings.trackingSpace != ETrackingUniverseOrigin.TrackingUniverseStanding)
			{
				SteamVR.settings.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
			}
		}
	}
	/// <summary>
	/// 현재 활성화 된 손을 찾는다.
	/// </summary>
	private void GetHandState()
	{
		     if (LeftActive  &&  RightActive) CurrentHandState = HandState.Both;
		else if (LeftActive  && !RightActive) CurrentHandState = HandState.Left;
		else if (!LeftActive &&  RightActive) CurrentHandState = HandState.Right;
		else								  CurrentHandState = HandState.None;
	}

    /// <summary>
    /// target의 위치와 각도를 초기화 해줍니다.
    /// </summary>
    /// <param name="target"></param>
    private void InitTransform(Transform target)
    {
        target.localPosition = Vector3.zero;
        target.localEulerAngles = Vector3.zero;
    }
    #endregion
}
/// <summary>
/// 컨트롤러의 활성화 상태를 기록하기 위해 사용합니다.
/// </summary>
public enum HandState
{
    /// <summary>
    /// 아무손도 사용하지 않는 상태 입니다.
    /// </summary>
    None,
    /// <summary>
    /// 왼손
    /// </summary>
    Left,
    /// <summary>
    /// 오른손
    /// </summary>
    Right,
    /// <summary>
    /// 양손
    /// </summary>
    Both
}
/// <summary>
/// 컨트롤러와 UI Line의 활성상태를 변경하는 곳에 적요할 것입니다.
/// </summary>
public enum HandActiveState
{
	/// <summary>
	/// 라인만 상태 변경을 합니다.
	/// </summary>
	Line,
	/// <summary>
	/// 손만 상태 변경 합니다.
	/// </summary>
	Hand,
	/// <summary>
	/// 라인과 손 모두 상태 변경 합니다.
	/// </summary>
	Both
}
[System.Serializable]
public struct LocationByDevice
{
	public string deviceContainName;
	public Vector3 pos;
	public Vector3 rot;
}