using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;


public class CommProductUI : MonoBehaviour
{
    public CommProduct prefab;
    public Text uiModelInfo;
    public Image uiModelImage;
    public ListView uiControlTable;
    public ListItem uiControlItem;
    public ControlUIManager uiManager;
    public Button uiFind;
    public ListView uiProductList;
    public ListItem uiProductItem;
    public GameObject uiMessageRoot;
    public GameObject uiMessageSearching;
    public Text uiSearchingStatus;

    private CommProduct _product;
    private bool _findChildProduct = false;
    private bool _cancelFind = false;

    void Awake()
    {
        if (uiControlTable != null)
            uiControlTable.OnChangedSelection.AddListener(SelectControlUI);

        uiFind.onClick.AddListener(OnFind);
        uiProductList.OnChangedSelection.AddListener(OnChangedSelectedProduct);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SetFindChildProduct(bool enable)
    {
        _findChildProduct = enable;
    }

    public void SetFindEnable(bool enable)
    {
        uiFind.interactable = enable;
    }

    public void ClearProduct()
    {
        foreach (ListItem item in uiProductList.items)
        {
            CommProduct product = (CommProduct)item.data;
            product.Disconnect();
        }

        OnChangedSelectedProduct();
    }

    public void CancelFind()
    {
        _cancelFind = true;
    }

    private void AddProduct(int id)
    {
        CommProduct product = Instantiate(prefab);
        product.transform.parent = transform;
        product.transform.localPosition = Vector3.zero;
        product.OnConnected.AddListener(OnConnected);
        product.OnConnectionFailed.AddListener(OnConnectionFailed);
        product.OnDisconnected.AddListener(OnDisconnected);
        product.OnLostConnection.AddListener(OnDisconnected);
        product.Connect(id);
    }

    private void RemoveProduct(CommProduct product)
    {
        if (product == null)
            return;

        foreach (ListItem item in uiProductList.items)
        {
            if(item.data.Equals(product))
            {
                uiProductList.RemoveItem(item);                
                DestroyImmediate(product.gameObject);
                break;
            }
        }
    }

    private void OnChangedSelectedProduct()
    {
        if (_product != null)
        {
            _product.Stop();
            _product = null;
        }           

        ListItem selectedItem = uiProductList.selectedItem;
        if (selectedItem != null)
        {
            _product = (CommProduct)selectedItem.data;
            _product.Run();
            if (_product.productInfo != null)
            {
                uiModelImage.sprite = _product.productInfo.image;

                StringBuilder info = new StringBuilder();
                info.AppendLine(string.Format("-Key: {0}", _product.productInfo.key));
                info.AppendLine(string.Format("-Type: {0}", _product.productInfo.type));
                info.AppendLine(string.Format("-Model: {0:d}", _product.model));
                info.AppendLine(string.Format("-Version: {0:d}", _product.version));
                info.AppendLine(string.Format("-Protocol: {0:f}", _product.productInfo.protocol.ToString()));
                if (_product.productInfo.firmware == null)
                    info.AppendLine("-Not support firmware");
                else
                    info.AppendLine(string.Format("-Firmware: v{0:f}, addr({1:x})", _product.productInfo.firmwareVersion, _product.productInfo.firmwareAddress));

                if (_product.productInfo.calibration == null)
                    info.AppendLine("-Not support calibration");
                else
                    info.AppendLine(string.Format("-Calibration: v{0:f}", _product.productInfo.calibrationVersion));
                uiModelInfo.text = info.ToString();

                uiManager.commProduct = _product;

                uiControlTable.ClearItem();
                uiManager.selectedUI = null;

                ControlUIInfo[] table = _product.productInfo.uiList;
                for (int i = 0; i < table.Length; i++)
                {
                    for (int j = 0; j < uiManager.uiList.Length; j++)
                    {
                        if (table[i].uiClass.Equals(uiManager.uiList[j].uiClass))
                        {
                            ListItem item = Instantiate(uiControlItem);
                            item.image.sprite = table[i].icon;
                            item.textList[0].text = table[i].name;
                            item.data = table[i];

                            uiControlTable.AddItem(item);
                            break;
                        }
                    }
                }
            }
            else
            {
                uiModelImage.sprite = null;
                uiModelInfo.text = "Unknown";
                uiManager.commProduct = null;
                uiControlTable.ClearItem();
                uiManager.selectedUI = null;
            }            
        }
        else
        {
            uiModelImage.sprite = null;
            uiModelInfo.text = "";
            uiManager.commProduct = null;
            uiControlTable.ClearItem();
            uiManager.selectedUI = null;
        }
    }

    private void OnFind()
    {
        ClearProduct();
        _cancelFind = false;
        AddProduct(CommProtocol.CM_ID);
        uiMessageRoot.SetActive(true);
        uiMessageSearching.SetActive(true);
    }

    private void OnConnected(CommProduct product)
    {
        product.gameObject.name = string.Format("[ID:{0:d}]", product.id);        

        if (product.productInfo != null)
            product.gameObject.name += product.productInfo.name;
        else
            product.gameObject.name += "Unknown";        

        ListItem item = Instantiate(uiProductItem);
        item.textList[0].text = product.gameObject.name;
        item.data = product;
        uiProductList.AddItem(item);

        if (_findChildProduct && !_cancelFind && (product.id < (CommProtocol.MAX_ID - 1)))
        {
            uiSearchingStatus.text = string.Format("ID: {0:d}", product.id);
            if (product.id == CommProtocol.CM_ID)
                AddProduct(0);
            else if (product.id == (CommProtocol.CM_ID - 1))
                AddProduct(CommProtocol.CM_ID + 1);
            else
                AddProduct(product.id + 1);
        }
        else
        {
            uiMessageRoot.SetActive(false);
            uiMessageSearching.SetActive(false);
        }
    }

    private void OnConnectionFailed(CommProduct product)
    {
        if(_findChildProduct && !_cancelFind && (product.id < (CommProtocol.MAX_ID - 1)))
        {
            uiSearchingStatus.text = string.Format("ID: {0:d}", product.id);
            if (product.id == CommProtocol.CM_ID)
                product.Connect(0);
            else if(product.id == (CommProtocol.CM_ID - 1))
                product.Connect(CommProtocol.CM_ID + 1);
            else
                product.Connect(product.id + 1);
        }
        else
        {
            DestroyImmediate(product.gameObject);
            uiMessageRoot.SetActive(false);
            uiMessageSearching.SetActive(false);
        }
    }

    private void OnDisconnected(CommProduct product)
    {
        RemoveProduct(product);
    }

    private void SelectControlUI()
    {
        ListItem selectedControl = uiControlTable.selectedItem;
        if (selectedControl == null)
            return;

        uiManager.selectedUI = (ControlUIInfo)selectedControl.data;
    }
}
