using UnityEngine;
using System.Collections;

public class ItemSpec
{
	public string item_id;
	public string name;

	public int coinPrice;
	public int gemPrice;
	public int maxCount = -1;
	public string type;
	public int level;


	public string itemName
	{
		get{ 
			return LanController.getString ("Item_" + name);
		}
	}
	public string itemInfo
	{
		get{ 
			return LanController.getString ("Item_" + name+"_Info");
		}
	}

	virtual public string iconAddress
	{
		get{ 
			return "Texture/icon/" + name +"Icon";
		}
	}

	public bool showInShop(){
		return coinPrice > 0 || gemPrice > 0;
	}
}

