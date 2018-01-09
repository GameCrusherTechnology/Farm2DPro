using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class LanController {
	static string localLanUrl = "xml/Lan/EN";
	static Dictionary<string,string> languages = new Dictionary<string, string>();
	//加载
	public static void ReadAndLoadXml()  
	{  
		XmlDocument xmlDoc = new XmlDocument();  
		Debug.Log("加载lanxml文档"); 
		xmlDoc.LoadXml (Resources.Load (localLanUrl).ToString());

		XmlNode languageNode = xmlDoc.SelectSingleNode("language");  
		foreach (XmlElement node in languageNode)  
		{  
			languages.Add (node.GetAttribute ("key"), node.GetAttribute ("value"));
		}

	}

	public static string getString(string key){
		if (languages.ContainsKey (key)) {
			return languages [key];
		} else {
			Debug.Log ("no language :" + key);
			return key;
		}
	}

}
