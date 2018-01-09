using UnityEngine;
using System.Collections;

public class TreeData : BuildingData
{
	public string treeId;
	public int plantTime;

	public void creatTreeBoard(int _id, string _item_id,int _posx,int _posy){
		data_id = _id;
		item_id = _item_id;
		pos_x = _posx;
		pos_y = _posy;
	}


	override public int expand_x{
		get{ 
			return 2;
		}
	}
	override public int expand_y{
		get{ 
			return 2;
		}
	}

	CropItemSpec _treeitemSpec;
	public CropItemSpec treeItemSpec{
		get{ 
			if(treeId!=null && _treeitemSpec == null){
				_treeitemSpec = SpecController.getItemById (treeId) as CropItemSpec;
			}
			return _treeitemSpec;
		}
	}

	BuildingItemSpec _itemSpec;
	public BuildingItemSpec itemSpec{
		get{ 
			if(item_id!=null && _itemSpec == null){
				_itemSpec = SpecController.getItemById (item_id) as BuildingItemSpec;
			}
			return _itemSpec;
		}
	}

}

