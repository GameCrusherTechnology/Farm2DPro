using UnityEngine;
using System.Collections;

public class TreasureData
{
	public string name ;
	public string type ="coin";//gem
	public int getCount;
	public float moneyCount;

	public TreasureData(string _name,string _type,int _getCount,float costCount){
		name = _name;
		type = _type;
		getCount = _getCount;
		moneyCount = costCount;
	}

	public string treasureMessage{
		get{
			return LanController.getString (name + "Info");
		}
	}

	public string TreasureIcon{
		get{ 
			return "Texture/icon/" + name;
		}
	}

	public string TreasureName{
		get{ 
			return LanController.getString (name);
		}
	}
}

