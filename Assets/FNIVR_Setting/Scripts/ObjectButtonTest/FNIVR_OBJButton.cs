using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FNIVR_OBJButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public GameObject target;

	private bool isOn = false;
	private bool isClick = false;

	private UnityAction action;

	private void Start()
	{
		gameObject.layer = LayerMask.NameToLayer("UI");

		if (target)
			action += delegate{ target.SetActive(!target.activeSelf); };
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Enter();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Exit();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Down();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Up();
	}

	private void Enter()
	{
		transform.localScale = Vector3.one * 1.1f;
		isOn = true;
	}
	private void Exit()
	{
		transform.localScale = Vector3.one;
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
		isOn = false;
	}
	private void Down()
	{
		transform.localScale = new Vector3(1.1f, 1.1f, 0.1f);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.5f);

		if (isOn)
			isClick = true;
	}
	private void Up()
	{
		transform.localScale = Vector3.one * (isOn ? 1.1f : 1f);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);

		if (isOn && isClick && action != null)
			action();

		isClick = false;
	}
}
