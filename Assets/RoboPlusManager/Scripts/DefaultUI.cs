﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class DefaultUI : ControlUI
{
	public Text uiText;

	protected override void OnSetUiInfo()
	{
		ControlUIInfo info = uiInfo;
		if(info != null)
		{
			StringBuilder content = new StringBuilder();
			content.AppendLine(string.Format("-Name: {0}", info.name));
			content.AppendLine(string.Format("-Class: {0}", info.uiClass));
			if(info.uiParameters != null)
			{
				for(int i=0; i<info.uiParameters.Length; i++)
					content.AppendLine(string.Format("-Parameter[{0:d}]: {1}", i, info.uiParameters[i]));
			}

			content.AppendLine();

			for(int i=0; i<info.uiItems.Length; i++)
			{
				content.AppendLine(string.Format("-Item[{0:d}]", i));
				content.AppendLine(string.Format("  >Name: {0}", info.uiItems[i].name));
				content.AppendLine(string.Format("  >Address: {0:d}", info.uiItems[i].address));
                content.AppendLine(string.Format("  >Access: {0}", info.uiItems[i].access.ToString()));
                content.AppendLine(string.Format("  >Savable: {0}", info.uiItems[i].savable.ToString()));
                content.AppendLine(string.Format("  >Byte Number: {0:d}", info.uiItems[i].bytes));
				content.AppendLine(string.Format("  >Default: {0:d}", info.uiItems[i].defaultValue));
			}
			uiText.text = content.ToString();
		}
		else
			uiText.text = "";
	}
}
