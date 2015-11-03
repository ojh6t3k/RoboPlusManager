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
    private bool _initializing;


    #region Override
    protected override void OnInitialize()
    {
        uiVoltageShutdown.onValueChanged.AddListener(OnChangedAlramShutdown);
        uiAngleShutdown.onValueChanged.AddListener(OnChangedAlramShutdown);
        uiOverheatingShutdown.onValueChanged.AddListener(OnChangedAlramShutdown);
        uiRangeShutdown.onValueChanged.AddListener(OnChangedAlramShutdown);
        uiChecksumShutdown.onValueChanged.AddListener(OnChangedAlramShutdown);
        uiOverloadShutdown.onValueChanged.AddListener(OnChangedAlramShutdown);
        uiInstructionShutdown.onValueChanged.AddListener(OnChangedAlramShutdown);

        uiVoltageLED.onValueChanged.AddListener(OnChangedAlramLED);
        uiAngleLED.onValueChanged.AddListener(OnChangedAlramLED);
        uiOverheatingLED.onValueChanged.AddListener(OnChangedAlramLED);
        uiRangeLED.onValueChanged.AddListener(OnChangedAlramLED);
        uiChecksumLED.onValueChanged.AddListener(OnChangedAlramLED);
        uiOverloadLED.onValueChanged.AddListener(OnChangedAlramLED);
        uiInstructionLED.onValueChanged.AddListener(OnChangedAlramLED);

        uiLED.onValueChanged.AddListener(OnChangedLED);
        uiLEDColor.onValueChanged.AddListener(OnChangedLEDColor);

        uiLowVoltage.onValueChanged.AddListener(OnChangedLowVoltage);
        uiHighVoltage.onValueChanged.AddListener(OnChangedHighVoltage);
        uiLowVoltageInput.OnChangedValue.AddListener(OnChangedLowVoltageInput);
        uiHighVoltageInput.OnChangedValue.AddListener(OnChangedHighVoltageInput);

        uiHighTemperature.onValueChanged.AddListener(OnChangedHighTemperature);
        uiHighTemperatureInput.OnChangedValue.AddListener(OnChangedHighTemperatureInput);

        uiReset.onClick.AddListener(OnReset);
        uiSave.onClick.AddListener(OnSave);
    }

    protected override void OnSetUiInfo()
    {
        ControlUIInfo info = uiInfo;

        _preventEvent = true;
        _initializing = true;

        _alramShutdown = info.GetUIItem("AlramShutdown");
        _uiAlarmShutdown = _alramShutdown.value;
 
        _voltageLowLimit = info.GetUIItem("VoltageLowLimit");        
        _uiLowVoltageLimit = _voltageLowLimit.value;

        _voltageHighLimit = info.GetUIItem("VoltageHighLimit");        
        _uiHighVoltageLimit = _voltageHighLimit.value;

        _voltage = info.GetUIItem("Voltage");        
        _uiCurrentVoltage = _voltage.value;

        _temperatureHighLimit = info.GetUIItem("TemperatureHighLimit");        
        _uiHighTemperatureLimit = _temperatureHighLimit.value;

        _temperature = info.GetUIItem("Temperature");        
        _uiCurrentTemperature = _temperature.value;
        
        _led = info.GetUIItem("LED");

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
            _uiAlarmLED = _alramLED.value;
            _uiLED = _led.value;
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

            _uiLEDColor = _led.value;
        }

        uiSave.interactable = false;

        _preventEvent = false;
        _initializing = false;
    }

    protected override void OnSetCommProduct()
    {
        commProduct.AddReadItem(_alramShutdown);
        commProduct.AddReadItem(_voltageLowLimit);
        commProduct.AddReadItem(_voltageHighLimit);
        commProduct.AddReadItem(_voltage);
        commProduct.AddReadItem(_temperatureHighLimit);
        commProduct.AddReadItem(_temperature);
        commProduct.AddReadItem(_led);

        if (uiInfo.version == 1)
            commProduct.AddReadItem(_alramLED);
    }

    protected override void OnUpdateUI()
    {
        bool modify = false;
        _preventEvent = true;

        modify |= _alramShutdown.modify;
        modify |= _voltageLowLimit.modify;
        modify |= _voltageHighLimit.modify;
        modify |= _temperatureHighLimit.modify;

        if (_alramShutdown.update)
            _uiAlarmShutdown = _alramShutdown.value;

        if(_voltageLowLimit.update)
            _uiLowVoltageLimit = _voltageLowLimit.value;

        if (_voltageHighLimit.update)
            _uiHighVoltageLimit = _voltageHighLimit.value;

        if (_voltage.update)
            _uiCurrentVoltage = _voltage.value;

        if (_temperatureHighLimit.update)
            _uiHighTemperatureLimit = _temperatureHighLimit.value;

        if (_temperature.update)
            _uiCurrentTemperature = _temperature.value;

        if (uiInfo.version == 1)
        {
            modify |= _alramLED.modify;

            if (_alramLED.update)
                _uiAlarmLED = _alramLED.value;

            if (_led.update)
                _uiLED = _led.value;
        }
        else if(uiInfo.version == 2)
        {
            if (_led.update)
                _uiLEDColor = _led.value;
        }

        uiSave.interactable = modify;
        _preventEvent = false;
    }

    protected override void OnWriteDone()
    {
    }
    #endregion

    #region Event
    private void OnChangedAlramShutdown(bool value)
    {
        if (_preventEvent)
            return;

        _alramShutdown.writeValue = _uiAlarmShutdown;
    }

    private void OnChangedAlramLED(bool value)
    {
        if (_preventEvent)
            return;

        _alramLED.writeValue = _uiAlarmLED;

        uiSave.interactable = true;
    }

    private void OnChangedLED(bool value)
    {
        if (_preventEvent)
            return;

        _led.writeValue = _uiLED;
        commProduct.AddWriteItem(_led);
    }

    private void OnChangedLEDColor(int value)
    {
        if (_preventEvent)
            return;

        _led.writeValue = _uiLEDColor;
        commProduct.AddWriteItem(_led);
    }

    private void OnChangedLowVoltage(float value)
    {
        if (_preventEvent)
            return;

        _voltageLowLimit.writeValue = (int)uiLowVoltage.value;
        _uiLowVoltageLimit = _voltageLowLimit.writeValue;
    }

    private void OnChangedHighVoltage(float value)
    {
        if (_preventEvent)
            return;

        _voltageHighLimit.writeValue = (int)uiHighVoltage.value;
        _uiHighVoltageLimit = _voltageHighLimit.writeValue;
    }

    private void OnChangedHighVoltageInput()
    {
        if (_preventEvent)
            return;

        _voltageHighLimit.writeValue = (int)uiHighVoltageInput.Value;
        _uiHighVoltageLimit = _voltageHighLimit.writeValue;
    }

    private void OnChangedLowVoltageInput()
    {
        if (_preventEvent)
            return;

        _voltageLowLimit.writeValue = (int)uiLowVoltageInput.Value;
        _uiLowVoltageLimit = _voltageLowLimit.writeValue;
    }

    private void OnChangedHighTemperature(float value)
    {
        if (_preventEvent)
            return;

        _temperatureHighLimit.writeValue = (int)uiHighTemperature.value;
        _uiHighTemperatureLimit = _temperatureHighLimit.writeValue;
    }

    private void OnChangedHighTemperatureInput()
    {
        if (_preventEvent)
            return;

        _temperatureHighLimit.writeValue = (int)uiHighTemperatureInput.Value;
        _uiHighTemperatureLimit = _temperatureHighLimit.writeValue;
    }

    private void OnReset()
    {
        if (_preventEvent)
            return;

        if(_alramShutdown.value != _alramShutdown.defaultValue)
        {
            _alramShutdown.writeValue = _alramShutdown.defaultValue;
            _uiAlarmShutdown = _alramShutdown.writeValue;
        }

        if (_voltageLowLimit.value != _voltageLowLimit.defaultValue)
        {
            _voltageLowLimit.writeValue = _voltageLowLimit.defaultValue;
            _uiLowVoltageLimit = _voltageLowLimit.writeValue;
        }

        if (_voltageHighLimit.value != _voltageHighLimit.defaultValue)
        {
            _voltageHighLimit.writeValue = _voltageHighLimit.defaultValue;
            _uiHighVoltageLimit = _voltageHighLimit.writeValue;
        }

        if (_temperatureHighLimit.value != _temperatureHighLimit.defaultValue)
        {
            _temperatureHighLimit.writeValue = _temperatureHighLimit.defaultValue;
            _uiHighTemperatureLimit = _temperatureHighLimit.writeValue;
        }

        if (uiInfo.version == 1)
        {
            if (_alramLED.value != _alramLED.defaultValue)
            {
                _alramLED.writeValue = _alramLED.defaultValue;
                _uiAlarmLED = _alramLED.writeValue;
            }
        }        
    }

    private void OnSave()
    {
        if (_preventEvent)
            return;

        commProduct.AddWriteItem(_alramShutdown);
        commProduct.AddWriteItem(_voltageLowLimit);
        commProduct.AddWriteItem(_voltageHighLimit);
        commProduct.AddWriteItem(_voltage);
        commProduct.AddWriteItem(_temperatureHighLimit);

        if (uiInfo.version == 1)
        {
            commProduct.AddWriteItem(_alramLED);
        }
    }
    #endregion

    #region UI Control
    private int _uiAlarmShutdown
    {
        get
        {
            int value = 0;

            if (uiVoltageShutdown.isOn)
                value |= 1;
            else
                value &= (255 - 1);

            if (uiAngleShutdown.isOn)
                value |= 2;
            else
                value &= (255 - 2);

            if (uiOverheatingShutdown.isOn)
                value |= 4;
            else
                value &= (255 - 4);

            if (uiRangeShutdown.isOn)
                value |= 8;
            else
                value &= (255 - 8);

            if (uiChecksumShutdown.isOn)
                value |= 16;
            else
                value &= (255 - 16);

            if (uiOverloadShutdown.isOn)
                value |= 32;
            else
                value &= (255 - 32);

            if (uiInstructionShutdown.isOn)
                value |= 64;
            else
                value &= (255 - 64);

            return value;
        }
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if ((value & 1) == 1)
                uiVoltageShutdown.isOn = true;
            else
                uiVoltageShutdown.isOn = false;
            if ((value & 2) == 2)
                uiAngleShutdown.isOn = true;
            else
                uiAngleShutdown.isOn = false;
            if ((value & 4) == 4)
                uiOverheatingShutdown.isOn = true;
            else
                uiOverheatingShutdown.isOn = false;
            if ((value & 8) == 8)
                uiRangeShutdown.isOn = true;
            else
                uiRangeShutdown.isOn = false;
            if ((value & 16) == 16)
                uiChecksumShutdown.isOn = true;
            else
                uiChecksumShutdown.isOn = false;
            if ((value & 32) == 32)
                uiOverloadShutdown.isOn = true;
            else
                uiOverloadShutdown.isOn = false;
            if ((value & 64) == 64)
                uiInstructionShutdown.isOn = true;
            else
                uiInstructionShutdown.isOn = false;

            _preventEvent = backup;
        }        
    }

    private int _uiAlarmLED
    {
        get
        {
            int value = 0;

            if (uiVoltageLED.isOn)
                value |= 1;
            else
                value &= (255 - 1);

            if (uiAngleLED.isOn)
                value |= 2;
            else
                value &= (255 - 2);

            if (uiOverheatingLED.isOn)
                value |= 4;
            else
                value &= (255 - 4);

            if (uiRangeLED.isOn)
                value |= 8;
            else
                value &= (255 - 8);

            if (uiChecksumLED.isOn)
                value |= 16;
            else
                value &= (255 - 16);

            if (uiOverloadLED.isOn)
                value |= 32;
            else
                value &= (255 - 32);

            if (uiInstructionLED.isOn)
                value |= 64;
            else
                value &= (255 - 64);

            return value;
        }
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if ((value & 1) == 1)
                uiVoltageLED.isOn = true;
            else
                uiVoltageLED.isOn = false;
            if ((value & 2) == 2)
                uiAngleLED.isOn = true;
            else
                uiAngleLED.isOn = false;
            if ((value & 4) == 4)
                uiOverheatingLED.isOn = true;
            else
                uiOverheatingLED.isOn = false;
            if ((value & 8) == 8)
                uiRangeLED.isOn = true;
            else
                uiRangeLED.isOn = false;
            if ((value & 16) == 16)
                uiChecksumLED.isOn = true;
            else
                uiChecksumLED.isOn = false;
            if ((value & 32) == 32)
                uiOverloadLED.isOn = true;
            else
                uiOverloadLED.isOn = false;
            if ((value & 64) == 64)
                uiInstructionLED.isOn = true;
            else
                uiInstructionLED.isOn = false;

            _preventEvent = backup;
        }
    }

    private int _uiLED
    {
        get
        {
            if (uiLED.isOn == true)
                return 1;
            else
                return 0;
        }
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (value == 1)
                uiLED.isOn = true;
            else
                uiLED.isOn = false;

            _preventEvent = backup;
        }
    }

    private int _uiLEDColor
    {
        get
        {
            return uiLEDColor.value;
        }
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            uiLEDColor.value = value;

            _preventEvent = backup;
        }
    }

    private int _uiLowVoltageLimit
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(_initializing)
            {
                uiLowVoltage.minValue = _voltageLowLimit.minValue;
                uiLowVoltage.maxValue = _voltageLowLimit.maxValue;
                uiLowVoltage.wholeNumbers = true;
                uiLowVoltageInput.minValue = _voltageLowLimit.minValue;
                uiLowVoltageInput.maxValue = _voltageLowLimit.maxValue;
                uiLowVoltageInput.unitValue = 1;
                uiLowVoltageInput.format = "f0";
            }

            uiLowVoltage.value = value;
            uiLowVoltageInput.Value = value;
            uiLowVoltageValue.text = string.Format("{0:f1}", value * 0.1f);

            _preventEvent = backup;
        }
    }

    private int _uiHighVoltageLimit
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(_initializing)
            {
                uiHighVoltage.minValue = _voltageHighLimit.minValue;
                uiHighVoltage.maxValue = _voltageHighLimit.maxValue;
                uiHighVoltage.wholeNumbers = true;
                uiHighVoltageInput.minValue = _voltageHighLimit.minValue;
                uiHighVoltageInput.maxValue = _voltageHighLimit.maxValue;
                uiHighVoltageInput.unitValue = 1;
                uiHighVoltageInput.format = "f0";
            }

            uiHighVoltage.value = value;
            uiHighVoltageInput.Value = value;
            uiHighVoltageValue.text = string.Format("{0:f1}",value * 0.1f);

            _preventEvent = backup;
        }
    }

    private int _uiCurrentVoltage
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(_initializing)
            {
                uiCurrentVoltage.minValue = _voltage.minValue;
                uiCurrentVoltage.maxValue = _voltage.maxValue;
                uiCurrentVoltage.wholeNumbers = true;
            }

            uiCurrentVoltage.value = value;
            uiCurrentVoltageValue.text = string.Format("{0:f1}", value * 0.1f);

            _preventEvent = backup;
        }
    }

    private int _uiHighTemperatureLimit
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(_initializing)
            {
                uiHighTemperature.minValue = _temperatureHighLimit.minValue;
                uiHighTemperature.maxValue = _temperatureHighLimit.maxValue;
                uiHighTemperature.wholeNumbers = true;
                uiHighTemperatureInput.minValue = _temperatureHighLimit.minValue;
                uiHighTemperatureInput.maxValue = _temperatureHighLimit.maxValue;
                uiHighTemperatureInput.unitValue = 1;
                uiHighTemperatureInput.format = "f0";
            }

            uiHighTemperature.value = value;
            uiHighTemperatureInput.Value = value;
            uiHighTemperatureValue.text = string.Format("{0:f0}", value);

            _preventEvent = backup;
        }
    }

    private int _uiCurrentTemperature
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(_initializing)
            {
                uiCurrentTemperature.minValue = _temperature.minValue;
                uiCurrentTemperature.maxValue = _temperature.maxValue;
                uiCurrentTemperature.wholeNumbers = true;
            }

            uiCurrentTemperature.value = value;
            uiCurrentTemperatureValue.text = string.Format("{0:f0}", value);

            _preventEvent = backup;
        }
    }
    #endregion
}
