using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ControlUI : MonoBehaviour
{
	private ControlUIInfo _uiInfo;
	private bool _active = false;
    private CommProduct _commProduct = null;

    protected virtual void OnInitialize() { }
    protected virtual void OnSetCommProduct() { }
	protected virtual void OnSetUiInfo() { }
    protected virtual void OnUpdateUI() { }
    protected virtual void OnWriteDone() { }

    void Awake()
	{
		active = _active;

        OnInitialize();
    }

	public string uiClass
	{
		get
		{
			return this.GetType().Name;
		}
	}

	public bool active
	{
		get
		{
			return _active;
		}
		set
		{
			_active = value;
			this.gameObject.SetActive(_active);
        }
	}

    public CommProduct commProduct
    {
        get
        {
            return _commProduct;
        }
        set
        {
            if(_commProduct != null)
            {
                _commProduct.ClearItem();
                _commProduct.OnReadUpdate.RemoveListener(OnUpdateUI);
                _commProduct.OnWriteCompleted.RemoveListener(OnWriteDone);
            }

            _commProduct = value;
            if (_commProduct != null)
            {
                OnSetCommProduct();
                _commProduct.OnReadUpdate.AddListener(OnUpdateUI);
                _commProduct.OnWriteCompleted.AddListener(OnWriteDone);
            }
        }
    }

	public ControlUIInfo uiInfo
	{
		get
		{
			return _uiInfo;
		}
		set
		{
			_uiInfo = value;
            if(_uiInfo != null)
                OnSetUiInfo();
		}
	}

    #region Convert
    protected int CenterValue(int minValue, int maxValue)
    {
        return (int)((maxValue - minValue + 1) / 2f);
    }

    protected int MaxSignValue(int minValue, int maxValue)
    {
        return CenterValue(minValue, maxValue) - 1;
    }

    protected float Value2Unit(int value, int minValue, int maxValue, float minUnit, float maxUnit)
    {        
        float ratio = Mathf.Abs((maxUnit - minUnit) / (maxValue - minValue + 1));
        return value * ratio + minUnit;
    }

    protected int Unit2Value(float unit, float minUnit, float maxUnit, int minValue, int maxValue)
    {
        float ratio = Mathf.Abs((maxValue - minValue + 1) / (maxUnit - minUnit));
        return Mathf.RoundToInt((unit - minUnit) * ratio);
    }

    protected int Value2SignValue(int value, int minValue, int maxValue)
    {
        int center = CenterValue(minValue, maxValue);

        if (value >= center)
            return -(value - center);
        else
            return value;
    }

    protected int SignValue2Value(int signValue, int minValue, int maxValue)
    {
        int center = CenterValue(minValue, maxValue);

        if (signValue < 0)
            return -signValue + center;
        else
            return signValue;
    }

    protected float Value2SignUnit(int value, int minValue, int maxValue, float minUnit, float maxUnit)
    {
        int signValue = Value2SignValue(value, minValue, maxValue);
        float unitValue = Value2Unit(Mathf.Abs(signValue), minValue, MaxSignValue(minValue, maxValue), minUnit, maxUnit);
        if (signValue < 0)
            unitValue = -unitValue;

        return unitValue;
    }
    #endregion
}
