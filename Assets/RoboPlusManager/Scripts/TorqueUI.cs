using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TorqueUI : ControlUI
{
    public UpdownValue uiMaxTorqueInput;
    public Slider uiMaxTorqueSlider;
    public Text uiMaxTorqueUnitValue;
    public RectTransform uiTorqueLimitView;
    public UpdownValue uiTorqueLimitInput;
    public Slider uiTorqueLimitSlider;
    public Text uiTorqueLimitUnitValue;
    public UpdownValue uiPunchInput;
    public Slider uiPunchSlider;
    public Text uiPunchUnitValue;
    public Toggle uiTorqueEnable;
    public Button uiSave;

    private ControlItemInfo _maxTorque;
    private ControlItemInfo _torqueLimit;
    private ControlItemInfo _punch;
    private ControlItemInfo _torqueEnable;

    private bool _preventEvent = false;
    private bool _initializing;

    #region Override
    protected override void OnInitialize()
    {
        uiMaxTorqueInput.OnChangedValue.AddListener(OnChangedMaxTorqueInput);
        uiMaxTorqueSlider.onValueChanged.AddListener(OnChangedMaxTorqueSlider);
        uiTorqueLimitInput.OnChangedValue.AddListener(OnChangedTorqueLimitInput);
        uiTorqueLimitSlider.onValueChanged.AddListener(OnChangedTorqueLimitSlider);
        uiPunchInput.OnChangedValue.AddListener(OnChangedPunchInput);
        uiPunchSlider.onValueChanged.AddListener(OnChangedPunchSlider);
        uiTorqueEnable.onValueChanged.AddListener(OnChangedTorqueEnable);
        uiSave.onClick.AddListener(OnClickSave);
    }

    protected override void OnSetUiInfo()
    {
        ControlUIInfo info = uiInfo;

        _preventEvent = true;
        _initializing = true;

        _maxTorque = info.GetUIItem("MaxTorque");
        _punch = info.GetUIItem("Punch");
        _torqueEnable = info.GetUIItem("TorqueEnable");

        if (uiInfo.version == 1)
        {
            uiTorqueLimitView.gameObject.SetActive(true);
            _torqueLimit = info.GetUIItem("TorqueLimit");
            _uiTorqueLimit = _torqueLimit.value;
        }
        else
        {
            uiTorqueLimitView.gameObject.SetActive(false);
        }

        _uiMaxTorque = _maxTorque.value;
        _uiPunch = _punch.value;
        _uiTorqueEnable = _torqueEnable.value; 

        uiSave.interactable = false;

        _preventEvent = false;
        _initializing = false;
    }

    protected override void OnSetCommProduct()
    {
        commProduct.AddReadItem(_maxTorque);
        commProduct.AddReadItem(_punch);
        commProduct.AddReadItem(_torqueEnable);

        if (uiInfo.version == 1)
        {
            commProduct.AddReadItem(_torqueLimit);
        }
    }

    protected override void OnUpdateUI()
    {
        bool modify = false;
        _preventEvent = true;

        modify |= _maxTorque.modify;

        if (_maxTorque.update)
            _uiMaxTorque = _maxTorque.value;

        if (_punch.update)
            _uiPunch = _punch.value;

        if (_torqueEnable.update)
            _uiTorqueEnable = _torqueEnable.value;

        if (uiInfo.version == 1)
        {
            if (_torqueLimit.update)
                _uiTorqueLimit = _torqueLimit.value;
        }

        uiSave.interactable = modify;
        _preventEvent = false;
    }

    protected override void OnWriteDone()
    {
    }
    #endregion

    #region Event
    private void OnChangedMaxTorqueInput()
    {
        if (_preventEvent)
            return;

        _maxTorque.writeValue = (int)uiMaxTorqueInput.Value;
        _uiMaxTorque = _maxTorque.writeValue;
    }

    private void OnChangedMaxTorqueSlider(float value)
    {
        if (_preventEvent)
            return;

        _maxTorque.writeValue = (int)uiMaxTorqueSlider.value;
        _uiMaxTorque = _maxTorque.writeValue;
    }

    private void OnChangedTorqueLimitInput()
    {
        if (_preventEvent)
            return;

        _torqueLimit.writeValue = (int)uiTorqueLimitInput.Value;
        _uiTorqueLimit = _torqueLimit.writeValue;
        commProduct.AddWriteItem(_torqueLimit);
    }

    private void OnChangedTorqueLimitSlider(float value)
    {
        if (_preventEvent)
            return;

        _torqueLimit.writeValue = (int)uiTorqueLimitSlider.value;
        _uiTorqueLimit = _torqueLimit.writeValue;
        commProduct.SetWriteItem(_torqueLimit);
    }

    private void OnChangedPunchInput()
    {
        if (_preventEvent)
            return;

        _punch.writeValue = (int)uiPunchInput.Value;
        _uiPunch = _punch.writeValue;
        commProduct.AddWriteItem(_punch);
    }

    private void OnChangedPunchSlider(float value)
    {
        if (_preventEvent)
            return;

        _punch.writeValue = (int)uiPunchSlider.value;
        _uiPunch = _punch.writeValue;
        commProduct.SetWriteItem(_punch);
    }

    private void OnChangedTorqueEnable(bool value)
    {
        if (_preventEvent)
            return;

        if (uiTorqueEnable.isOn)
            _torqueEnable.writeValue = 1;
        else
            _torqueEnable.writeValue = 0;
        commProduct.AddWriteItem(_torqueEnable);
    }

    private void OnClickSave()
    {
        if (_preventEvent)
            return;

        commProduct.AddWriteItem(_maxTorque);
    }
    #endregion

    #region UI Control
    private int _uiMaxTorque
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (_initializing)
            {
                uiMaxTorqueSlider.minValue = _maxTorque.minValue;
                uiMaxTorqueSlider.maxValue = _maxTorque.maxValue;
                uiMaxTorqueSlider.wholeNumbers = true;

                uiMaxTorqueInput.minValue = _maxTorque.minValue;
                uiMaxTorqueInput.maxValue = _maxTorque.maxValue;
                uiMaxTorqueInput.unitValue = 1;
                uiMaxTorqueInput.format = "f0";
            }

            uiMaxTorqueSlider.value = value;
            uiMaxTorqueInput.Value = value;
            uiMaxTorqueUnitValue.text = string.Format("{0:d} ({1:f1}%)", value, Value2Unit(value, _maxTorque.minValue, _maxTorque.maxValue, 0f, 100f));

            _preventEvent = backup;
        }
    }

    private int _uiTorqueLimit
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (_initializing)
            {
                uiTorqueLimitSlider.minValue = _torqueLimit.minValue;
                uiTorqueLimitSlider.maxValue = _torqueLimit.maxValue;
                uiTorqueLimitSlider.wholeNumbers = true;

                uiTorqueLimitInput.minValue = _torqueLimit.minValue;
                uiTorqueLimitInput.maxValue = _torqueLimit.maxValue;
                uiTorqueLimitInput.unitValue = 1;
                uiTorqueLimitInput.format = "f0";
            }

            uiTorqueLimitSlider.value = value;
            uiTorqueLimitInput.Value = value;
            uiTorqueLimitUnitValue.text = string.Format("{0:d} ({1:f1}%)", value, Value2Unit(value, _torqueLimit.minValue, _torqueLimit.maxValue, 0f, 100f));

            _preventEvent = backup;
        }
    }

    private int _uiPunch
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (_initializing)
            {
                uiPunchSlider.minValue = _punch.minValue;
                uiPunchSlider.maxValue = _punch.maxValue;
                uiPunchSlider.wholeNumbers = true;

                uiPunchInput.minValue = _punch.minValue;
                uiPunchInput.maxValue = _punch.maxValue;
                uiPunchInput.unitValue = 1;
                uiPunchInput.format = "f0";
            }

            uiPunchSlider.value = value;
            uiPunchInput.Value = value;
            uiPunchUnitValue.text = string.Format("{0:d} ({1:f1}%)", value, Value2Unit(value, _punch.minValue, _punch.maxValue, 0f, 100f));

            _preventEvent = backup;
        }
    }

    private int _uiTorqueEnable
    {
        set
        {
            bool backup = _preventEvent;
            _preventEvent = true;

            if (value == 1)
                uiTorqueEnable.isOn = true;
            else
                uiTorqueEnable.isOn = false;

            _preventEvent = backup;
        }
    }

    #endregion
}
