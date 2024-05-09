/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: 게이즈 아이콘 표시용도, Oculus가 기초임
/// 사용법: Prefabs(FNIVR_GazePointerRing)를 이용해서 사용할 것
/// 수정이력 
/// 

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI pointer driven by gaze input.
/// </summary>
public class FNIVR_GazePointer : MonoBehaviour
{
    private static FNIVR_GazePointer _instance;
    /// <summary>
    /// FNIVR_GazePointerRing 인스턴스를 만든다.
    /// </summary>
    public static FNIVR_GazePointer instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FNIVR_GazePointer>();
            }
            return _instance;
        }
    }

	public bool showPoint = false;
	
	[Tooltip("Should the pointer be hidden when not over interactive objects.")]
    public bool hideByDefault = true;

    [Tooltip("Time after leaving interactive object before pointer fades.")]
    public float showTimeoutPeriod = 1;

    [Tooltip("Time after mouse pointer becoming inactive before pointer unfades.")]
    public float hideTimeoutPeriod = 0.1f;

    [Tooltip("Keep a faint version of the pointer visible while using a mouse")]
    public bool dimOnHideRequest = true;

    [Tooltip("Angular scale of pointer")]
    public float depthScaleMultiplier = 0.03f;

    /// <summary>
    /// The gaze ray.
    /// </summary>
    [Header("외부에서 넣어 줌")]
    public Transform rayTransform;

    /// <summary>
    /// Is gaze pointer current visible
    /// </summary>
    public bool hidden { get; private set; }

    /// <summary>
    /// Current scale applied to pointer
    /// </summary>
    public float currentScale { get; private set; }

    /// <summary>
    /// Current depth of pointer from camera
    /// </summary>
    private float depth;
    private float hideUntilTime;
    /// <summary>
    /// How many times position has been set this frame. Used to detect when there are no position sets in a frame.
    /// </summary>
    private int positionSetsThisFrame = 0;
    /// <summary>
    /// Last time code requested the pointer be shown. Usually when pointer passes over interactive elements.
    /// </summary>
    private float lastShowRequestTime;
    /// <summary>
    /// Last time pointer was requested to be hidden. Usually mouse pointer activity.
    /// </summary>
    private float lastHideRequestTime;

    [Tooltip("Radius of the cursor. Used for preventing geometry intersections.")]
    public float cursorRadius = 1f;


    /// <summary>
    /// 레이캐스트의 포커스의 알파값을 선언하거나 사라질때의 크기 등을 정의한다.
    /// </summary>
    public float visibilityStrength
    {
        get
        {
            // It's possible there are reasons to show the cursor - such as it hovering over some UI - and reasons to hide 
            // the cursor - such as another input method (e.g. mouse) being used. We take both of these in to account.


            float strengthFromShowRequest;
            if (hideByDefault)
            {
                // fade the cursor out with time
                strengthFromShowRequest = Mathf.Clamp01(1 - (Time.time - lastShowRequestTime) / showTimeoutPeriod);
            }
            else
            {
                // keep it fully visible
                strengthFromShowRequest = 1;
            }

            // Now consider factors requesting pointer to be hidden
            float strengthFromHideRequest;

            strengthFromHideRequest = (lastHideRequestTime + hideTimeoutPeriod > Time.time) ? (dimOnHideRequest ? 0.1f : 0) : 1;


            // Hide requests take priority
            return Mathf.Min(strengthFromShowRequest, strengthFromHideRequest);
        }
    }

    public void Awake()
    {
        currentScale = 1;
        // Only allow one instance at runtime.
        if (_instance != null && _instance != this)
        {
            enabled = false;
            DestroyImmediate(this);
            return;
        }

        _instance = this;
    }

    void Update()
    {
        if (rayTransform == null && Camera.main != null)
            rayTransform = Camera.main.transform;

        // Move the gaze cursor to keep it in the middle of the view
        if (rayTransform != null)
            transform.position = rayTransform.position + rayTransform.forward * depth;

        // Should we show or hide the gaze cursor?
        if (visibilityStrength == 0 && !hidden)
        {
            Hide();
        }
        else if (visibilityStrength > 0 && hidden)
        {
            Show();
        }
    }

    /// <summary>
    /// 포인터의 위치와 방향 설정
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="normal"></param>
    public void SetPosition(Vector3 pos, Vector3 normal)
    {
        transform.position = pos;

        // Set the rotation to match the normal of the surface it's on.
        Quaternion newRot = transform.rotation;
        newRot.SetLookRotation(normal, rayTransform.up);
        transform.rotation = newRot;

        // record depth so that distance doesn't pop when pointer leaves an object
        depth = (rayTransform.position - pos).magnitude;

        //set scale based on depth
        currentScale = depth * depthScaleMultiplier;
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        positionSetsThisFrame++;
    }

    /// <summary>
    /// 카메라용 포인터 위치와 방향 설정
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(Vector3 pos)
    {
        SetPosition(pos, rayTransform.forward);
    }

    public float GetCurrentRadius()
    {
        return cursorRadius * currentScale;
    }

    void LateUpdate()
    {
        // This happens after all Updates so we know that if positionSetsThisFrame is zero then nothing set the position this frame
        if (positionSetsThisFrame == 0)
        {
            // No geometry intersections, so gazing into space. Make the cursor face directly at the camera
            Quaternion newRot = transform.rotation;
            if (rayTransform != null)
                newRot.SetLookRotation(rayTransform.forward, rayTransform.up);
            transform.rotation = newRot;
        }

        positionSetsThisFrame = 0;
    }

    /// <summary>
    /// 포인터를 숨겨지게 한다.
    /// </summary>
    public void RequestHide()
    {
        if (!dimOnHideRequest)
        {
            Hide();
        }
        lastHideRequestTime = Time.time;
    }

    /// <summary>
    /// 포인터를 포시하게 한다.
    /// </summary>
    public void RequestShow()
    {
        Show();
        lastShowRequestTime = Time.time;
    }


	// Disable/Enable child elements when we show/hide the cursor. For performance reasons.
	void Hide()
	{
		if (showPoint)
		{
			for (int cnt = 0; cnt < transform.childCount; cnt++)
			{
				transform.GetChild(cnt).gameObject.SetActive(false);
			}
		}
		if (GetComponent<Renderer>())
            GetComponent<Renderer>().enabled = false;
        hidden = true;
    }

    void Show()
	{
		if (showPoint)
		{
			for (int cnt = 0; cnt < transform.childCount; cnt++)
			{
				transform.GetChild(cnt).gameObject.SetActive(true);
			}
		}
		if (GetComponent<Renderer>())
            GetComponent<Renderer>().enabled = true;
        hidden = false;
    }

}