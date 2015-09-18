using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class DriveUI : ControlUI
{
    public Toggle uiJointMode;
    public Toggle uiWheelMode;
    public RectTransform uiJointView;
    public RectTransform uiWheelView;

    private bool _preventEvent;

    protected override void OnUpdateUIInfo()
    {
        ControlUIInfo info = uiInfo;

        _preventEvent = true;

        _preventEvent = false;
    }

    public void OnChangedMode()
    {
        if (_preventEvent)
            return;

        uiJointView.gameObject.SetActive(uiJointMode.isOn);
        uiWheelView.gameObject.SetActive(uiWheelMode.isOn);
    }
}
