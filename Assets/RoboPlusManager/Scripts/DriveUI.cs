using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public class DriveUI : ControlUI
{
    public Toggle uiJointMode;
    public Toggle uiWheelMode;
    public RectTransform uiJointView;
    public RectTransform uiWheelView;
    public Button uiSave;
    public Toggle uiMoving;
    public Slider uiCWPresentLoad;
    public Slider uiCCWPresentLoad;
    public Text uiPresentLoadValue;

    public RectTransform uiCWLimitDial;
    public RectTransform uiCCWLimitDial;
    public RectTransform uiGoalPosDial;
    public RectTransform uiPresentPosDial;
    public DialControl uiDialKnob;
    public UpdownValue uiAngleInput;
    public Button uiAngleCenter;
    public Toggle uiCWLimitSelector;
    public Toggle uiCCWLimitSelector;
    public Toggle uiGoalPosSelector;
    public Text uiCWLimitValue;
    public Text uiCCWLimitValue;
    public Text uiGoalPosValue;
    public Text uiPresentPosValue;

    public Slider uiJointSpeedSlider;
    public Slider uiPresentJointSpeedSlider;
    public UpdownValue uiJointSpeedInput;
    public Text uiJointSpeedValue;
    public Text uiPresentJointSpeedValue;

    public Slider uiWheelSpeedSlider;
    public Slider uiPresentWheelSpeedSlider;
    public UpdownValue uiWheelSpeedInput;
    public Text uiWheelSpeedValue;
    public Text uiPresentWheelSpeedValue;
    public Button uiWheelSpeedStop;

    private ControlItemInfo _cwAngleLimit;
    private ControlItemInfo _ccwAngleLimit;
    private ControlItemInfo _driveMode;
    private ControlItemInfo _goalPosition;
    private ControlItemInfo _presentPosition;
    private ControlItemInfo _jointSpeed;
    private ControlItemInfo _presentJointSpeed;
    private ControlItemInfo _moving;
    private ControlItemInfo _presentLoad;
    private ControlItemInfo _wheelSpeed;
    private ControlItemInfo _presentWheelSpeed;

    private bool _preventEvent;
    private bool _initializing;
    private float _minAngle;
    private float _maxAngle;
    private float _minRPM;
    private float _maxRPM;

    #region Override
    protected override void OnInitialize()
    {
        uiJointMode.onValueChanged.AddListener(OnChangedMode);
        uiWheelMode.onValueChanged.AddListener(OnChangedMode);
        uiDialKnob.OnChangedValue.AddListener(OnChangedDialKnob);
        uiAngleInput.OnChangedValue.AddListener(OnChangedValueInput);
        uiAngleCenter.onClick.AddListener(OnPressedCenter);
        uiCWLimitSelector.onValueChanged.AddListener(OnSelectedCWLimit);
        uiCCWLimitSelector.onValueChanged.AddListener(OnSelectedCCWLimit);
        uiGoalPosSelector.onValueChanged.AddListener(OnSelectedGoalPos);
        uiSave.onClick.AddListener(OnSave);
        uiJointSpeedSlider.onValueChanged.AddListener(OnChangedJointSpeed);
        uiJointSpeedInput.OnChangedValue.AddListener(OnChangedJointSpeedInput);
        uiWheelSpeedSlider.onValueChanged.AddListener(OnChangedWheelSpeed);
        uiWheelSpeedInput.OnChangedValue.AddListener(OnChangedWheelSpeedInput);
        uiWheelSpeedStop.onClick.AddListener(OnWheelSpeedStop);
    }

    protected override void OnSetUiInfo()
    {
        ControlUIInfo info = uiInfo;

        string[] tokens = info.uiParameters[0].Split(new char[] { '~' });
        _minAngle = float.Parse(tokens[0]);
        _maxAngle = float.Parse(tokens[1]);
        tokens = info.uiParameters[1].Split(new char[] { '~' });
        _minRPM = float.Parse(tokens[0]);
        _maxRPM = float.Parse(tokens[1]);

        _preventEvent = true;
        _initializing = true;

        _cwAngleLimit = info.GetUIItem("CWAngleLimit");
        _ccwAngleLimit = info.GetUIItem("CCWAngleLimit");
        _goalPosition = info.GetUIItem("GoalPosition");

        _uiCWAngleLimit = _cwAngleLimit.value;
        _uiCCWAngleLimit = _ccwAngleLimit.value;
        if (info.version == 3)
        {
            _driveMode = info.GetUIItem("DriveMode");
            if (_driveMode.value == 1)
                _uiJointMode = false;
            else if (_driveMode.value == 2)
                _uiJointMode = true;
        }
        else
        {
            if (_cwAngleLimit.value == _cwAngleLimit.minValue && _ccwAngleLimit.value == _ccwAngleLimit.minValue)
                _uiJointMode = false;
            else
                _uiJointMode = true;
        }
                
        _uiGoalPosition = _goalPosition.value;

        _presentPosition = info.GetUIItem("PresentPosition");
        _uiPresentPosition = _presentPosition.value;

        _jointSpeed = info.GetUIItem("JointSpeed");
        _uiJointSpeed = _jointSpeed.value;

        _presentJointSpeed = info.GetUIItem("PresentJointSpeed");
        _uiPresentJointSpeed = _presentJointSpeed.value;        

        _moving = info.GetUIItem("Moving");
        _uiMoving = _moving.value;

        _presentLoad = info.GetUIItem("PresentLoad");
        _uiPresentLoad = _presentLoad.value;
        
        _wheelSpeed = info.GetUIItem("WheelSpeed");
        _uiWheelSpeed = _wheelSpeed.value;

        _presentWheelSpeed = info.GetUIItem("PresentWheelSpeed");
        _uiPresentWheelSpeed = _presentWheelSpeed.value;        

        uiSave.interactable = false;

        _preventEvent = false;
        _initializing = false;
    }

    protected override void OnSetCommProduct()
    {
        commProduct.AddReadItem(_cwAngleLimit);
        commProduct.AddReadItem(_ccwAngleLimit);
        commProduct.AddReadItem(_goalPosition);
        commProduct.AddReadItem(_presentPosition);
        commProduct.AddReadItem(_jointSpeed);
        commProduct.AddReadItem(_presentJointSpeed);
        commProduct.AddReadItem(_moving);
        commProduct.AddReadItem(_presentLoad);
        commProduct.AddReadItem(_wheelSpeed);
        commProduct.AddReadItem(_presentWheelSpeed);

        if(uiInfo.version == 3)
        {
            commProduct.AddReadItem(_driveMode);
        }
    }

    protected override void OnUpdateUI()
    {
        bool modify = false;
        _preventEvent = true;

        modify |= _cwAngleLimit.modify;
        modify |= _ccwAngleLimit.modify;

        if (uiInfo.version == 3)
        {
            if(_driveMode.update)
            {
                if (_driveMode.value == 1)
                    _uiJointMode = false;
                else if (_driveMode.value == 2)
                    _uiJointMode = true;
            }

            modify |= _driveMode.modify;
        }

        if (_cwAngleLimit.update || _ccwAngleLimit.update)
        {
            if(uiInfo.version != 3)
            {
                if (_cwAngleLimit.value == _cwAngleLimit.minValue && _ccwAngleLimit.value == _ccwAngleLimit.minValue)
                    _uiJointMode = false;
                else
                    _uiJointMode = true;
            }

            if (_cwAngleLimit.update)
                _uiCWAngleLimit = _cwAngleLimit.value;

            if (_ccwAngleLimit.update)
                _uiCCWAngleLimit = _ccwAngleLimit.value;
        }        

        _uiGoalPosition = _goalPosition.value;

        if (_presentPosition.update)
            _uiPresentPosition = _presentPosition.value;

        if (_jointSpeed.update)
            _uiJointSpeed = _jointSpeed.value;

        if (_presentJointSpeed.update)
            _uiPresentJointSpeed = _presentJointSpeed.value;

        if (_moving.update)
            _uiMoving = _moving.value;

        if (_presentLoad.update)
            _uiPresentLoad = _presentLoad.value;

        if (_wheelSpeed.update)
            _uiWheelSpeed = _wheelSpeed.value;

        if (_presentWheelSpeed.update)
            _uiPresentWheelSpeed = _presentWheelSpeed.value;

        uiSave.interactable = modify;
        _preventEvent = false;
    }

    protected override void OnWriteDone()
    {
    }
    #endregion

    #region Event
    private void OnChangedMode(bool value)
    {
        if (_preventEvent)
            return;

        if(uiInfo.version == 3)
        {
            if (_uiJointMode)
                _driveMode.writeValue = 1;
            else
                _driveMode.writeValue = 2;
        }
        else
        {
            if(_uiJointMode)
            {
                _cwAngleLimit.writeValue = _cwAngleLimit.minValue;
                _ccwAngleLimit.writeValue = _ccwAngleLimit.minValue;                
            }
            else
            {
                _cwAngleLimit.writeValue = _cwAngleLimit.defaultValue;
                _ccwAngleLimit.writeValue = _ccwAngleLimit.defaultValue;
            }
        }
    }

    private void OnChangedDialKnob()
    {
        if (_preventEvent)
            return;

        int dialValue = uiDialKnob.Value;

        if (uiCWLimitSelector.isOn)
        {
            _cwAngleLimit.writeValue = dialValue;
            _uiCWAngleLimit = dialValue;
        }
        else if (uiCCWLimitSelector.isOn)
        {
            _ccwAngleLimit.writeValue = dialValue;
            _uiCCWAngleLimit = dialValue;
        }
        else if (uiGoalPosSelector.isOn)
        {
            _goalPosition.writeValue = dialValue;
            commProduct.SetWriteItem(_goalPosition);
        }
    }

    private void OnChangedValueInput()
    {
        if (_preventEvent)
            return;

        int inputValue = (int)uiAngleInput.Value;        

        if (uiCWLimitSelector.isOn)
        {
            _cwAngleLimit.writeValue = inputValue;
            _uiCWAngleLimit = inputValue;
        }
        else if (uiCCWLimitSelector.isOn)
        {
            _ccwAngleLimit.writeValue = inputValue;
            _uiCCWAngleLimit = inputValue;
        }
        else if (uiGoalPosSelector.isOn)
        {
            _goalPosition.writeValue = inputValue;
            commProduct.SetWriteItem(_goalPosition);
        }
    }

    private void OnPressedCenter()
    {
        if (_preventEvent)
            return;

        _goalPosition.writeValue = CenterValue(_goalPosition.minValue, _goalPosition.maxValue);
        commProduct.SetWriteItem(_goalPosition);
    }

    private void OnSelectedCWLimit(bool value)
    {
        if (_preventEvent)
            return;

        _uiSelector = 0;
    }

    private void OnSelectedCCWLimit(bool value)
    {
        if (_preventEvent)
            return;

        _uiSelector = 1;
    }

    private void OnSelectedGoalPos(bool value)
    {
        if (_preventEvent)
            return;

        _uiSelector = 2;
    }

    private void OnSave()
    {
        if (_preventEvent)
            return;

        commProduct.AddWriteItem(_cwAngleLimit);
        commProduct.AddWriteItem(_ccwAngleLimit);

        if(uiInfo.version == 3)
        {
            commProduct.AddWriteItem(_driveMode);
        }
    }

    private void OnChangedJointSpeed(float value)
    {
        if (_preventEvent)
            return;

        _jointSpeed.writeValue = (int)uiJointSpeedSlider.value;
        commProduct.SetWriteItem(_jointSpeed);
    }

    private void OnChangedJointSpeedInput()
    {
        if (_preventEvent)
            return;

        _jointSpeed.writeValue = (int)uiJointSpeedInput.Value;
        commProduct.SetWriteItem(_jointSpeed);
    }

    private void OnChangedWheelSpeed(float value)
    {
        if (_preventEvent)
            return;

        _wheelSpeed.writeValue = SignValue2Value((int)uiWheelSpeedSlider.value, _wheelSpeed.minValue, _wheelSpeed.maxValue);
        commProduct.SetWriteItem(_wheelSpeed);
    }

    private void OnChangedWheelSpeedInput()
    {
        if (_preventEvent)
            return;

        _wheelSpeed.writeValue = SignValue2Value((int)uiWheelSpeedInput.Value, _wheelSpeed.minValue, _wheelSpeed.maxValue);
        commProduct.SetWriteItem(_wheelSpeed);
    }

    private void OnWheelSpeedStop()
    {
        if (_preventEvent)
            return;

        _wheelSpeed.writeValue = _wheelSpeed.minValue;
        commProduct.SetWriteItem(_wheelSpeed);
    }
    #endregion

    #region UI Control
    private bool _uiJointMode
    {
        get
        {
            return uiJointView.gameObject.activeSelf;
        }
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(value && (!uiJointView.gameObject.activeSelf || _initializing)) // Set Joint Mode
            {
                uiJointMode.isOn = true;
                uiWheelMode.isOn = false;
                uiJointView.gameObject.SetActive(true);
                uiWheelView.gameObject.SetActive(false);

                _uiSelector = 2;
            }
            else if(!value && (uiJointView.gameObject.activeSelf || _initializing)) // Set Wheel Mode
            {
                uiWheelMode.isOn = true;
                uiJointMode.isOn = false;
                uiWheelView.gameObject.SetActive(true);
                uiJointView.gameObject.SetActive(false);
            }

            _preventEvent = backup;
        }
    }

    private int _uiSelector
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(_initializing)
            {
                uiDialKnob.minAngle = _minAngle;
                uiDialKnob.maxAngle = _maxAngle;
            }

            if(value == 0) // Select CW Limit
            {
                uiCWLimitSelector.isOn = true;
                uiCCWLimitSelector.isOn = false;
                uiGoalPosSelector.isOn = false;

                uiDialKnob.centerValue = CenterValue(_cwAngleLimit.minValue, _cwAngleLimit.maxValue);
                uiDialKnob.minValue = _cwAngleLimit.minValue;
                uiDialKnob.maxValue = _cwAngleLimit.maxValue;
                uiDialKnob.Reset();
                uiDialKnob.Value = _cwAngleLimit.value;

                uiAngleInput.minValue = _cwAngleLimit.minValue;
                uiAngleInput.maxValue = _cwAngleLimit.maxValue;
                uiAngleInput.unitValue = 1;
                uiAngleInput.format = "f0";
                uiAngleInput.Value = _cwAngleLimit.value;

                uiAngleCenter.interactable = false;
            }
            else if(value == 1) // Select CCW Limit
            {
                uiCWLimitSelector.isOn = false;
                uiCCWLimitSelector.isOn = true;
                uiGoalPosSelector.isOn = false;

                uiDialKnob.centerValue = CenterValue(_ccwAngleLimit.minValue, _ccwAngleLimit.maxValue);
                uiDialKnob.minValue = _ccwAngleLimit.minValue;
                uiDialKnob.maxValue = _ccwAngleLimit.maxValue;
                uiDialKnob.Reset();
                uiDialKnob.Value = _ccwAngleLimit.value;

                uiAngleInput.minValue = _ccwAngleLimit.minValue;
                uiAngleInput.maxValue = _ccwAngleLimit.maxValue;
                uiAngleInput.unitValue = 1;
                uiAngleInput.format = "f0";
                uiAngleInput.Value = _ccwAngleLimit.value;

                uiAngleCenter.interactable = false;
            }
            else // Select goal Position
            {
                uiCWLimitSelector.isOn = false;
                uiCCWLimitSelector.isOn = false;
                uiGoalPosSelector.isOn = true;

                uiDialKnob.centerValue = CenterValue(_goalPosition.minValue, _goalPosition.maxValue);
                uiDialKnob.minValue = _goalPosition.minValue;
                uiDialKnob.maxValue = _goalPosition.maxValue;
                uiDialKnob.Reset();
                uiDialKnob.Value = _goalPosition.value;

                uiAngleInput.minValue = _goalPosition.minValue;
                uiAngleInput.maxValue = _goalPosition.maxValue;
                uiAngleInput.unitValue = 1;
                uiAngleInput.format = "f0";
                uiAngleInput.Value = _goalPosition.value;

                uiAngleCenter.interactable = true;
            }

            _preventEvent = backup;
        }
    }

    private int _uiCWAngleLimit
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(uiCWLimitSelector.isOn)
            {
                uiDialKnob.Value = value;
                uiAngleInput.Value = value;
            }

            if(_uiJointMode || _initializing)
            {
                Vector3 euler = uiCWLimitDial.localEulerAngles;
                euler.z = Value2Unit(value, _cwAngleLimit.minValue, _cwAngleLimit.maxValue, _minAngle, _maxAngle);
                uiCWLimitValue.text = string.Format("{0:d} ({1:f2}°)", value, euler.z);
                uiCWLimitDial.localEulerAngles = euler;
            }

            _preventEvent = backup;
        }
    }

    private int _uiCCWAngleLimit
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (uiCCWLimitSelector.isOn)
            {
                uiDialKnob.Value = value;
                uiAngleInput.Value = value;
            }

            if (_uiJointMode || _initializing)
            {
                Vector3 euler = uiCCWLimitDial.localEulerAngles;
                euler.z = Value2Unit(value, _ccwAngleLimit.minValue, _ccwAngleLimit.maxValue, _minAngle, _maxAngle);
                uiCCWLimitValue.text = string.Format("{0:d} ({1:f2}°)", value, euler.z);
                uiCCWLimitDial.localEulerAngles = euler;
            }

            _preventEvent = backup;
        }
    }

    private int _uiGoalPosition
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (uiGoalPosSelector.isOn)
            {
                uiDialKnob.Value = value;
                uiAngleInput.Value = value;
            }

            if (_uiJointMode || _initializing)
            {
                Vector3 euler = uiGoalPosDial.localEulerAngles;
                euler.z = Value2Unit(value, _goalPosition.minValue, _goalPosition.maxValue, _minAngle, _maxAngle);
                uiGoalPosValue.text = string.Format("{0:d} ({1:f2}°)", value, euler.z);
                uiGoalPosDial.localEulerAngles = euler;
            }

            _preventEvent = backup;
        }
    }

    private int _uiPresentPosition
    {
        set
        {
            if (_uiJointMode || _initializing)
            {
                Vector3 euler = uiPresentPosDial.localEulerAngles;
                euler.z = Value2Unit(value, _presentPosition.minValue, _presentPosition.maxValue, _minAngle, _maxAngle);
                uiPresentPosValue.text = string.Format("{0:d} ({1:f2}°)", value, euler.z);
                uiPresentPosDial.localEulerAngles = euler;
            }
        }
    }

    private int _uiJointSpeed
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if(_initializing)
            {
                uiJointSpeedSlider.minValue = _jointSpeed.minValue;
                uiJointSpeedSlider.maxValue = _jointSpeed.maxValue;
                uiJointSpeedSlider.wholeNumbers = true;
                
                uiJointSpeedInput.minValue = _jointSpeed.minValue;
                uiJointSpeedInput.maxValue = _jointSpeed.maxValue;
                uiJointSpeedInput.unitValue = 1f;
                uiJointSpeedInput.format = "f0";                
            }

            if (_uiJointMode || _initializing)
            {
                uiJointSpeedSlider.value = value;
                uiJointSpeedInput.Value = value;
                if (value == 0)
                    uiJointSpeedValue.text = string.Format("{0:d} (Max rpm)", value);
                else
                    uiJointSpeedValue.text = string.Format("{0:d} ({1:f1}rpm)",value, Value2Unit(value, _jointSpeed.minValue, _jointSpeed.maxValue, _minRPM, _maxRPM));
            }

            _preventEvent = backup;
        }
    }

    private int _uiPresentJointSpeed
    {
        set
        {
            if (_initializing)
            {
                uiPresentJointSpeedSlider.minValue = _presentJointSpeed.minValue;
                uiPresentJointSpeedSlider.maxValue = _presentJointSpeed.maxValue;
                uiPresentJointSpeedSlider.wholeNumbers = true;                
            }

            if (_uiJointMode || _initializing)
            {
                uiPresentJointSpeedSlider.value = value;
                uiPresentJointSpeedValue.text = string.Format("{0:d} ({1:f1}rpm)", value, Value2Unit(value, _presentJointSpeed.minValue, _presentJointSpeed.maxValue, _minRPM, _maxRPM));
            }
        }
    }

    private int _uiMoving
    {
        set
        {
            uiMoving.isOn = Convert.ToBoolean(value);
        }
    }

    private int _uiPresentLoad
    {
        set
        {
            if (_initializing)
            {
                uiCCWPresentLoad.minValue = 0f;
                uiCCWPresentLoad.maxValue = MaxSignValue(_presentLoad.minValue, _presentLoad.maxValue);
                uiCCWPresentLoad.wholeNumbers = true;
                uiCWPresentLoad.minValue = uiCCWPresentLoad.minValue;
                uiCWPresentLoad.maxValue = uiCCWPresentLoad.maxValue;
                uiCWPresentLoad.wholeNumbers = true;                
            }

            int signValue = Value2SignValue(value, _presentLoad.minValue, _presentLoad.maxValue);
            if (signValue >= 0)
            {
                uiCCWPresentLoad.value = signValue;
                uiCWPresentLoad.value = uiCWPresentLoad.minValue;
            }
            else
            {
                uiCCWPresentLoad.value = uiCCWPresentLoad.minValue;
                uiCWPresentLoad.value = -signValue;
            }
            uiPresentLoadValue.text = string.Format("{0:d} ({1:f1}%)", value, Value2SignUnit(value, _presentLoad.minValue, _presentLoad.maxValue, 0f, 100f));
        }
    }

    private int _uiWheelSpeed
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (_initializing)
            {
                uiWheelSpeedSlider.minValue = -MaxSignValue(_wheelSpeed.minValue, _wheelSpeed.maxValue);
                uiWheelSpeedSlider.maxValue = -uiWheelSpeedSlider.minValue;
                uiWheelSpeedSlider.wholeNumbers = true;
                                
                uiWheelSpeedInput.minValue = uiWheelSpeedSlider.minValue;
                uiWheelSpeedInput.maxValue = uiWheelSpeedSlider.maxValue;
                uiWheelSpeedInput.unitValue = 1f;
                uiWheelSpeedInput.format = "f0";                
            }

            if (!_uiJointMode || _initializing)
            {
                uiWheelSpeedSlider.value = Value2SignValue(value, _wheelSpeed.minValue, _wheelSpeed.maxValue);
                uiWheelSpeedInput.Value = uiWheelSpeedSlider.value;
                uiWheelSpeedValue.text = string.Format("{0:d} ({1:f1}%)", value, Value2SignUnit(value, _wheelSpeed.minValue, _wheelSpeed.maxValue, 0f, 100f));
            }

            _preventEvent = backup;
        }
    }

    private int _uiPresentWheelSpeed
    {
        set
        {
            if (_initializing)
            {
                uiPresentWheelSpeedSlider.minValue = -MaxSignValue(_presentWheelSpeed.minValue, _presentWheelSpeed.maxValue);
                uiPresentWheelSpeedSlider.maxValue = -uiPresentWheelSpeedSlider.minValue;
                uiPresentWheelSpeedSlider.wholeNumbers = true;                
            }

            if (!_uiJointMode || _initializing)
            {
                uiPresentWheelSpeedSlider.value = Value2SignValue(value, _presentWheelSpeed.minValue, _presentWheelSpeed.maxValue);
                uiPresentWheelSpeedValue.text = string.Format("{0:d} ({1:f1}%)", value, Value2SignUnit(value, _presentWheelSpeed.minValue, _presentWheelSpeed.maxValue, 0f, 100f));
            }
        }
    }
    #endregion
}
