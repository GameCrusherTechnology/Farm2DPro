using UnityEngine;
using System.Collections;

public class GardenData
{
	public int data_id;
	public string item_id = "50000";
	public int bound_x ;
	public int bound_y ;
	public int bound_m ;
	public int bound_n ;

	public int pos_x;
	public int pos_y;

	public string crop_id ;
	public string last_id = "10002";
	public int plantTime;

	public string floorId;

	public void creatGarden(int _id, string _item_id,int _posx,int _posy){
		data_id = _id;
		item_id = _item_id;
		pos_x = _posx;
		pos_y = _posy;

		bound_x = bound_y = -1;
		bound_m = bound_n = 1;
		floorId = gardenItemSpec().floorId;
	}

	CropItemSpec _cropitemSpec;
	public CropItemSpec cropItemSpec(){
		if(crop_id!=null && _cropitemSpec == null){
			_cropitemSpec = SpecController.getItemById (crop_id) as CropItemSpec;
		}
		return _cropitemSpec;
	}

	public void setCrop(string cropId){
		crop_id = cropId;
		_cropitemSpec = SpecController.getItemById (crop_id) as CropItemSpec;
	}

	BuildingItemSpec _itemSpec;
	public BuildingItemSpec gardenItemSpec(){
		if(item_id!=null && _itemSpec == null){
			_itemSpec = SpecController.getItemById (item_id) as BuildingItemSpec;
		}
		return _itemSpec;
	}

	DecorationItemSpec _floorSpec;
	public DecorationItemSpec floorItemSpec(){
		if(floorId!=null && _floorSpec == null){
			_floorSpec = SpecController.getItemById (floorId) as DecorationItemSpec;
		}
		return _floorSpec;
	}
	public void changetFloor(DecorationItemSpec spec){
		floorId = spec.item_id;
		_floorSpec = spec;
	}

	public int cropCount(){
		return (bound_m - bound_x +1) * (bound_n - bound_y+1);
	}

}

