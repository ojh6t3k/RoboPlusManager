using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(RectTransform))]
public class DialControl : MonoBehaviour, IPointerDownHandler, IDragHandler
{
	public bool interactable = true;
	public RectTransform knob;
	public float centerAngle = 0f;
	public float minAngle = -180f;
	public float maxAngle = 180f;
	public bool cw = false;
	public int centerValue = 512;
	public int minValue = 0;
    public int maxValue = 1023;

	public UnityEvent OnChangedValue;

	private RectTransform _rectTransform;
	private float _angle = 0f;
	private Vector2 _prePos;
	private float _sumAngle;
	private float _centerAngle;


	// Use this for initialization
	void Start ()
	{
		_rectTransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnEnable()
	{
		Reset();
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _prePos);
		_sumAngle = knob.localEulerAngles.z - _centerAngle;
		if(_sumAngle > 180f)
			_sumAngle -= 360f;
		else if(_sumAngle < -180f)
			_sumAngle += 360f;
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		if(interactable == false)
			return;

		Vector2 pos;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out pos))
		{
			float a = Vector2.Angle(_prePos, pos);
			Vector3 axis = Vector3.Cross(_prePos, pos);
			if(axis.z < 0f)
				a = -a;

			_sumAngle += a;
			Vector3 eulerAngles = knob.localEulerAngles;
			a = _centerAngle + Mathf.Clamp(_sumAngle, minAngle, maxAngle);
			if(Mathf.Approximately(eulerAngles.z, a) == false)
			{
				eulerAngles.z = a;
				knob.localEulerAngles = eulerAngles;
				OnChangedValue.Invoke();
			}
			_prePos = pos;
		}
	}

	public void Reset()
	{
		_centerAngle = centerAngle;
		if(cw == true)
			_centerAngle = -_centerAngle;

		if(knob != null)
		{
			Vector3 eulerAngles = knob.localEulerAngles;
			eulerAngles.z = _centerAngle;
			knob.localEulerAngles = eulerAngles;
		}
	}

	public float angle
	{
		set
		{
			if(knob == null)
				return;

			float a = Mathf.Clamp(value, minAngle, maxAngle);
			if(cw == true)
				a = -a;

			Vector3 eulerAngles = knob.localEulerAngles;
			eulerAngles.z = _centerAngle + a;
			knob.localEulerAngles = eulerAngles;
		}
		get
		{
			if(knob == null)
				return 0f;

			float a = knob.localEulerAngles.z - _centerAngle;
			if(a > 180f)
				a -= 360f;
			else if(a < -180f)
				a += 360f;

			if(cw == true)
				a = -a;

			return a;
		}
	}

	public int Value
	{
		set
		{
			if(knob == null)
				return;

            float value2Angle = Mathf.Abs(maxAngle - minAngle) / Mathf.Abs(maxValue - minValue + 1);
            angle = (Mathf.Clamp(value, minValue, maxValue) - centerValue) * value2Angle;
		}
		get
		{
			float a = angle;
            float angle2Value = Mathf.Abs(maxValue - minValue + 1) / Mathf.Abs(maxAngle - minAngle);

            return Mathf.RoundToInt(centerValue + a * angle2Value);
		}
	}
}
