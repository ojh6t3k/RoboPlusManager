using UnityEngine;
using System.Collections;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("R+ Manager")]
	[Tooltip("DialControl.Reset()")]
	public class DialControlReset : FsmStateAction
	{
		[RequiredField]
		public DialControl dial;

		public override void Reset()
		{
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(dial != null)
				dial.Reset();
			
			Finish();
		}
	}
}
