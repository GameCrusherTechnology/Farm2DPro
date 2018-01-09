using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardTile : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
	string entityType ;
	int entityId;
	Dictionary<int,Dictionary<int, SpriteRenderer>> tileDic = new Dictionary<int, Dictionary<int, SpriteRenderer>> ();
	public void init (int x,int y,int w,int h,int _entityID,string _entityType){
		GameObject tile = transform.FindChild ("Tile").gameObject;
		entityId = _entityID;
		entityType = _entityType;
		tileDic = new Dictionary<int, Dictionary<int, SpriteRenderer>> (); 
		for(int i=x;i<w;i++){
			Dictionary<int, SpriteRenderer> dicY = new Dictionary<int, SpriteRenderer> ();
			for(int j=y;j<h;j++){
				GameObject tile1 = (GameObject)Instantiate(tile,DataManager.TileCenterToWorldPos(i,j),Quaternion.identity,transform);
				dicY.Add (j,tile1.GetComponent<SpriteRenderer>());
			}
			tileDic.Add (i, dicY);
		}
		tile.SetActive (false);
		transform.localPosition = Vector3.zero;
	}

	public bool  checkTiles(Vector2 tilePos){
		bool isAvali = true;
		foreach(int x in tileDic.Keys){
			Dictionary<int, SpriteRenderer> dicY = tileDic [x];
			foreach (int y in dicY.Keys) {
				SpriteRenderer renderer = dicY [y];
				Tile curTile = DataManager.getTile ((int)tilePos.x + x,(int)tilePos.y+y);
				if (curTile != null && isAvaliable (curTile)) {
					renderer.color = Color.white;
				} else {
					isAvali = false;
					renderer.color = Color.red;
				}
			}
		}
		return isAvali;
	}

	bool isAvaliable(Tile t){
		if (t.entityDic.Count == 0 ) {
			return true;
		} else {
			if(t.entityDic.ContainsKey(entityType) && t.entityDic[entityType] == entityId){
				return true;
			}
			//判断是否可以叠加

			return false;
		}
	}
}

