using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class RepeatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public bool interactable = true;
	public float delayTime = 1f;

	public UnityEvent OnRepeated;

	private bool _press = false;
	private float _time;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(_press == true)
		{
			if(_time > delayTime)
			{
				OnRepeated.Invoke();
			}
			_time += Time.deltaTime;
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		if(interactable == false)
			return;

		_time = 0f;
		_press = true;
		OnRepeated.Invoke();
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		if(interactable == false)
			return;

		_press = false;
	}
}
