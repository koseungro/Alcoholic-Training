/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: FNIVR_GazePointerRing의 보조 역할
/// 사용법: Prefabs(FNIVR_GazePointerRing)를 이용해서 사용할 것
/// 수정이력 
/// 
/// 2018.09.13백인성
/// public bool alwayOnLine = false; 추가
/// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// FNIVR_GazePointer를 보조 합니다. FNIVR_GazePointer의 상태에 따라 라인렌더러를 숨기거나 보이게 한다.
/// </summary>
[RequireComponent(typeof(FNIVR_GazePointer))]
public class FNIVR_GazePointerSurpport : MonoBehaviour
{
    #region Instance
    private static FNIVR_GazePointerSurpport _instance;
    public static FNIVR_GazePointerSurpport Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FNIVR_GazePointerSurpport>();

                if (_instance == null)
                    Debug.LogError("FNIVR_GazePointerSurpport 찾을 수 없습니다. " +
                                   "FNIVR_GazePointerSurpport를 GameObject(FNIVR_GazePointerRing)에 적용하여 주세요");

            }
            return _instance;
        }
    }
    #endregion

    #region Property
    /// <summary>
    /// 레이캐스트의 파티클 효과를 정의하고 반환한다.
    /// </summary>
    private GameObject MyPointerGO
    {
        get
        {
            if (m_pointerGameObject == null)
                m_pointerGameObject = transform.Find("Pointer").gameObject;
            return m_pointerGameObject;
        }
    }
    /// <summary>
    /// FNIVR_GazePointer를 정의하고 반환한다.
    /// </summary>
    public FNIVR_GazePointer MyGazePointer
    {
        get
        {
            if (m_gazePointer == null)
                m_gazePointer = GetComponent<FNIVR_GazePointer>();
            return m_gazePointer;
        }
    }
    /// <summary>
    /// 레이캐스트의 정보를 반환한다.
    /// </summary>
    public Ray GetRay
    {
        get
        {
            return MyGazePointer.rayTransform ? new Ray(MyGazePointer.rayTransform.position, MyGazePointer.rayTransform.forward) : new Ray();

        }
    }
    /// <summary>
    /// 레이캐스트의 포커스 아이콘을 정의하고 반환한다.
    /// </summary>
    private MeshRenderer MyGazeRenderer
    {
        get
        {
            if (m_gazeRenderer == null)
                m_gazeRenderer = MyPointerGO.GetComponent<MeshRenderer>();
            return m_gazeRenderer;
        }
    }
    /// <summary>
    /// 레이캐스트의 라인을 활성화 여부를 설정한다.
    /// </summary>
    public bool ActiveLines
    {
        set
        {
            MyGazeRenderer.enabled = value;
        }
    }
	#endregion

	#region public field
	public HandState deactiveLine;
	/// <summary>
	/// 라인을 활성화 혹은 비활성화 합니다.
	/// </summary>
	public bool LineActive = true;

	//2018.09.13백인성 추가
	/// <summary>
	/// True일 때 LineActive가 True인 상태에서 게이즈 포인터가 숨겨져도 라인이 활성화 됩니다.
	/// </summary>
	public bool alwayOnLine = false;
	public bool useWorldSpace = false;
	[Range(0.0001f, 0.01f)]
	public float lineWidth = 0.0005f;
	#endregion

	#region protected / private field
	/// <summary>
	/// 선을 표시 하기 위한 라인 렌더러
	/// </summary>
	private LineRenderer m_lineRenderer;
	/// <summary>
	/// m_gazePointer.rayTransform을 지정하기 위해 사용합니다. MyGazePointer를 통해 사용되어야 합니다.
	/// </summary>
	private FNIVR_GazePointer m_gazePointer;
    /// <summary>
    /// 라인을 숨길 때 포인터를 같이 숨기기 위해서 사용합니다. MyGazeRenderer를 통해 사용되어야 합니다.
    /// </summary>
    private MeshRenderer m_gazeRenderer;
    /// <summary>
    /// 포인터를 가져오기 위해 사용합니다. MyPointerGO를 통해 사용되어야 합니다.
    /// </summary>
    private GameObject m_pointerGameObject;
    #endregion

    #region Unity base Method
    private IEnumerator Start()
	{
		while (FNIVR_Device.Instance.CurrentRayPoint == null)
			yield return null;
		m_lineRenderer = FNIVR_Device.Instance.CurrentRayPoint.GetComponent<LineRenderer>();

		MyGazePointer.rayTransform = FNIVR_Device.Instance.CurrentRayPoint;
	}
    private void LateUpdate()
    {
        SetLineActive();

    }
    #endregion

    #region Private Method
    /// <summary>
    /// 라인의 상태를 설정합니다.
    /// LineActive 변수의 영향을 받습니다.
    /// </summary>
    private void SetLineActive()
	{
        if (FNIVR_Device.Instance.CurrentRayPoint == null)
            return;

        if (MyGazePointer.rayTransform != FNIVR_Device.Instance.CurrentRayPoint)
        {
			//기존 라인 숨기기
			m_lineRenderer.enabled = false;
			//새 라인 불러오기
			m_lineRenderer = FNIVR_Device.Instance.CurrentRayPoint.GetComponent<LineRenderer>();
			if (deactiveLine != FNIVR_Device.CurrentHandState)
				m_lineRenderer.enabled = true;

			MyGazePointer.rayTransform = FNIVR_Device.Instance.CurrentRayPoint;
		}

		m_lineRenderer.useWorldSpace = useWorldSpace;

		ActiveLine();

		if (LineActive)
		{
			// 2018.09.13백인성
			// alwayOnLine가 True이면 게이즈 포인터만 사라지고 False이면 전부 사라집니다.
			if (alwayOnLine)
			{
				if (deactiveLine != FNIVR_Device.CurrentHandState)
					m_lineRenderer.enabled = true;
				m_lineRenderer.widthMultiplier = lineWidth;

				if (m_lineRenderer.useWorldSpace)
				{
					m_lineRenderer.SetPosition(0, MyGazePointer.rayTransform.position);
					if (MyGazePointer.hidden == false)
						m_lineRenderer.SetPosition(1, transform.position);
					else
						m_lineRenderer.SetPosition(1, MyGazePointer.rayTransform.position + MyGazePointer.rayTransform.forward.normalized);
				}
				else
				{
					m_lineRenderer.SetPosition(0, Vector3.zero);
					float dist = 0;
					if (MyGazePointer.hidden == false)
						dist = Vector3.Distance(FNIVR_Device.Instance.CurrentRayPoint.position, transform.position);
					else
						dist = 1;
					m_lineRenderer.SetPosition(1, Vector3.forward * dist);
				}
			}
			else
			{
				if (FNIVR_GazePointer.instance.hidden == false)
				{
					if (deactiveLine != FNIVR_Device.CurrentHandState)
						m_lineRenderer.enabled = true;
					m_lineRenderer.widthMultiplier = lineWidth;

					if (m_lineRenderer.useWorldSpace)
					{
						m_lineRenderer.SetPosition(0, FNIVR_Device.Instance.CurrentRayPoint.position);
						m_lineRenderer.SetPosition(1, transform.position);
					}
					else
					{
						float dist = Vector3.Distance(FNIVR_Device.Instance.CurrentRayPoint.position, transform.position);
						m_lineRenderer.SetPosition(0, Vector3.forward * dist);
						m_lineRenderer.SetPosition(1, Vector3.zero);
					}
				}
				else
				{
					if (deactiveLine != FNIVR_Device.CurrentHandState)
						m_lineRenderer.enabled = false;
				}
			}
		}
		else
		{
			if (deactiveLine != FNIVR_Device.CurrentHandState)
				m_lineRenderer.enabled = false;
		}
	}
    #endregion

    public void ActiveLine()
    {
        ActiveLines = FNIVR_Device.CurrentHandState != HandState.None ? LineActive : false;
    }
}