using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine.UI;


[Serializable]
public class UIInfo
{
	public string name;
	public int defaultValue;
	public int value;
	public float unit;
	public string[] parameters;
}

[Serializable]
public class ControlItemInfo
{
	public string name;
	public Sprite icon;
	public int address;
	public bool accessRead;
	public bool accessWrite;
	public bool isROM;
	public string type;
	public UIInfo ui;
}

[Serializable]
public class ProductInfo
{
	public string name;
	public string key;
	public string type;
	public int model;
	public Sprite image;
	public TextAsset firmware;
	public float firmwareVersion;
	public TextAsset calibration;
	public float calibrationVersion;
	public float protocolVersion;
	public ControlItemInfo[] controlTable;
}

public class ProductManager : MonoBehaviour
{
	public ProductInfo[] productList;

	void Awake()
	{
		Load();
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private void Load()
	{
		Sprite[] icons = Resources.LoadAll<Sprite>("Product/Icon");

		List<ProductInfo> products = new List<ProductInfo>();

		UnityEngine.Object[] list = Resources.LoadAll("Product", typeof(TextAsset));
		for(int i=0; i<list.Length; i++)
		{
			TextAsset manifest = (TextAsset)list[i];
			try
			{
				XmlDocument xml = new XmlDocument();
				xml.LoadXml(manifest.text);

				XmlElement xmlEle = (XmlElement)xml.SelectSingleNode("/Product");
				ProductInfo product = new ProductInfo();
				product.key = xmlEle.Attributes["key"].Value;
				product.name = xmlEle.Attributes["name"].Value;
				product.type = xmlEle.Attributes["type"].Value;
				try
				{
					product.model = int.Parse(xmlEle.Attributes["model"].Value);
				}
				catch(Exception)
				{
				}
				product.image = Resources.Load<Sprite>("Product/Image/" + product.key);

				xmlEle = (XmlElement)xml.SelectSingleNode("/Product/Firmware");
				product.firmware = Resources.Load<TextAsset>("Product/Firmware/" + product.key);
				product.firmwareVersion = float.Parse(xmlEle.Attributes["version"].Value);

				xmlEle = (XmlElement)xml.SelectSingleNode("/Product/Protocol");
				product.protocolVersion = float.Parse(xmlEle.Attributes["version"].Value);

				xmlEle = (XmlElement)xml.SelectSingleNode("/Product/Calibration");
				if(xmlEle != null)
				{
					product.calibration = Resources.Load<TextAsset>("Product/Calibration/" + product.key);
					product.calibrationVersion = float.Parse(xmlEle.Attributes["version"].Value);
				}

				xmlEle = (XmlElement)xml.SelectSingleNode("/Product/ControlTable");

				List<ControlItemInfo> items = new List<ControlItemInfo>();
				XmlNodeList xmlNodes = xmlEle.SelectNodes("/Product/ControlTable/Item");
				for(int j=0; j<xmlNodes.Count; j++)
				{
					ControlItemInfo item = new ControlItemInfo();
					string stringValue = xmlNodes[j].Attributes["icon"].Value;
					for(int n=0; n<icons.Length; n++)
					{
						if(icons[n].name.Equals(stringValue) == true)
						{
							item.icon = icons[n];
							break;
						}
					}
					item.name = xmlNodes[j].Attributes["name"].Value;

					item.address = int.Parse(xmlNodes[j].Attributes["address"].Value);
					stringValue = xmlNodes[j].Attributes["access"].Value;
					if(stringValue.Equals("r") == true)
					{
						item.accessRead = true;
						item.accessWrite = false;
					}
					else if(stringValue.Equals("w") == true)
					{
						item.accessRead = false;
						item.accessWrite = true;
					}
					else if(stringValue.Equals("rw") == true)
					{
						item.accessRead = true;
						item.accessWrite = true;
					}
					else
					{
						item.accessRead = false;
						item.accessWrite = false;
					}
					item.isROM = bool.Parse(xmlNodes[j].Attributes["rom"].Value);
					item.type = xmlNodes[j].Attributes["type"].Value;
					UIInfo uiInfo = new UIInfo();
					uiInfo.name = xmlNodes[j].Attributes["ui"].Value;
					uiInfo.defaultValue = int.Parse(xmlNodes[j].Attributes["default"].Value);
					uiInfo.parameters = xmlNodes[j].Attributes["param"].Value.Split(new char[] { ',' });
					uiInfo.unit = float.Parse(xmlNodes[j].Attributes["unit"].Value);
					item.ui = uiInfo;

					items.Add(item);
				}
				product.controlTable = items.ToArray();

				products.Add(product);
			}
			catch(Exception e)
			{
			//	Debug.Log(e);
			}
		}

		productList = products.ToArray();
	}
}
