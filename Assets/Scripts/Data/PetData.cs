using UnityEngine;
using System.Collections;

public class PetData
{
	public string item_id;

	public  void creatPet(string _item_id){
		item_id = _item_id;

	}

	PetItemSpec _petItemSpec;
	public PetItemSpec petItemSpec{
		get{
			if(_petItemSpec == null){
				_petItemSpec = SpecController.getItemById (item_id) as PetItemSpec;
			}
			return _petItemSpec;
		}
	}
}

