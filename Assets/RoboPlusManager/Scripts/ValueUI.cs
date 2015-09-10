using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ValueUI : ControlUI
{
	public UpdownValue uiUpDown;
	public Text uiValue;
	public Text uiUnitValue;

	private int _value;

	public override void OnUpdateUIInfo()
	{
		ControlItemInfo info = uiInfo;
		if(info != null)
		{
			defaultValue = info.defaultValue;
			unit = info.unit;

			string[] token = info.uiParameters[0].Split(new char[] { '~' });
			uiUpDown.minValue = float.Parse(token[0]);
			uiUpDown.maxValue = float.Parse(token[1]);
			uiUpDown.initValue = defaultValue;
			uiUpDown.unitValue = 1.0f;

			uiUpDown.gameObject.SetActive(info.accessWrite);
			uiValue.gameObject.SetActive(!info.accessWrite);
			if(info.unitFormat == null)
				uiUnitValue.gameObject.SetActive(false);
			else
				uiUnitValue.gameObject.SetActive(true);

			uiReset.gameObject.SetActive(!info.accessRead);
			uiSave.gameObject.SetActive((info.isROM & !info.accessRead));

			Value = defaultValue;
		}
	}

	public override int Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
			if(uiUpDown.isActiveAndEnabled)
				uiUpDown.Value = value;
			else
				uiValue.text = _value.ToString();

			if(uiUnitValue.isActiveAndEnabled)
				uiUnitValue.text = string.Format(uiInfo.unitFormat, _value * uiInfo.unit);
		}
	}

	public override void Reset()
	{
		Value = defaultValue;
	}

	public void OnChangedValue()
	{
		Value = (int)uiUpDown.Value;
	}
}
