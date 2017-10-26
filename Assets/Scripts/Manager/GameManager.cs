using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager 
{
	public static string Server_Url = "http://192.168.1.101/NatureServer/test.php";

	public static PlayerData MyPlayData ;

	public static bool isMyFarm = true ; 
	public static bool isNewPlayer = false;


	//tile
	static Dictionary<int,Dictionary<int,Tile>> FarmTiles = new Dictionary<int, Dictionary<int, Tile>>();
	public static void InitTile(int x,int y,int m,int n){
		Dictionary<int,Dictionary<int,Tile>> tiles = new Dictionary<int, Dictionary<int, Tile>>();
		int a = x;
		while(a <= m){
			int b = y;
			Dictionary<int,Tile> group = new Dictionary<int, Tile> ();
			while (b <= n) {
				group[b] = new Tile(a,b);
				b++;
			}
			tiles[a] = group;
			a++;
		}
		FarmTiles = tiles;
	}

	public static Tile getTile(int _x,int _y){
		if(!FarmTiles.ContainsKey(_x)){
			return null;
		}else if(!FarmTiles[_x].ContainsKey(_y)){
			return null;
		}else{
			return FarmTiles [_x] [_y];
		}
	}

	public static Vector2 TileToWorldPos (int iosx,int iosy){
		return new Vector2( (iosx-iosy) * 0.3f , (iosx+iosy)* 0.15f);
	}
	public static Vector2 WorldPosToTile (float x,float y){
		return new Vector2(Mathf.Round ((x /0.3f +y/0.15f) / 2) , Mathf.Round ((y/0.15f - x/0.3f)/2));
	}


	//prefabs
	static Dictionary<string,GameObject> prefabs = new Dictionary<string, GameObject>();
	public static GameObject getPrefabByName(string name){
		if (prefabs.ContainsKey (name)) {
			return prefabs [name];
		} else {
			GameObject loadPre =	Resources.Load ("prefabs/" + name) as GameObject;
			prefabs [name] = loadPre;
			return loadPre;
		}
	}

}

