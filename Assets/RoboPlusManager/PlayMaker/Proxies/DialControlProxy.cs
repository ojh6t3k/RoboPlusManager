using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;


public class DialControlProxy : MonoBehaviour
{
	public readonly string builtInOnChangedValue = "DIAL / ON CHANGED VALUE";
	
	public string eventOnChangedValue = "DIAL / ON CHANGED VALUE";
	
	private DialControl _dial;
	private PlayMakerFSM _fsm;
	private FsmEventTarget _fsmEventTarget;
	
	// Use this for initialization
	void Start ()
	{
		_fsm = FindObjectOfType<PlayMakerFSM>();
		if(_fsm == null)
			_fsm = gameObject.AddComponent<PlayMakerFSM>();
		
		_dial = GetComponent<DialControl>();
		if(_dial != null)
		{
			_dial.OnChangedValue.AddListener(OnChangedValue);
		}
		
		_fsmEventTarget = new FsmEventTarget();
		_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
		_fsmEventTarget.excludeSelf = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	private void OnChangedValue()
	{
		_fsm.Fsm.Event(_fsmEventTarget, eventOnChangedValue);
	}
}
