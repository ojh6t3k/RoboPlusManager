using UnityEngine;
using System.Collections;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("R+ Manager")]
	[Tooltip("ListView.AddItem()")]
	public class ListViewAddItem : FsmStateAction
	{
		[RequiredField]
		public ListView listView;
		public ListItem listItem;
		public Sprite sprite;
		public FsmString[] textList;
		public FsmObject data;

		public override void Reset()
		{
			listView = null;
			listItem = null;
			sprite = null;
			textList = null;
			data = new FsmObject { UseVariable = true };
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(listView != null)
			{
				ListItem item = GameObject.Instantiate(listItem);
				item.image.sprite = sprite;
				for(int i=0; i<textList.Length; i++)
				{
					if(!textList[i].IsNone)
						item.textList[i].text = textList[i].Value;
				}
				if(!data.IsNone)
					item.data = data.Value;

				listView.AddItem(item);
			}

			Finish();
		}
	}
}
