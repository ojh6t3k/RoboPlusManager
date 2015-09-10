using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class InfoViewUI : ControlUI
{
	public Text uiText;

	public override void OnUpdateUIInfo()
	{
		ControlItemInfo info = uiInfo;
		if(info != null)
		{
			StringBuilder content = new StringBuilder();
			content.AppendLine(string.Format("-Name: {0}", info.uiName));
			content.AppendLine(string.Format("-Default: {0:d}", info.defaultValue));
			content.AppendLine(string.Format("-Unit: {0:f}", info.unit));
			for(int i=0; i<info.uiParameters.Length; i++)
				content.AppendLine(string.Format("-Parameter[{0:d}]: {1}", i, info.uiParameters[i]));
			uiText.text = content.ToString();
		}
		else
			uiText.text = "";
	}
}
