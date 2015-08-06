using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;


public class ListViewProxy : MonoBehaviour
{
	public readonly string builtInOnChangedSelection = "LIST VIEW / ON CHANGED SELECTION";

	public string eventOnChangedSelection = "LIST VIEW / ON CHANGED SELECTION";

	private ListView _listView;
	private PlayMakerFSM _fsm;
	private FsmEventTarget _fsmEventTarget;
		
	// Use this for initialization
	void Start ()
	{
		_fsm = FindObjectOfType<PlayMakerFSM>();
		if(_fsm == null)
			_fsm = gameObject.AddComponent<PlayMakerFSM>();
		
		_listView = GetComponent<ListView>();
		if(_listView != null)
		{
			_listView.OnChangedSelection.AddListener(OnChangedSelection);
		}
		
		_fsmEventTarget = new FsmEventTarget();
		_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
		_fsmEventTarget.excludeSelf = false;
	}
		
	// Update is called once per frame
	void Update ()
	{
			
	}
		
	private void OnChangedSelection()
	{
		_fsm.Fsm.Event(_fsmEventTarget, eventOnChangedSelection);
	}
}
