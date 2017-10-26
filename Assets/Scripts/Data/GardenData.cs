using UnityEngine;
using System.Collections;

public class GardenData
{
	public int data_id;
	public string item_id;
	public int bound_x ;
	public int bound_y ;
	public int bound_w ;
	public int bound_h ;

	public int pos_x;
	public int pos_y;

	public string product_id;
	public int plantTime;

	public void creatGarden(string _item_id,int _posx,int _posy){
		item_id = _item_id;
		pos_x = _posx;
		pos_y = _posy;

		bound_x = bound_y = -2;
		bound_w = bound_h = 5;
	}
}

