using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class InformationUI : ControlUI
{
	public Text uiModelNumber;
	public Text uiVersion;

	private ControlItemInfo _modelNumber;
	private ControlItemInfo _version;
	
	public override void OnUpdateUIInfo()
	{
		ControlUIInfo info = uiInfo;

		_modelNumber = info.GetUIItem("ModelNumber");
		_version = info.GetUIItem("Version");

		uiModelNumber.text = _modelNumber.defaultValue.ToString();
		uiVersion.text = _version.defaultValue.ToString();
	}
}
