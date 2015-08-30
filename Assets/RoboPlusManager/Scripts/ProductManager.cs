using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.IO;

[Serializable]
public class ControlItemInfo
{
	public string key;
	public string name;
	public Sprite icon;
	public int address;
	public bool accessRead;
	public bool accessWrite;
	public bool isROM;
	public int bytes;
	public int defaultValue;
	public int minValue;
	public int maxValue;
	public int value;
	public string uiType;
}

[Serializable]
public class ProductInfo
{
	public string key;
	public string name;
	public string type;
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
					item.key = xmlNodes[j].Attributes["key"].Value;
					item.name = xmlNodes[j].Attributes["name"].Value;
					for(int n=0; n<icons.Length; n++)
					{
						if(icons[n].name.Equals(item.key) == true)
						{
							item.icon = icons[n];
							break;
						}
					}
					item.address = int.Parse(xmlNodes[j].Attributes["address"].Value);
					string stringValue = xmlNodes[j].Attributes["access"].Value;
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
					stringValue = xmlNodes[j].Attributes["type"].Value;
					if(stringValue.Equals("rom") == true)
						item.isROM = true;
					else if(stringValue.Equals("ram") == true)
						item.isROM = false;
					item.bytes = int.Parse(xmlNodes[j].Attributes["bytes"].Value);
					item.defaultValue = int.Parse(xmlNodes[j].Attributes["default"].Value);
					string[] token = xmlNodes[j].Attributes["range"].Value.Split(new char[] { ',' });
					item.minValue = int.Parse(token[0]);
					item.maxValue = int.Parse(token[1]);
					item.uiType = xmlNodes[j].Attributes["ui"].Value;

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
