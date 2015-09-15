using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class ProductUITest : MonoBehaviour
{
	public ProductManager productManager;
	public ListView uiModelList;
	public ListItem uiModelItem;
	public Text uiModelInfo;
	public Image uiModelImage;
	public ListView uiControlTable;
	public ListItem uiControlItem;
	public ControlUIManager uiManager;


	// Use this for initialization
	void Start ()
	{
		uiModelList.ClearItem();

		ProductInfo[] productList = productManager.productList;
		for(int i=0; i<productList.Length; i++)
		{
			ListItem item = GameObject.Instantiate(uiModelItem);
			item.textList[0].text = productList[i].name;
			item.data = productList[i];
			uiModelList.AddItem(item);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SelectModelUI()
	{
		ListItem selectedModel = uiModelList.selectedItem;
		ProductInfo product = (ProductInfo)selectedModel.data;

		uiModelImage.sprite = product.image;

		StringBuilder info = new StringBuilder();
		info.AppendLine(string.Format("-Key: {0}", product.key));
		info.AppendLine(string.Format("-Type: {0}", product.type));
		info.AppendLine(string.Format("-Model: {0:d}", product.model));
		info.AppendLine(string.Format("-Protocol: v{0:f}", product.protocolVersion));
		if(product.firmware == null)
			info.AppendLine("-Not support firmware");
		else
            info.AppendLine(string.Format("-Firmware: v{0:f}, addr({1:x})", product.firmwareVersion, product.firmwareAddress));

        if (product.calibration == null)
			info.AppendLine("-Not support calibration");
		else
			info.AppendLine(string.Format("-Calibration: v{0:f}", product.calibrationVersion));
		uiModelInfo.text = info.ToString();

		uiControlTable.ClearItem();
		uiManager.selectedUI = null;

		ControlUIInfo[] table = product.uiList;
		for(int i=0; i<table.Length; i++)
		{
			ListItem item = GameObject.Instantiate(uiControlItem);
			item.image.sprite = table[i].icon;
			item.textList[0].text = table[i].name;
			item.data = table[i];

			uiControlTable.AddItem(item);
		}
	}

	public void SelectControlUI()
	{
		ListItem selectedControl = uiControlTable.selectedItem;
		if(selectedControl == null)
			return;

		uiManager.selectedUI = (ControlUIInfo)selectedControl.data;
	}
}
