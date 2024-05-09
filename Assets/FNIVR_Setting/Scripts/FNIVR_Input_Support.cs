/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: 인풋 신호 감지
/// 사용법: Prefabs(FNIVR_EventSystem)를 이용해서 사용할 것
/// 수정이력 
/// FNIXR에 맞게 수정중

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// 인풋 신호를 컨트롤 합니다. FNIVR_InputModule에서 IsDown, IsUp를 사용합니다.
/// 외부에서는 onDown과 onUp을 사용하여 이벤트를 받습니다.
/// </summary>
[RequireComponent(typeof(UnityEngine.EventSystems.FNIVR_InputModule))]
public class FNIVR_Input_Support : MonoBehaviour
{
    #region Instance
    private static FNIVR_Input_Support _instance;
    public static FNIVR_Input_Support Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FNIVR_Input_Support>();

                if (_instance == null)
                    Debug.LogError("FNIVR_Input_Support 찾을 수 없습니다. " +
                                   "FNIVR_Input_Support를 GameObject(FNIVR_EventSystem)에 적용하여 주세요");

            }
            return _instance;
        }
    }
    #endregion

    #region Evnet
    public delegate void VoidType();
    public event VoidType onDown;
    public event VoidType onUp;
	public event VoidType onPress;
	#endregion

	#region Proterty
	/// <summary>
	/// OpenVR_Button 혹은 gazeClickKey의 입력이 Down인지 확인합니다.
	/// Down이 일어나고 1번 onDown를 호출 합니다.
	/// </summary>
	public bool IsDown
    {
        get
        {
			bool check = Input.GetKeyDown(gazeClickKey) || GetHandActionDown;
            if (check && onDown != null)
                onDown();
            return check;
        }
	}
	/// <summary>
	/// OpenVR_Button 혹은 gazeClickKey의 입력이 Down인지 확인합니다.
	/// Down이 일어나고 1번 onDown를 호출 합니다.
	/// </summary>
	public bool IsPress
	{
		get
		{
			bool check = Input.GetKey(gazeClickKey) || GetHandAction;
			if (check && onPress != null)
				onPress();
			return check;
		}
	}
	/// <summary>
	/// OpenVR_Button 혹은 gazeClickKey의 입력이 Up인지 확인합니다.
	/// Up이 일어나고 1번 onUp를 호출 합니다.
	/// </summary>
	public bool IsUp
    {
        get
        {
            bool check = Input.GetKeyUp(gazeClickKey) || GetHandActionUp;
            if (check && onUp != null)
                onUp();
            return check;
        }
    }
	/// <summary>
	/// 컨트롤러의 트리거를 당겼을 때 동작입니다.
	/// </summary>
	private bool GetHandActionDown
	{
		get
		{
			bool check_L = handAction.GetStateDown(SteamVR_Input_Sources.LeftHand);
			bool check_R = handAction.GetStateDown(SteamVR_Input_Sources.RightHand);
			bool isFirst = true;

			if (check_L)
			{
				isFirst = FNIVR_Device.CurrentHandState != HandState.Left;
				FNIVR_Device.Instance.ChangeActiveHand(HandState.Left);
				return true;
			}
			if (check_R)
			{
				isFirst = FNIVR_Device.CurrentHandState != HandState.Right;
				FNIVR_Device.Instance.ChangeActiveHand(HandState.Right);
				return true;
			}

			return check_L || check_R;
		}
	}
	/// <summary>
	/// 컨트롤러의 트리거를 누르고 있을 때 동작입니다.
	/// </summary>
	private bool GetHandAction
	{
		get
		{
			bool check_L = handAction.GetState(SteamVR_Input_Sources.LeftHand);
			bool check_R = handAction.GetState(SteamVR_Input_Sources.RightHand);
			bool isFirst = true;

			if (check_L)
			{
				isFirst = FNIVR_Device.CurrentHandState != HandState.Left;
				FNIVR_Device.Instance.ChangeActiveHand(HandState.Left);
				return true;
			}
			if (check_R)
			{
				isFirst = FNIVR_Device.CurrentHandState != HandState.Right;
				FNIVR_Device.Instance.ChangeActiveHand(HandState.Right);
				return true;
			}

			return check_L || check_R;
		}
	}
	/// <summary>
	/// 컨트롤러의 트리러를 때었을 때 동작입니다.
	/// </summary>
	private bool GetHandActionUp
	{
		get
		{
			bool check_L = handAction.GetStateUp(SteamVR_Input_Sources.LeftHand);
			bool check_R = handAction.GetStateUp(SteamVR_Input_Sources.RightHand);
			bool isFirst = true;

			if (check_L)
			{
				isFirst = FNIVR_Device.CurrentHandState != HandState.Left;
				FNIVR_Device.Instance.ChangeActiveHand(HandState.Left);
				return true;
			}
			if (check_R)
			{
				isFirst = FNIVR_Device.CurrentHandState != HandState.Right;
				FNIVR_Device.Instance.ChangeActiveHand(HandState.Right);
				return true;
			}

			return check_L || check_R;
		}
	}
	#endregion

	#region public field
	/// <summary>
	/// 컨트롤러 상호작용 버튼
	/// </summary>
	public SteamVR_Action_Boolean handAction;
	/// <summary>
	/// 키보드 상호작용 버튼
	/// </summary>
	public KeyCode gazeClickKey = KeyCode.Space;
	[Header("Vibration Option")]
	/// <summary>
	/// 컨트롤러 상호작용 버튼
	/// </summary>
	public SteamVR_Action_Vibration vibrationAction;
	public SteamVR_Input_Sources vibrationHand = SteamVR_Input_Sources.RightHand;
	public float repeatTime = 0.2f;
	public float delay = 0;
	/// <summary>
	/// 1회 진동의 길이
	/// </summary>
	[Range(0f, 1f)]
	public float duration = 0.1f;
	/// <summary>
	/// 진동빈도
	/// </summary>
	[Range(1f, 360f)]
	public float frequency = 75;
	/// <summary>
	/// 진동강도
	/// </summary>
	[Range(0f, 1f)]
	public float amplitude = 1;
	#endregion

	#region private field
	/// <summary>
	/// 진동 코루틴문을 사용하기 위한 변수 입니다.
	/// </summary>
	private IEnumerator m_vibrationLoop_Routine;
	private float curT = 0;
	#endregion

	#region Public Method
	/// <summary>
	/// 진동을 1회 줍니다. 베터리가 너무 없으면 진동이 가지 않음
	/// </summary>
	/// <param name="input_Sources">진동을 줄 곳</param>
	public void Vibration(SteamVR_Input_Sources input_Sources)
    {
		////////////////////////재생지연, 재생길이, 빈도(0~320), 강도(0~100f), 진동을 줄 곳
		vibrationAction.Execute(delay,    duration, frequency,   amplitude,    input_Sources);
	}
	/// <summary>
	/// 1회 신호로 지정한 횟수 만큼 진동을 줍니다. 베터리가 너무 없으면 진동이 가지 않음
	/// </summary>
	/// <param name="count">진동 횟수</param>
	/// <param name="input_Sources">진동을 줄 곳</param>
	public void Vibration(int count, SteamVR_Input_Sources input_Sources)
	{
		if (m_vibrationLoop_Routine != null)
			StopCoroutine(m_vibrationLoop_Routine);
		m_vibrationLoop_Routine = VibrationLoop_Routine(count, SteamVR_Input_Sources.RightHand);

		StartCoroutine(m_vibrationLoop_Routine);
	}
	/// <summary>
	/// 오른손에 진동을 줍니다. 연속 호출 용. 신호가 오는 동안 계속 진동을 줍니다. 베터리가 너무 없으면 진동이 가지 않음
	/// </summary>
	public void Vibration()
	{
		if (curT < repeatTime)
			curT += Time.deltaTime;
		else
		{
			curT = 0;
			Vibration(vibrationHand);
		}
	}
	#endregion

	#region Private Method
	/// <summary>
	/// 진동을 지정한 수 만큼 반복적으로 줍니다.
	/// </summary>
	/// <param name="count">진동 횟수</param>
	/// <param name="input_Sources">진동을 줄 곳</param>
	/// <returns></returns>
	private IEnumerator VibrationLoop_Routine(int count, SteamVR_Input_Sources input_Sources)
	{
		for (int cnt = 0; cnt < count; cnt++)
		{
			Vibration(input_Sources);
			yield return new WaitForSeconds(duration + 0.1f);
		}
	}
	#endregion
}