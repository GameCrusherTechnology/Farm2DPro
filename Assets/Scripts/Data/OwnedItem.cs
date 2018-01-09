using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OwnedItem
{
	public string item_id;
	public int count;

	public void init(string id,int c){
		item_id = id;
		count = c;
	}

}

