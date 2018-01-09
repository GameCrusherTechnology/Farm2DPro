using UnityEngine;
using System.Collections;

public class BuildingData
{
	public int pos_x;
	public int pos_y;

	public string item_id;
	public int data_id = 1000;
	public string gameuid;
	//翻转
	public bool isTurnOver;



	public void creatBuildingData(int _data_id, string _item_id,int _posx,int _posy){
		data_id = _data_id;
		item_id = _item_id;
		pos_x = _posx;
		pos_y = _posy;
	}
	public int getItemType(){
		return SpecController.getItemType (item_id);
	}

	DecorationItemSpec _spec;
	public DecorationItemSpec itemSpec{
		get{ 
			if(_spec == null){
				_spec = SpecController.getItemById (item_id) as DecorationItemSpec;
			}
			return _spec;
		}

	}

	virtual public int expand_x{
		get{ 
			return isTurnOver ? itemSpec.expand_x : itemSpec.expand_y;
		}
	}
	virtual public int expand_y{
		get{ 
			return isTurnOver ? itemSpec.expand_y : itemSpec.expand_x;
		}
	}
}

