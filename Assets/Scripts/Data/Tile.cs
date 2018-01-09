using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Tile 
{
	public int x;
	public int y;

	public int owned_id;

	public Tile(int _x,int _y){
		x = _x;
		y = _y;
	}
		

	public Dictionary<string,int> entityDic = new Dictionary<string, int>();

	public void addEntity(int id , string type){
		if (entityDic.ContainsKey (type)) {
			entityDic [type] = id;
		} else {
			entityDic.Add (type,id);
		}
	}

	public void removeEntity(int id , string type){
		if (entityDic.ContainsKey (type) && entityDic [type] == id) {
			entityDic.Remove (type);
		}
	}

	public OwnedItem getTopEntity(){
		OwnedItem item = new OwnedItem (); 
		foreach(string type in entityDic.Keys){
			if(type == DataManager.ENTITY_DECORATION || type == DataManager.ENTITY_GARDEN || type == DataManager.ENTITY_HOUSE|| type == DataManager.ENTITY_TREE){
				item.init (type,entityDic [type]);
				break;
			}
		}
		return item;
	}
}

