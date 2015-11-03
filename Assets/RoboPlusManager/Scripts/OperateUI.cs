using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OperateUI : ControlUI
{
    public RectTransform v1_uiView;
    public RectTransform v2_uiView;
    public UpdownValue v2_uiSleepTimerInput;
    public Text v2_uiSleepTimerUnitValue;
    public Text v2_uiButtonCountValue;
    public RectTransform v3_uiView;
    public RectTransform v4_uiView;
    public Toggle v4_uiButton;
    public Toggle v4_uiLED;

    private ControlItemInfo _button;
    private ControlItemInfo _led;
    private ControlItemInfo _sleepTimer;
    private ControlItemInfo _buttonCount;

    private int version;
    private bool _preventEvent = false;
    private bool _initializing;

    #region Override
    protected override void OnInitialize()
    {
        v2_uiSleepTimerInput.OnChangedValue.AddListener(OnChangedSleepTimerInput);
        v4_uiLED.onValueChanged.AddListener(OnChangedLED);
    }

    protected override void OnSetUiInfo()
    {
        ControlUIInfo info = uiInfo;

        _preventEvent = true;
        _initializing = true;

        version = info.version;
        if (version == 1)
        {
            v1_uiView.gameObject.SetActive(true);
            v2_uiView.gameObject.SetActive(false);
            v3_uiView.gameObject.SetActive(false);
            v4_uiView.gameObject.SetActive(false);
        }
        else if(version == 2)
        {
            v1_uiView.gameObject.SetActive(false);
            v2_uiView.gameObject.SetActive(true);
            v3_uiView.gameObject.SetActive(false);
            v4_uiView.gameObject.SetActive(false);

            _buttonCount = info.GetUIItem("ButtonCount");
            _uiButtonCount = _buttonCount.value;

            _sleepTimer = info.GetUIItem("SleepTimer");
            _uiSleepTimer = _sleepTimer.value;
        }
        else if (version == 3)
        {
            v1_uiView.gameObject.SetActive(false);
            v2_uiView.gameObject.SetActive(false);
            v3_uiView.gameObject.SetActive(true);
            v4_uiView.gameObject.SetActive(false);
        }
        else if (version == 4)
        {
            v1_uiView.gameObject.SetActive(false);
            v2_uiView.gameObject.SetActive(false);
            v3_uiView.gameObject.SetActive(false);
            v4_uiView.gameObject.SetActive(true);

            _button = info.GetUIItem("Button");
            _uiButton = _button.value;

            _led = info.GetUIItem("LED");
            _uiLED = _led.value;
        }

        _preventEvent = false;
        _initializing = false;
    }

    protected override void OnSetCommProduct()
    {
        if (version == 1)
        {
        }
        else if (version == 2)
        {
            commProduct.AddReadItem(_sleepTimer);
            commProduct.AddReadItem(_buttonCount);
        }
        else if (version == 3)
        {
        }
        else if (version == 4)
        {
            commProduct.AddReadItem(_button);
            commProduct.AddReadItem(_led);
        }
    }

    protected override void OnUpdateUI()
    {
        _preventEvent = true;        

        if (version == 1)
        {
        }
        else if (version == 2)
        {
            if (_sleepTimer.update)
                _uiSleepTimer = _sleepTimer.value;

            if (_buttonCount.update)
                _uiButtonCount = _buttonCount.value;
        }
        else if (version == 3)
        {
        }
        else if (version == 4)
        {
            if (_button.update)
                _uiButton = _button.value;

            if (_led.update)
                _uiLED = _led.value;
        }

        _preventEvent = false;
    }

    protected override void OnWriteDone()
    {
    }
    #endregion

    #region Event
    private void OnChangedSleepTimerInput()
    {
        if (_preventEvent)
            return;

        if (version == 1)
        {
        }
        else if (version == 2)
        {
            _sleepTimer.writeValue = (int)v2_uiSleepTimerInput.Value;
        }
        else if (version == 3)
        {
        }
        else if (version == 4)
        {
        }

        commProduct.AddWriteItem(_sleepTimer);        
    }

    private void OnChangedLED(bool value)
    {
        if (_preventEvent)
            return;

        if (value == true)
            _led.writeValue = 1;
        else
            _led.writeValue = 0;

        commProduct.AddWriteItem(_led);
    }
    #endregion

    #region UI Control
    private int _uiSleepTimer
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;            

            if (version == 1)
            {
            }
            else if (version == 2)
            {
                if (_initializing)
                {
                    v2_uiSleepTimerInput.minValue = _sleepTimer.minValue;
                    v2_uiSleepTimerInput.maxValue = _sleepTimer.maxValue;
                    v2_uiSleepTimerInput.unitValue = 1f;
                    v2_uiSleepTimerInput.format = "f0";
                }

                v2_uiSleepTimerInput.Value = value;
                v2_uiSleepTimerUnitValue.text = string.Format("{0:d} min", value);
            }
            else if (version == 3)
            {
            }
            else if (version == 4)
            {
            }

            _preventEvent = backup;
        }
    }

    private int _uiButtonCount
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (version == 1)
            {
            }
            else if (version == 2)
            {
                v2_uiButtonCountValue.text = value.ToString();
            }
            else if (version == 3)
            {
            }
            else if (version == 4)
            {
            }

            _preventEvent = backup;
        }
    }

    private int _uiButton
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (version == 1)
            {
            }
            else if (version == 2)
            {
            }
            else if (version == 3)
            {
            }
            else if (version == 4)
            {
                if (value == 0)
                    v4_uiButton.isOn = false;
                else
                    v4_uiButton.isOn = true;
            }

            _preventEvent = backup;
        }
    }

    private int _uiLED
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (version == 1)
            {
            }
            else if (version == 2)
            {
            }
            else if (version == 3)
            {
            }
            else if (version == 4)
            {
                if (value == 0)
                    v4_uiLED.isOn = false;
                else
                    v4_uiLED.isOn = true;
            }

            _preventEvent = backup;
        }
    }
    #endregion
}
