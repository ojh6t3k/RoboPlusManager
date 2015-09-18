using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlarmUI : ControlUI
{
    public Toggle uiVoltageShutdown;
    public Toggle uiAngleShutdown;
    public Toggle uiOverheatingShutdown;
    public Toggle uiRangeShutdown;
    public Toggle uiChecksumShutdown;
    public Toggle uiOverloadShutdown;
    public Toggle uiInstructionShutdown;

    public Text uiTitleAlarmLED;
    public Toggle uiVoltageLED;
    public Toggle uiAngleLED;
    public Toggle uiOverheatingLED;
    public Toggle uiRangeLED;
    public Toggle uiChecksumLED;
    public Toggle uiOverloadLED;
    public Toggle uiInstructionLED;

    public Toggle uiLED;
    public Dropdown uiLEDColor;
    public Text uiLEDColorText;
    public Button uiReset;
    public Button uiSave;

    public Slider uiLowVoltage;
    public Slider uiCurrentVoltage;
    public Slider uiHighVoltage;
    public UpdownValue uiLowVoltageInput;
    public UpdownValue uiHighVoltageInput;
    public Text uiLowVoltageValue;
    public Text uiCurrentVoltageValue;
    public Text uiHighVoltageValue;

    public Slider uiCurrentTemperature;
    public Slider uiHighTemperature;
    public UpdownValue uiHighTemperatureInput;
    public Text uiCurrentTemperatureValue;
    public Text uiHighTemperatureValue;

    private ControlItemInfo _alramShutdown;
    private ControlItemInfo _alramLED;
    private ControlItemInfo _voltageLowLimit;
    private ControlItemInfo _voltageHighLimit;
    private ControlItemInfo _voltage;
    private ControlItemInfo _temperatureHighLimit;
    private ControlItemInfo _temperature;
    private ControlItemInfo _led;

    private bool _preventEvent = false;

    void Update()
    {
    }

    protected override void OnUpdateUIInfo()
    {
        ControlUIInfo info = uiInfo;

        _preventEvent = true;

        _alramShutdown = info.GetUIItem("AlramShutdown");
        _voltageLowLimit = info.GetUIItem("VoltageLowLimit");
        _voltageHighLimit = info.GetUIItem("VoltageHighLimit");
        _voltage = info.GetUIItem("Voltage");
        _temperatureHighLimit = info.GetUIItem("TemperatureHighLimit");
        _temperature = info.GetUIItem("Temperature");
        _led = info.GetUIItem("LED");

        if ((_alramShutdown.value & 1) == 1)
            uiVoltageShutdown.isOn = true;
        else
            uiVoltageShutdown.isOn = false;
        if ((_alramShutdown.value & 2) == 2)
            uiAngleShutdown.isOn = true;
        else
            uiAngleShutdown.isOn = false;
        if ((_alramShutdown.value & 4) == 4)
            uiOverheatingShutdown.isOn = true;
        else
            uiOverheatingShutdown.isOn = false;
        if ((_alramShutdown.value & 8) == 8)
            uiRangeShutdown.isOn = true;
        else
            uiRangeShutdown.isOn = false;
        if ((_alramShutdown.value & 16) == 16)
            uiChecksumShutdown.isOn = true;
        else
            uiChecksumShutdown.isOn = false;
        if ((_alramShutdown.value & 32) == 32)
            uiOverloadShutdown.isOn = true;
        else
            uiOverloadShutdown.isOn = false;
        if ((_alramShutdown.value & 64) == 64)
            uiInstructionShutdown.isOn = true;
        else
            uiInstructionShutdown.isOn = false;

        uiLowVoltage.minValue = _voltageLowLimit.minValue;
        uiLowVoltage.maxValue = _voltageLowLimit.maxValue;
        uiLowVoltage.wholeNumbers = true;
        uiLowVoltage.value = _voltageLowLimit.value;

        uiHighVoltage.minValue = _voltageHighLimit.minValue;
        uiHighVoltage.maxValue = _voltageHighLimit.maxValue;
        uiHighVoltage.wholeNumbers = true;
        uiHighVoltage.value = _voltageHighLimit.value;

        uiCurrentVoltage.minValue = _voltage.minValue;
        uiCurrentVoltage.maxValue = _voltage.maxValue;
        uiCurrentVoltage.wholeNumbers = true;
        uiCurrentVoltage.value = _voltage.value;

        uiLowVoltageInput.minValue = _voltageLowLimit.minValue;
        uiLowVoltageInput.maxValue = _voltageLowLimit.maxValue;
        uiLowVoltageInput.unitValue = 1;
        uiLowVoltageInput.format = "f0";
        uiLowVoltageInput.Value = _voltageLowLimit.value;

        uiHighVoltageInput.minValue = _voltageHighLimit.minValue;
        uiHighVoltageInput.maxValue = _voltageHighLimit.maxValue;
        uiHighVoltageInput.unitValue = 1;
        uiHighVoltageInput.format = "f0";
        uiHighVoltageInput.Value = _voltageHighLimit.value;

        uiLowVoltageValue.text = string.Format("{0:f1}", _voltageLowLimit.value * 0.1f);
        uiCurrentVoltageValue.text = string.Format("{0:f1}", _voltage.value * 0.1f);
        uiHighVoltageValue.text = string.Format("{0:f1}", _voltageHighLimit.value * 0.1f);

        uiHighTemperature.minValue = _temperatureHighLimit.minValue;
        uiHighTemperature.maxValue = _temperatureHighLimit.maxValue;
        uiHighTemperature.wholeNumbers = true;
        uiHighTemperature.value = _temperatureHighLimit.value;

        uiCurrentTemperature.minValue = _temperature.minValue;
        uiCurrentTemperature.maxValue = _temperature.maxValue;
        uiCurrentTemperature.wholeNumbers = true;
        uiCurrentTemperature.value = _temperature.value;

        uiHighTemperatureInput.minValue = _temperatureHighLimit.minValue;
        uiHighTemperatureInput.maxValue = _temperatureHighLimit.maxValue;
        uiHighTemperatureInput.unitValue = 1;
        uiHighTemperatureInput.format = "f0";
        uiHighTemperatureInput.Value = _temperatureHighLimit.value;

        uiCurrentTemperatureValue.text = string.Format("{0:f0}", _temperature.value);
        uiHighTemperatureValue.text = string.Format("{0:f0}", _temperatureHighLimit.value);

        if (info.version == 1)
        {
            uiTitleAlarmLED.gameObject.SetActive(true);
            uiVoltageLED.gameObject.SetActive(true);
            uiAngleLED.gameObject.SetActive(true);
            uiOverheatingLED.gameObject.SetActive(true);
            uiRangeLED.gameObject.SetActive(true);
            uiChecksumLED.gameObject.SetActive(true);
            uiOverloadLED.gameObject.SetActive(true);
            uiInstructionLED.gameObject.SetActive(true);
            uiLED.gameObject.SetActive(true);
            uiLEDColor.gameObject.SetActive(false);
            uiLEDColorText.gameObject.SetActive(false);

            _alramLED = info.GetUIItem("AlramLED");
            if ((_alramLED.value & 1) == 1)
                uiVoltageLED.isOn = true;
            else
                uiVoltageLED.isOn = false;
            if ((_alramLED.value & 2) == 2)
                uiAngleLED.isOn = true;
            else
                uiAngleLED.isOn = false;
            if ((_alramLED.value & 4) == 4)
                uiOverheatingLED.isOn = true;
            else
                uiOverheatingLED.isOn = false;
            if ((_alramLED.value & 8) == 8)
                uiRangeLED.isOn = true;
            else
                uiRangeLED.isOn = false;
            if ((_alramLED.value & 16) == 16)
                uiChecksumLED.isOn = true;
            else
                uiChecksumLED.isOn = false;
            if ((_alramLED.value & 32) == 32)
                uiOverloadLED.isOn = true;
            else
                uiOverloadLED.isOn = false;
            if ((_alramLED.value & 64) == 64)
                uiInstructionLED.isOn = true;
            else
                uiInstructionLED.isOn = false;

            if (_led.value == 1)
                uiLED.isOn = true;
            else
                uiLED.isOn = false;
        }
        else if(info.version == 2)
        {
            uiTitleAlarmLED.gameObject.SetActive(false);
            uiVoltageLED.gameObject.SetActive(false);
            uiAngleLED.gameObject.SetActive(false);
            uiOverheatingLED.gameObject.SetActive(false);
            uiRangeLED.gameObject.SetActive(false);
            uiChecksumLED.gameObject.SetActive(false);
            uiOverloadLED.gameObject.SetActive(false);
            uiInstructionLED.gameObject.SetActive(false);
            uiLED.gameObject.SetActive(false);
            uiLEDColor.gameObject.SetActive(true);
            uiLEDColorText.gameObject.SetActive(true);

            int value = _led.value;
            int n = 0;
            while(value > 0)
            {
                value /= 2;
                n++;
            }
            uiLEDColor.value = n;
        }

        uiSave.interactable = false;
        _preventEvent = false;
    }

    public void OnChangedAlramShutdown()
    {
        if (_preventEvent)
            return;

        if (uiVoltageShutdown.isOn == true)
            _alramShutdown.value |= 1;
        else
            _alramShutdown.value &= (255 - 1);

        if (uiAngleShutdown.isOn == true)
            _alramShutdown.value |= 2;
        else
            _alramShutdown.value &= (255 - 2);

        if (uiOverheatingShutdown.isOn == true)
            _alramShutdown.value |= 4;
        else
            _alramShutdown.value &= (255 - 4);

        if (uiRangeShutdown.isOn == true)
            _alramShutdown.value |= 8;
        else
            _alramShutdown.value &= (255 - 8);

        if (uiChecksumShutdown.isOn == true)
            _alramShutdown.value |= 16;
        else
            _alramShutdown.value &= (255 - 16);

        if (uiOverloadShutdown.isOn == true)
            _alramShutdown.value |= 32;
        else
            _alramShutdown.value &= (255 - 32);

        if (uiInstructionShutdown.isOn == true)
            _alramShutdown.value |= 64;
        else
            _alramShutdown.value &= (255 - 64);

        uiSave.interactable = true;
    }

    public void OnChangedAlramLED()
    {
        if (_preventEvent)
            return;

        if (uiVoltageLED.isOn == true)
            _alramLED.value |= 1;
        else
            _alramLED.value &= (255 - 1);

        if (uiAngleLED.isOn == true)
            _alramLED.value |= 2;
        else
            _alramLED.value &= (255 - 2);

        if (uiOverheatingLED.isOn == true)
            _alramLED.value |= 4;
        else
            _alramLED.value &= (255 - 4);

        if (uiRangeLED.isOn == true)
            _alramLED.value |= 8;
        else
            _alramLED.value &= (255 - 8);

        if (uiChecksumLED.isOn == true)
            _alramLED.value |= 16;
        else
            _alramLED.value &= (255 - 16);

        if (uiOverloadLED.isOn == true)
            _alramLED.value |= 32;
        else
            _alramLED.value &= (255 - 32);

        if (uiInstructionLED.isOn == true)
            _alramLED.value |= 64;
        else
            _alramLED.value &= (255 - 64);

        uiSave.interactable = true;
    }

    public void OnChangedLED()
    {
        if (_preventEvent)
            return;

        if (uiLED.isOn == true)
            _led.value = 1;
        else
            _led.value = 0;
    }

    public void OnChangedLEDColor()
    {
        if (_preventEvent)
            return;

        _led.value = 0;
        for (int i = 0; i < uiLEDColor.value; i++)
            _led.value |= (0x01 << i);
    }

    public void OnChangedLowVoltage()
    {
        if(uiLowVoltage.value > (uiHighVoltage.value - 1))
            uiLowVoltage.value = uiHighVoltage.value - 1;

        if (_preventEvent)
            return;

        _preventEvent = true;

        int value = (int)uiLowVoltage.value;
        if (value != _voltageLowLimit.value)
        {
            _voltageLowLimit.value = value;
            uiLowVoltageInput.Value = value;
            uiLowVoltageValue.text = string.Format("{0:f1}", value * 0.1f);
            uiSave.interactable = true;
        }       

        _preventEvent = false;        
    }

    public void OnChangedHighVoltage()
    {
        if (uiHighVoltage.value < (uiLowVoltage.value + 1))
            uiHighVoltage.value = uiLowVoltage.value + 1;

        if (_preventEvent)
            return;

        _preventEvent = true;

        int value = (int)uiHighVoltage.value;
        if (value != _voltageHighLimit.value)
        {
            _voltageHighLimit.value = value;
            uiHighVoltageInput.Value = value;
            uiHighVoltageValue.text = string.Format("{0:f1}", value * 0.1f);
            uiSave.interactable = true;
        }       

        _preventEvent = false;        
    }

    public void OnChangedHighVoltageInput()
    {
        if (uiHighVoltageInput.Value < (uiLowVoltage.value + 1))
            uiHighVoltageInput.Value = uiLowVoltage.value + 1;

        if (_preventEvent)
            return;

        _preventEvent = true;

        int value = (int)uiHighVoltageInput.Value;
        if (value != _voltageHighLimit.value)
        {
            _voltageHighLimit.value = value;
            uiHighVoltage.value = value;
            uiHighVoltageValue.text = string.Format("{0:f1}", value * 0.1f);
            uiSave.interactable = true;
        }       

        _preventEvent = false;        
    }

    public void OnChangedLowVoltageInput()
    {
        if (uiLowVoltageInput.Value > (uiHighVoltage.value - 1))
            uiLowVoltageInput.Value = uiHighVoltage.value - 1;

        if (_preventEvent)
            return;

        _preventEvent = true;

        int value = (int)uiLowVoltageInput.Value;
        if (value != _voltageLowLimit.value)
        {
            _voltageLowLimit.value = value;
            uiLowVoltage.value = value;
            uiLowVoltageValue.text = string.Format("{0:f1}", value * 0.1f);
            uiSave.interactable = true;
        }       

        _preventEvent = false;        
    }

    public void OnChangedHighTemperature()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        int value = (int)uiHighTemperature.value;
        if (value != _temperatureHighLimit.value)
        {
            _temperatureHighLimit.value = value;
            uiHighTemperatureInput.Value = value;
            uiHighTemperatureValue.text = string.Format("{0:f0}", value);
            uiSave.interactable = true;
        }

        _preventEvent = false;
    }

    public void OnChangedHighTemperatureInput()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        int value = (int)uiHighTemperatureInput.Value;
        if (value != _temperatureHighLimit.value)
        {
            _temperatureHighLimit.value = value;
            uiHighTemperature.value = value;
            uiHighTemperatureValue.text = string.Format("{0:f0}", value);
            uiSave.interactable = true;
        }

        _preventEvent = false;
    }

    public void OnReset()
    {
        if (_preventEvent)
            return;
    }

    public void OnSave()
    {
        if (_preventEvent)
            return;

        uiSave.interactable = false;
    }
}
