using UnityEngine;
using System.Collections.Generic;
using LitJson;

public class PlayerData
{
	public string gameUid;
	public string name ;
	public string charactor ;
	public int exp = 10000000;
	public int coin = 10000;
	public int gem = 10000;
	public string fenceId = "42000";

	public int lastTime;

	public int level{
		get{ 
			return Mathf.FloorToInt( Mathf.Sqrt (exp/10));
		}
	}
	//pet
	public List<PetData> pets = new List<PetData>();


	public string gardensJson;
	Dictionary<string,GardenData> _Gardens;
	public Dictionary<string,GardenData> Gardens {
		get{ 
			if(_Gardens == null){
				if (gardensJson != null) {
					_Gardens = JsonUtility.FromJson<Dictionary<string,GardenData>> (gardensJson);
				} else {
					_Gardens= new Dictionary<string, GardenData>();
				}
			}
			return _Gardens;
		}
		set{
			_Gardens = value;
		}
	} 

	public void upDataGarden(GardenData gdata){
		if (Gardens.ContainsKey (gdata.data_id.ToString())) {
			Gardens [gdata.data_id.ToString()] = gdata;
		} else {
			Gardens.Add (gdata.data_id.ToString(), gdata);
		}
	}
	public int getGardenCount(string itemId){
		int c = 0;
		foreach(string id in Gardens.Keys){
			if(Gardens[id].item_id == itemId){
				c++;
			}
		}
		return c;
	}



	public List<BuildingData> Buildings = new List<BuildingData>();

	public List<TreeData> Trees = new List<TreeData>();
	public int getTreeBoardCount(string itemId){
		int c = 0;
		foreach(TreeData data in Trees){
			if(data.item_id == itemId){
				c++;
			}
		}
		return c;
	}
	public void removeTree(int data_id){
		foreach(TreeData data in Trees){
			if(data.data_id == data_id){
				Trees.Remove (data);
				break;
			}
		}
	}



	public int sceneExpand;

	public int MaxSceneLength{
		get{ 
			return 10 + sceneExpand;
		}
	}


	public void removeBuildings(int data_id){
		foreach(BuildingData data in Buildings){
			if(data.data_id == data_id){
				Buildings.Remove (data);
				break;
			}
		}
	}

	public Dictionary<string,OwnedItem> ownedItems = new Dictionary<string, OwnedItem>();
	public void addOwnedItem(OwnedItem item){
		if (ownedItems.ContainsKey (item.item_id)) {
			ownedItems [item.item_id].count += item.count;
		} else {
			ownedItems.Add (item.item_id,item);
		}
	}

	public OwnedItem getOwnedItem(string id){
		if (ownedItems.ContainsKey (id)) {
			return ownedItems [id];
		} else {
			OwnedItem item = new OwnedItem ();
			item.init (id,0);
			return item;
		}
	}
	public void reduceOwnedItem(string id,int count){
		if (ownedItems.ContainsKey (id)) {
			ownedItems [id].count -= count;
		}
	}




	DecorationItemSpec _fenceSpec;
	public DecorationItemSpec fenceItemSpec{
		get{ 
			if(fenceId!=null && _fenceSpec == null){
				_fenceSpec = SpecController.getItemById (fenceId) as DecorationItemSpec;
			}
			return _fenceSpec;
		}
	}
	public void changetFence(DecorationItemSpec spec){
		fenceId = spec.item_id;
		_fenceSpec = spec;
	}
		


	public string charactorIconAddress{
		get{
			return "Texture/character/" + charactor + "Icon0001";
		}
	}
}

