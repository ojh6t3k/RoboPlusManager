using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;


[Serializable]
public class ControlItemInfo
{
    public enum ACCESS
    {
        R,
        RW,
        W
    }

	public string name;
	public int address;
	public ACCESS access;
	public bool savable;
	public int bytes;
	public int defaultValue;
    public int minValue;
    public int maxValue;
    public int value;

	public void Reset()
	{
		value = defaultValue;
	}
}

[Serializable]
public class ControlUIInfo
{
	public string name;
	public Sprite icon;
	public string uiClass;
    public int version;
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
    public int firmwareAddress;
	public TextAsset calibration;
	public float calibrationVersion;
	public float protocolVersion;
	public ControlUIInfo[] uiList;

    public ControlUIInfo GetControlUIInfo(string uiClass)
    {
        foreach (ControlUIInfo uiInfo in uiList)
        {
            if (uiInfo.uiClass.Equals(uiClass) == true)
                return uiInfo;
        }

        return null;
    }
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

    public ProductInfo GetProductInfo(int model)
    {
        foreach(ProductInfo info in productList)
        {
            if (info.model == model)
                return info;
        }

        return null;
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
                {
                    xmlEle = (XmlElement)xml.SelectSingleNode("/Product/Firmware");
                    product.firmwareVersion = float.Parse(xmlEle.Attributes["version"].Value);
                    try
                    {
                        product.firmwareAddress = Convert.ToInt32(xmlEle.Attributes["address"].Value, 16);
                    }
                    catch (Exception)
                    {
                    }
                }

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
                    ui.version = int.Parse(xmlNodes[j].Attributes["version"].Value);
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
                        List<int> continueList = new List<int>();
                        try
                        {
                            string[] tokens = xmlNodes2[k].Attributes["continue"].Value.Split(new char[] { ',' });
                            for(int x=0; x<tokens.Length; x++)
                            {
                                string[] tokens2 = tokens[x].Split(new char[] { '~' });
                                int min = int.Parse(tokens2[0]);
                                int max = min;
                                if (tokens2.Length > 1)
                                    max = int.Parse(tokens2[1]);
                                for (int y = min; y <= max; y++)
                                    continueList.Add(y);
                            }
                        }
                        catch(Exception)
                        {
                            continueList.Add(0);
                        }

                        for (int l=0; l< continueList.Count; l++)
                        {
                            ControlItemInfo item = new ControlItemInfo();
                            item.name = xmlNodes2[k].Attributes["name"].Value;
                            item.address = int.Parse(xmlNodes2[k].Attributes["address"].Value);
                            stringValue = xmlNodes2[k].Attributes["access"].Value;
                            if (stringValue.Equals("r") == true)
                                item.access = ControlItemInfo.ACCESS.R;
                            else if (stringValue.Equals("w") == true)
                                item.access = ControlItemInfo.ACCESS.W;
                            else if (stringValue.Equals("rw") == true)
                                item.access = ControlItemInfo.ACCESS.RW;
                            item.savable = bool.Parse(xmlNodes2[k].Attributes["save"].Value);
                            item.bytes = int.Parse(xmlNodes2[k].Attributes["byte"].Value);
                            try
                            {
                                item.defaultValue = int.Parse(xmlNodes2[k].Attributes["default"].Value);
                            }
                            catch (Exception)
                            {
                                item.defaultValue = 0;
                            }
                            try
                            {
                                string[] tokens = xmlNodes2[k].Attributes["range"].Value.Split(new char[] { '~' });
                                item.minValue = int.Parse(tokens[0]);
                                item.maxValue = int.Parse(tokens[1]);
                            }
                            catch (Exception)
                            {
                                item.minValue = 0;
                                item.maxValue = 256;
                                for (int n = 1; n < item.bytes; n++)
                                    item.maxValue *= 256;
                                item.maxValue--;
                            }

                            item.Reset();

                            if(continueList.Count > 1)
                                item.name += string.Format("{0:d}", continueList[l]);
                            item.address += (continueList[l] * item.bytes);

                            items.Add(item);
                        }                 
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
