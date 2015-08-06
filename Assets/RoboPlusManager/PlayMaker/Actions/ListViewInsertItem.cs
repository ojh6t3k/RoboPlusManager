using UnityEngine;
using System.Collections;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("R+ Manager")]
	[Tooltip("ListView.InsertItem()")]
	public class ListViewInsertItem : FsmStateAction
	{
		[RequiredField]
		public ListView listView;
		public ListItem listItem;
		public Sprite sprite;
		public FsmString text;
		public FsmObject data;
		
		public override void Reset()
		{
			listView = null;
			listItem = null;
			sprite = null;
			text = new FsmString { UseVariable = true };
			data = new FsmObject { UseVariable = true };
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(listView != null)
			{
				string t = null;
				if(!text.IsNone)
					t = text.Value;
				
				UnityEngine.Object o = null;
				if(!data.IsNone)
					o = data.Value;
				
				listView.InsertItem(listItem, sprite, t, o);
			}
			
			Finish();
		}
	}
}
