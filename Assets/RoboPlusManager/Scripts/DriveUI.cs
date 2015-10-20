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

    private ControlItemInfo _cwAngleLimit;
    private ControlItemInfo _ccwAngleLimit;
    private ControlItemInfo _goalPosition;
    private ControlItemInfo _presentPosition;
    private ControlItemInfo _jointSpeed;
    private ControlItemInfo _presentJointSpeed;
    private ControlItemInfo _moving;
    private ControlItemInfo _presentLoad;
    private ControlItemInfo _wheelSpeed;
    private ControlItemInfo _presentWheelSpeed;

    private bool _preventEvent;
    private float _minAngle;
    private float _maxAngle;
    private float _minRPM;
    private float _maxRPM;

    float _sign1 = 1f;
    float _sign2 = 1f;
    float _sign3 = 1f;
    float _sign4 = 1f;
    void Update()
    {
        
        if (uiJointMode.isOn)
        {
            if (_presentPosition != null)
            {
                /*
                _presentPosition.value += (int)_sign1;
                if (_presentPosition.value <= _presentPosition.minValue)
                {
                    _presentPosition.value = _presentPosition.minValue;
                    _sign1 = 1f;
                }
                else if (_presentPosition.value >= _presentPosition.maxValue)
                {
                    _presentPosition.value = _presentPosition.maxValue;
                    _sign1 = -1f;
                }
                */

                RefreshPresentPosition();
            }            

            if (_presentJointSpeed != null)
            {
                /*
                _presentJointSpeed.value += (int)_sign3;
                if (_presentJointSpeed.value <= _presentJointSpeed.minValue)
                {
                    _presentJointSpeed.value = _presentJointSpeed.minValue;
                    _sign3 = 1f;
                }
                else if (_presentJointSpeed.value >= _presentJointSpeed.maxValue)
                {
                    _presentJointSpeed.value = _presentJointSpeed.maxValue;
                    _sign3 = -1f;
                }
                */

                RefreshPresentJointSpeed();
            }
        }
        else if(uiWheelMode.isOn)
        {
            if (_presentWheelSpeed != null)
            {
                /*
                _presentWheelSpeed.value += (int)_sign4;
                if (_presentWheelSpeed.value <= _presentWheelSpeed.minValue)
                {
                    _presentWheelSpeed.value = _presentWheelSpeed.minValue;
                    _sign4 = 1f;
                }
                else if (_presentWheelSpeed.value >= _presentWheelSpeed.maxValue)
                {
                    _presentWheelSpeed.value = _presentWheelSpeed.maxValue;
                    _sign4 = -1f;
                }
                */

                RefreshPresentWheelSpeed();
            }
        }

        if (_presentLoad != null)
        {
            /*
            _presentLoad.value += (int)_sign2;
            if (_presentLoad.value <= _presentLoad.minValue)
            {
                _presentLoad.value = _presentLoad.minValue;
                _sign2 = 1f;
            }
            else if (_presentLoad.value >= _presentLoad.maxValue)
            {
                _presentLoad.value = _presentLoad.maxValue;
                _sign2 = -1f;
            }
            */

            RefreshPresentLoadValue();
        }        
    }

    protected override void OnUpdateUIInfo()
    {
        ControlUIInfo info = uiInfo;

        string[] tokens = info.uiParameters[0].Split(new char[] { '~' });
        _minAngle = float.Parse(tokens[0]);
        _maxAngle = float.Parse(tokens[1]);
        tokens = info.uiParameters[1].Split(new char[] { '~' });
        _minRPM = float.Parse(tokens[0]);
        _maxRPM = float.Parse(tokens[1]);
        
        _cwAngleLimit = info.GetUIItem("CWAngleLimit");
        _ccwAngleLimit = info.GetUIItem("CCWAngleLimit");
        _goalPosition = info.GetUIItem("GoalPosition");
        _presentPosition = info.GetUIItem("PresentPosition");
        _jointSpeed = info.GetUIItem("JointSpeed");
        _presentJointSpeed = info.GetUIItem("PresentJointSpeed");
        _moving = info.GetUIItem("Moving");
        _presentLoad = info.GetUIItem("PresentLoad");
        _wheelSpeed = info.GetUIItem("WheelSpeed");
        _presentWheelSpeed = info.GetUIItem("PresentWheelSpeed");        

        _preventEvent = true;        

        if (_cwAngleLimit.value == _cwAngleLimit.minValue && _ccwAngleLimit.value == _ccwAngleLimit.minValue)
        {
            uiWheelMode.isOn = true;
            uiJointMode.isOn = false;            
        }
        else
        {
            uiWheelMode.isOn = false;
            uiJointMode.isOn = true;
        }

        uiDialKnob.minAngle = _minAngle;
        uiDialKnob.maxAngle = _maxAngle;
        uiDialKnob.centerAngle = 0f;

        uiJointSpeedSlider.minValue = _jointSpeed.minValue;
        uiJointSpeedSlider.maxValue = _jointSpeed.maxValue;
        uiJointSpeedSlider.wholeNumbers = true;

        uiPresentJointSpeedSlider.minValue = _presentJointSpeed.minValue;
        uiPresentJointSpeedSlider.maxValue = _presentJointSpeed.maxValue;
        uiPresentJointSpeedSlider.wholeNumbers = true;

        uiJointSpeedInput.minValue = _jointSpeed.minValue;
        uiJointSpeedInput.maxValue = _jointSpeed.maxValue;
        uiJointSpeedInput.unitValue = 1f;
        uiJointSpeedInput.format = "f0";

        uiCCWPresentLoad.minValue = 0f;
        uiCCWPresentLoad.maxValue = MaxSignValue(_presentLoad.minValue, _presentLoad.maxValue);
        uiCCWPresentLoad.wholeNumbers = true;

        uiCWPresentLoad.minValue = uiCCWPresentLoad.minValue;
        uiCWPresentLoad.maxValue = uiCCWPresentLoad.maxValue;
        uiCWPresentLoad.wholeNumbers = true;

        uiPresentWheelSpeedSlider.minValue = -MaxSignValue(_presentWheelSpeed.minValue, _presentWheelSpeed.maxValue);
        uiPresentWheelSpeedSlider.maxValue = -uiPresentWheelSpeedSlider.minValue;
        uiPresentWheelSpeedSlider.wholeNumbers = true;

        uiWheelSpeedSlider.minValue = uiPresentWheelSpeedSlider.minValue;
        uiWheelSpeedSlider.maxValue = uiPresentWheelSpeedSlider.maxValue;
        uiWheelSpeedSlider.wholeNumbers = true;

        uiWheelSpeedInput.minValue = uiPresentWheelSpeedSlider.minValue;
        uiWheelSpeedInput.maxValue = uiPresentWheelSpeedSlider.maxValue;
        uiWheelSpeedInput.unitValue = 1f;
        uiWheelSpeedInput.format = "f0";

        uiSave.interactable = false;
        uiMoving.isOn = Convert.ToBoolean(_moving.value);
        RefreshPresentLoadValue();
        RefreshModeView();
                
        _preventEvent = false;
    }

    #region Refresh
    private void RefreshModeView()
    {
        uiJointView.gameObject.SetActive(uiJointMode.isOn);
        uiWheelView.gameObject.SetActive(uiWheelMode.isOn);

        if (uiJointMode.isOn)
        {
            uiCWLimitSelector.isOn = false;
            uiCCWLimitSelector.isOn = false;
            uiGoalPosSelector.isOn = true;
            uiAngleCenter.interactable = true;

            RefreshPositionInput();
            RefreshCWLimit();
            RefreshCCWLimit();
            RefreshGoalPosition();
            RefreshPresentPosition();

            uiJointSpeedSlider.value = _jointSpeed.value;
            uiJointSpeedInput.Value = _jointSpeed.value;

            RefreshJointSpeedValue();
            RefreshPresentJointSpeed();

            if(commProduct != null)
            {
                commProduct.AddReadItem(_presentPosition);
                commProduct.AddReadItem(_presentJointSpeed);
                commProduct.AddReadItem(_presentLoad);
            }
        }

        if (uiWheelMode.isOn)
        {
            uiWheelSpeedSlider.value = Value2SignValue(_wheelSpeed.value, _wheelSpeed.minValue, _wheelSpeed.maxValue);
            uiWheelSpeedInput.Value = uiWheelSpeedSlider.value;

            RefreshWheelSpeedValue();
            RefreshPresentWheelSpeed();

            if (commProduct != null)
            {
                commProduct.RemoveReadItem(_presentPosition);
                commProduct.RemoveReadItem(_presentJointSpeed);
                commProduct.RemoveReadItem(_presentLoad);
            }            
        }
    }

    private void RefreshCWLimit()
    {
        Vector3 euler = uiCWLimitDial.localEulerAngles;
        euler.z = Value2Unit(_cwAngleLimit.value, _cwAngleLimit.minValue, _cwAngleLimit.maxValue, _minAngle, _maxAngle);
        uiCWLimitValue.text = string.Format("{0:d} ({1:f2}°)", _cwAngleLimit.value, euler.z);
        uiCWLimitDial.localEulerAngles = euler;
    }

    private void RefreshCCWLimit()
    {
        Vector3 euler = uiCCWLimitDial.localEulerAngles;
        euler.z = Value2Unit(_ccwAngleLimit.value, _ccwAngleLimit.minValue, _ccwAngleLimit.maxValue, _minAngle, _maxAngle);
        uiCCWLimitValue.text = string.Format("{0:d} ({1:f2}°)", _ccwAngleLimit.value, euler.z);
        uiCCWLimitDial.localEulerAngles = euler;
    }

    private void RefreshGoalPosition()
    {
        Vector3 euler = uiGoalPosDial.localEulerAngles;
        euler.z = Value2Unit(_goalPosition.value, _goalPosition.minValue, _goalPosition.maxValue, _minAngle, _maxAngle);
        uiGoalPosValue.text = string.Format("{0:d} ({1:f2}°)", _goalPosition.value, euler.z);
        uiGoalPosDial.localEulerAngles = euler;
    }

    private void RefreshPresentPosition()
    {
        Vector3 euler = uiPresentPosDial.localEulerAngles;
        euler.z = Value2Unit(_presentPosition.value, _presentPosition.minValue, _presentPosition.maxValue, _minAngle, _maxAngle);
        uiPresentPosValue.text = string.Format("{0:d} ({1:f2}°)", _presentPosition.value, euler.z);
        uiPresentPosDial.localEulerAngles = euler;
    }

    private void RefreshPositionInput()
    {
        int value, min, max;

        if (uiCWLimitSelector.isOn)
        {
            value = _cwAngleLimit.value;
            min = _cwAngleLimit.minValue;
            max = _cwAngleLimit.maxValue;
        }
        else if (uiCCWLimitSelector.isOn)
        {
            value = _ccwAngleLimit.value;
            min = _ccwAngleLimit.minValue;
            max = _ccwAngleLimit.maxValue;
        }
        else
        {
            value = _goalPosition.value;
            min = _goalPosition.minValue;
            max = _goalPosition.maxValue;
        }

        uiDialKnob.centerValue = CenterValue(min, max);
        uiDialKnob.minValue = min;
        uiDialKnob.maxValue = max;
        uiDialKnob.Reset();
        uiDialKnob.Value = value;

        uiAngleInput.minValue = min;
        uiAngleInput.maxValue = max;
        uiAngleInput.unitValue = 1;
        uiAngleInput.format = "f0";
        uiAngleInput.Value = value;
    }

    private void RefreshJointSpeedValue()
    {
        if (_jointSpeed.value == 0)
            uiJointSpeedValue.text = string.Format("{0:d} (Max rpm)", _jointSpeed.value);
        else
            uiJointSpeedValue.text = string.Format("{0:d} ({1:f1}rpm)", _jointSpeed.value, Value2Unit(_jointSpeed.value, _jointSpeed.minValue, _jointSpeed.maxValue, _minRPM, _maxRPM));
    }

    private void RefreshPresentJointSpeed()
    {
        uiPresentJointSpeedSlider.value = _presentJointSpeed.value;
        uiPresentJointSpeedValue.text = string.Format("{0:d} ({1:f1}rpm)", _presentJointSpeed.value, Value2Unit(_presentJointSpeed.value, _presentJointSpeed.minValue, _presentJointSpeed.maxValue, _minRPM, _maxRPM));
    }

    private void RefreshPresentLoadValue()
    {
        int signValue = Value2SignValue(_presentLoad.value, _presentLoad.minValue, _presentLoad.maxValue);
        if(signValue >= 0)
        {
            uiCCWPresentLoad.value = signValue;
            uiCWPresentLoad.value = uiCWPresentLoad.minValue;
        }
        else
        {
            uiCCWPresentLoad.value = uiCCWPresentLoad.minValue;
            uiCWPresentLoad.value = -signValue;
        }
        
        uiPresentLoadValue.text = string.Format("{0:d} ({1:f1}%)", _presentLoad.value, Value2SignUnit(_presentLoad.value, _presentLoad.minValue, _presentLoad.maxValue, 0f, 100f));
    }

    private void RefreshWheelSpeedValue()
    {
        uiWheelSpeedValue.text = string.Format("{0:d} ({1:f1}%)", _wheelSpeed.value, Value2SignUnit(_wheelSpeed.value, _wheelSpeed.minValue, _wheelSpeed.maxValue, 0f, 100f));
    }

    private void RefreshPresentWheelSpeed()
    {
        uiPresentWheelSpeedSlider.value = Value2SignValue(_presentWheelSpeed.value, _presentWheelSpeed.minValue, _presentWheelSpeed.maxValue);
        uiPresentWheelSpeedValue.text = string.Format("{0:d} ({1:f1}%)", _presentWheelSpeed.value, Value2SignUnit(_presentWheelSpeed.value, _presentWheelSpeed.minValue, _presentWheelSpeed.maxValue, 0f, 100f));
    }
    #endregion

    #region Event
    public void OnChangedMode()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        if (uiWheelMode.isOn)
        {
            _cwAngleLimit.value = _cwAngleLimit.minValue;
            _ccwAngleLimit.value = _cwAngleLimit.value;
        }

        if (uiJointMode.isOn)
        {
            _cwAngleLimit.value = _cwAngleLimit.defaultValue;
            _ccwAngleLimit.value = _ccwAngleLimit.defaultValue;
        }

        RefreshModeView();

        uiSave.interactable = true;
        _preventEvent = false;
    }

    public void OnChangedDialKnob()
    {
        int dialValue = uiDialKnob.Value;

        if (uiCWLimitSelector.isOn)
        {
            if (dialValue > (_ccwAngleLimit.value - 1))
                uiDialKnob.Value = (_ccwAngleLimit.value - 1);
        }

        if (uiCCWLimitSelector.isOn)
        {
            if (dialValue < (_cwAngleLimit.value + 1))
                uiDialKnob.Value = (_cwAngleLimit.value + 1);
        }

        if(uiGoalPosSelector.isOn)
        {
            if (dialValue > _ccwAngleLimit.value)
                uiDialKnob.Value = _ccwAngleLimit.value;
            else if (dialValue < _cwAngleLimit.value)
                uiDialKnob.Value = _cwAngleLimit.value;
        }

        if (_preventEvent)
            return;

        _preventEvent = true;

        dialValue = uiDialKnob.Value;
        uiAngleInput.Value = dialValue;

        if (uiCWLimitSelector.isOn)
        {
            if (dialValue != _cwAngleLimit.value)
            {
                _cwAngleLimit.value = dialValue;
                RefreshCWLimit();
                uiSave.interactable = true;
            }
        }
        else if(uiCCWLimitSelector.isOn)
        {
            if (dialValue != _ccwAngleLimit.value)
            {
                _ccwAngleLimit.value = dialValue;
                RefreshCCWLimit();
                uiSave.interactable = true;
            }
        }
        else if (uiGoalPosSelector.isOn)
        {
            if (dialValue != _goalPosition.value)
            {
                _goalPosition.value = dialValue;
                commProduct.SetWriteItem(_goalPosition);
                RefreshGoalPosition();
            }
        }

        _preventEvent = false;
    }

    public void OnChangedValueInput()
    {
        int inputValue = (int)uiAngleInput.Value;

        if (uiCWLimitSelector.isOn)
        {
            if (inputValue > (_ccwAngleLimit.value - 1))
                uiAngleInput.Value = (_ccwAngleLimit.value - 1);
        }

        if (uiCCWLimitSelector.isOn)
        {
            if (inputValue < (_cwAngleLimit.value + 1))
                uiAngleInput.Value = (_cwAngleLimit.value + 1);
        }

        if (uiGoalPosSelector.isOn)
        {
            if (inputValue > _ccwAngleLimit.value)
                uiAngleInput.Value = _ccwAngleLimit.value;
            else if (inputValue < _cwAngleLimit.value)
                uiAngleInput.Value = _cwAngleLimit.value;
        }

        if (_preventEvent)
            return;

        _preventEvent = true;

        inputValue = (int)uiAngleInput.Value;
        uiDialKnob.Value = inputValue;

        if (uiCWLimitSelector.isOn)
        {
            if (inputValue != _cwAngleLimit.value)
            {
                _cwAngleLimit.value = inputValue;
                RefreshCWLimit();
                uiSave.interactable = true;
            }
        }
        else if (uiCCWLimitSelector.isOn)
        {
            if (inputValue != _ccwAngleLimit.value)
            {
                _ccwAngleLimit.value = inputValue;
                RefreshCCWLimit();
                uiSave.interactable = true;
            }
        }
        else if (uiGoalPosSelector.isOn)
        {
            if (inputValue != _goalPosition.value)
            {
                _goalPosition.value = inputValue;
                commProduct.AddWriteItem(_goalPosition);
                RefreshGoalPosition();
            }
        }

        _preventEvent = false;
    }

    public void OnPressedCenter()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        _goalPosition.value = CenterValue(_goalPosition.minValue, _goalPosition.maxValue);

        uiDialKnob.Value = _goalPosition.value;
        uiAngleInput.Value = _goalPosition.value;

        RefreshGoalPosition();

        _preventEvent = false;
    }

    public void OnSelectedCWLimit()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        RefreshPositionInput();
        uiAngleCenter.interactable = false;

        _preventEvent = false;
    }

    public void OnSelectedCCWLimit()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        RefreshPositionInput();
        uiAngleCenter.interactable = false;

        _preventEvent = false;
    }

    public void OnSelectedGoalPos()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        RefreshPositionInput();
        uiAngleCenter.interactable = true;

        _preventEvent = false;
    }

    public void OnSave()
    {
        if (_preventEvent)
            return;

        uiSave.interactable = false;
    }

    public void OnChangedJointSpeed()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        _jointSpeed.value = (int)uiJointSpeedSlider.value;
        uiJointSpeedInput.Value = _jointSpeed.value;
        RefreshJointSpeedValue();

        _preventEvent = false;
    }

    public void OnChangedJointSpeedInput()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        _jointSpeed.value = (int)uiJointSpeedInput.Value;
        uiJointSpeedSlider.value = _jointSpeed.value;
        RefreshJointSpeedValue();

        _preventEvent = false;
    }

    public void OnChangedWheelSpeed()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        _wheelSpeed.value = SignValue2Value((int)uiWheelSpeedSlider.value, _wheelSpeed.minValue, _wheelSpeed.maxValue);
        uiWheelSpeedInput.Value = uiWheelSpeedSlider.value;
        RefreshWheelSpeedValue();

        _preventEvent = false;
    }

    public void OnChangedWheeSpeedInput()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        _wheelSpeed.value = SignValue2Value((int)uiWheelSpeedInput.Value, _wheelSpeed.minValue, _wheelSpeed.maxValue);
        uiWheelSpeedSlider.value = uiWheelSpeedInput.Value;
        RefreshWheelSpeedValue();

        _preventEvent = false;
    }

    public void OnWheelSpeedStop()
    {
        if (_preventEvent)
            return;

        _preventEvent = true;

        _wheelSpeed.value = _wheelSpeed.minValue;
        uiWheelSpeedSlider.value = 0f;
        uiWheelSpeedInput.Value = 0f;
        RefreshWheelSpeedValue();

        _preventEvent = false;
    }
    #endregion
}
