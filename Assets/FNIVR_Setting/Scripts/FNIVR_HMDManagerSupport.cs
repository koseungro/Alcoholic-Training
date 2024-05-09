/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력 
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class FNIVR_HMDManagerSupport : MonoBehaviour
{
	/// <summary>
	/// 자신의 켄버스를 가져옵니다.
	/// </summary>
	public Canvas MyCanvas
	{
		get
		{
			if (m_canvas == null) m_canvas = GetComponent<Canvas>();
			return m_canvas;
		}
	}
	/// <summary>
	/// 자신의 켄버스스케일러를 가져옵니다.
	/// </summary>
	public CanvasScaler MyCanvasScaler
	{
		get
		{
			if (m_canvasScaler == null) m_canvasScaler = MyCanvas.GetComponent<CanvasScaler>();
			return m_canvasScaler;
		}
	}
	/// <summary>
	/// imageState를 표시할 이미지입니다.
	/// </summary>
	public Image MyTextImage
	{
		get
		{
			if (m_textImage == null)
				m_textImage = MyPivot.transform.Find("Image_Text").GetComponent<Image>();
			return m_textImage;
		}
	}
	/// <summary>
	/// imageState를 표시할 이미지입니다.
	/// </summary>
	public Image MyCenterImage
	{
		get
		{
			if (m_centerImage == null)
				m_centerImage = MyPivot.transform.Find("Image_HMD").GetComponent<Image>();
			return m_centerImage;
		}
	}
	/// <summary>
	/// 실질적으로 숨겨지거나 표시 되는 부모입니다.
	/// </summary>
	public GameObject MyParent
	{
		get
		{
			if (m_parent == null)
				m_parent = transform.Find("Panel").gameObject;
			return m_parent;
		}
	}
	/// <summary>
	/// 실질적으로 움직이는 부모입니다.
	/// </summary>
	public RectTransform MyPivot
	{
		get
		{
			if (m_pivot == null)
				m_pivot = MyParent.transform.Find("Pivot").GetComponent<RectTransform>();
			return m_pivot;
		}
	}

	/// <summary>
	/// MyImage가 사용할 변수 값입니다.
	/// </summary>
	protected Image m_textImage;
	/// <summary>
	/// MyImage가 사용할 변수 값입니다.
	/// </summary>
	protected Image m_centerImage;
	/// <summary>
	/// MyCanvasDisplay가 사용할 변수 값입니다.
	/// </summary>
	protected Canvas m_canvas;
	/// <summary>
	/// MyCanvas가 사용할 변수 값입니다.
	/// </summary>
	protected CanvasScaler m_canvasScaler;
	/// <summary>
	/// MyParent가 사용할 변수 값입니다.
	/// </summary>
	protected GameObject m_parent;
	/// <summary>
	/// MyPivot이 사용할 변수 값입니다.
	/// </summary>
	protected RectTransform m_pivot;
}
