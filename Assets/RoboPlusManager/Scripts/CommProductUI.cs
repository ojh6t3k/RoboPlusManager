using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;


[RequireComponent(typeof(CommProduct))]
public class CommProductUI : MonoBehaviour
{
    public Text uiModelInfo;
    public Image uiModelImage;
    public ListView uiControlTable;
    public ListItem uiControlItem;
    public ControlUIManager uiManager;

    private CommProduct _product;

    void Awake()
    {
        _product = GetComponent<CommProduct>();
        _product.OnConnected.AddListener(OnConnected);
        _product.OnDisconnected.AddListener(OnDisconnected);
        _product.OnLostConnection.AddListener(OnDisconnected);

        if (uiControlTable != null)
            uiControlTable.OnChangedSelection.AddListener(SelectControlUI);

        if (uiManager != null)
            uiManager.commProduct = _product;
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    private void OnConnected()
    {
        if (_product.productInfo == null)
            return;

        if(uiModelImage != null)
            uiModelImage.sprite = _product.productInfo.image;

        if (uiModelInfo != null)
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine(string.Format("-Key: {0}", _product.productInfo.key));
            info.AppendLine(string.Format("-Type: {0}", _product.productInfo.type));
            info.AppendLine(string.Format("-Model: {0:d}", _product.model));
            info.AppendLine(string.Format("-Version: {0:d}", _product.version));
            info.AppendLine(string.Format("-Protocol: v{0:f}", _product.productInfo.protocolVersion));
            if (_product.productInfo.firmware == null)
                info.AppendLine("-Not support firmware");
            else
                info.AppendLine(string.Format("-Firmware: v{0:f}, addr({1:x})", _product.productInfo.firmwareVersion, _product.productInfo.firmwareAddress));

            if (_product.productInfo.calibration == null)
                info.AppendLine("-Not support calibration");
            else
                info.AppendLine(string.Format("-Calibration: v{0:f}", _product.productInfo.calibrationVersion));
            uiModelInfo.text = info.ToString();
        }

        if(uiControlTable != null && uiManager != null)
        {
            uiControlTable.ClearItem();
            uiManager.selectedUI = null;

            ControlUIInfo[] table = _product.productInfo.uiList;
            for (int i = 0; i < table.Length; i++)
            {
                for(int j=0; j<uiManager.uiList.Length; j++)
                {
                    if(table[i].uiClass.Equals(uiManager.uiList[j].uiClass))
                    {
                        ListItem item = GameObject.Instantiate(uiControlItem);
                        item.image.sprite = table[i].icon;
                        item.textList[0].text = table[i].name;
                        item.data = table[i];

                        uiControlTable.AddItem(item);
                        break;
                    }
                }                
            }
        }        
    }

    private void OnDisconnected()
    {

    }

    private void SelectControlUI()
    {
        ListItem selectedControl = uiControlTable.selectedItem;
        if (selectedControl == null)
            return;

        uiManager.selectedUI = (ControlUIInfo)selectedControl.data;
    }
}
