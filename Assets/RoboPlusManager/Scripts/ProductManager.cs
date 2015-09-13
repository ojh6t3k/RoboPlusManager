﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine.UI;


[Serializable]
public class ControlItemInfo
{
	public string name;
	public int address;
	public bool accessRead;
	public bool accessWrite;
	public bool isROM;
	public int bytes;
	public int defaultValue;
}

[Serializable]
public class ControlUIInfo
{
	public string name;
	public Sprite icon;
	public string uiClass;
	public string[] uiParameters;
	public ControlItemInfo[] uiItems;

	public ControlItemInfo GetUIItem(string name)
	{
		foreach(ControlItemInfo item in uiItems)
		{
			if(item.name.Equals(name))
				return item;
		}

		return null;
	}
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
	public ControlUIInfo[] uiList;
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
				product.firmware = Resources.Load<TextAsset>("Product/Firmware/" + product.key);
				if(product.firmware != null)
					product.firmwareVersion = float.Parse(xmlEle.Attributes["version"].Value);

				xmlEle = (XmlElement)xml.SelectSingleNode("/Product/Calibration");
				if(xmlEle != null)
				{
					product.calibration = Resources.Load<TextAsset>("Product/Calibration/" + product.key);
					if(product.calibration != null)
						product.calibrationVersion = float.Parse(xmlEle.Attributes["version"].Value);
				}

				product.image = Resources.Load<Sprite>("Product/Image/" + product.key);

				xmlEle = (XmlElement)xml.SelectSingleNode("/Product/Control");
				product.protocolVersion = float.Parse(xmlEle.Attributes["protocol"].Value);

				List<ControlUIInfo> uis = new List<ControlUIInfo>();
				XmlNodeList xmlNodes = xmlEle.SelectNodes("./UI");
				for(int j=0; j<xmlNodes.Count; j++)
				{
					ControlUIInfo ui = new ControlUIInfo();
					string stringValue = xmlNodes[j].Attributes["icon"].Value;
					for(int n=0; n<icons.Length; n++)
					{
						if(icons[n].name.Equals(stringValue) == true)
						{
							ui.icon = icons[n];
							break;
						}
					}
					ui.name = xmlNodes[j].Attributes["name"].Value;
					ui.uiClass = xmlNodes[j].Attributes["class"].Value;
					try
					{
						ui.uiParameters = xmlNodes[j].Attributes["param"].Value.Split(new char[] { ',' });
					}
					catch(Exception)
					{
					}

					List<ControlItemInfo> items = new List<ControlItemInfo>();
					XmlNodeList xmlNodes2 = xmlNodes[j].SelectNodes("./Item");
					for(int k=0; k<xmlNodes2.Count; k++)
					{
						ControlItemInfo item = new ControlItemInfo();
						item.name = xmlNodes2[k].Attributes["name"].Value;
						item.address = int.Parse(xmlNodes2[k].Attributes["address"].Value);
						stringValue = xmlNodes2[k].Attributes["access"].Value;
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
						item.isROM = bool.Parse(xmlNodes2[k].Attributes["save"].Value);
						item.bytes = int.Parse(xmlNodes2[k].Attributes["byte"].Value);
						item.defaultValue = int.Parse(xmlNodes2[k].Attributes["default"].Value);

						items.Add(item);
					}
					ui.uiItems = items.ToArray();
					uis.Add(ui);
				}
				product.uiList = uis.ToArray();

				products.Add(product);
			}
			catch(Exception)
			{
			}
		}

		productList = products.ToArray();
	}
}