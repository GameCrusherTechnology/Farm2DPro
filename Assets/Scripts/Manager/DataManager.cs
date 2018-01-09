using UnityEngine;
using System.Collections.Generic;
using LitJson;

public class DataManager {
	public const string ENTITY_GARDEN = "GardenEntity";
	public const string ENTITY_DECORATION = "DecorateEntity";
	public const string ENTITY_HOUSE = "DecorateHouse";
	public const string ENTITY_TREE = "TreeEntity";

	//NPC
	public static string[] CharactorList = {"Boy","Girl","GentleMan","Lady","Madam","Elder"};

	public static string getUid(){
		return "Farmuid_"+ System.DateTime.Now.ToBinary();
		string uid =  PlayerPrefs.GetString ("SuperFarmerUid");
		if (uid == null || uid == "") {
			uid =  SystemInfo.deviceUniqueIdentifier;
			if(uid == null  || uid == ""){
				uid = "SuperFarmerUid_"+ System.DateTime.Now;
			}
			PlayerPrefs.SetString ("SuperFarmerUid",uid);
		} 
		return uid;
	}


	public static PlayerData getMyData(){
		Debug.Log (PlayerPrefs.GetString("MyLocalPlayerData"));
		PlayerData user = JsonUtility.FromJson<PlayerData> (PlayerPrefs.GetString("MyLocalPlayerData"));
		return user;
	}


	public static PlayerData creatNewer(){
		Dictionary<string, GardenData> gardens = new  Dictionary<string, GardenData> ();
		int t = 0;
		while(t < 1){
			GardenData newGarden = new GardenData ();
			newGarden.creatGarden (1000,"31000",-5,-5);
			gardens.Add ("1000",newGarden);
			t++;
		}

		PlayerData user = new PlayerData ();
		user.Gardens = gardens;

		BuildingData buildData = new BuildingData ();
		buildData.creatBuildingData (1000,"30000",0,0);
		List< BuildingData> buildings = new List<BuildingData> ();
		buildings.Add (buildData);

		BuildingData buildData3 = new BuildingData ();
		buildData3.creatBuildingData (1002,"40000",2,2);
		buildings.Add (buildData3);

		user.Buildings = buildings;

		Dictionary<string,OwnedItem> items = new Dictionary<string, OwnedItem> ();
		OwnedItem i = new OwnedItem ();
		i.init ("10001",20);
		items.Add ("10001",i);

		OwnedItem i1 = new OwnedItem ();
		i1.init ("10002",5);
		items.Add ("10002",i1);

		OwnedItem i2 = new OwnedItem ();
		i2.init ("10003",5);
		items.Add ("10003",i2);

		OwnedItem i4 = new OwnedItem ();
		i4.init ("10004",5);
		items.Add ("10004",i4);

		OwnedItem i5 = new OwnedItem ();
		i5.init ("10005",5);
		items.Add ("10005",i5);

		OwnedItem i6 = new OwnedItem ();
		i6.init ("10006",5);
		items.Add ("10006",i6);

		OwnedItem i7 = new OwnedItem ();
		i7.init ("10007",5);
		items.Add ("10007",i7);

		OwnedItem i8 = new OwnedItem ();
		i8.init ("10008",5);
		items.Add ("10008",i8);

		OwnedItem i9 = new OwnedItem ();
		i9.init ("10009",5);
		items.Add ("10009",i9);

		OwnedItem i10 = new OwnedItem ();
		i10.init ("10010",5);
		items.Add ("10010",i10);

		OwnedItem i11 = new OwnedItem ();
		i11.init ("10011",5);
		items.Add ("10011",i11);

		user.ownedItems = items;

		PetData petData = new PetData ();
		petData.creatPet ("60000");
		user.pets.Add (petData);


		PlayerData player1 = new PlayerData ();
		player1.gameUid = "1000";
		player1.name = "Rose";
		player1.charactor = "Rose";
		addFriend (player1);

		return user;
	}

	static int worldLength = 100;
	//tile
	static Dictionary<int,Dictionary<int,Tile>> FarmTiles = new Dictionary<int, Dictionary<int, Tile>>();
	public static void InitTile(int l){
		worldLength = l;
		Dictionary<int,Dictionary<int,Tile>> tiles = new Dictionary<int, Dictionary<int, Tile>>();
		int a = -l;
		while(a < l){
			int b = -l;
			Dictionary<int,Tile> group = new Dictionary<int, Tile> ();
			while (b < l) {
				group[b] = new Tile(a,b);
				b++;
			}
			tiles[a] = group;
			a++;
		}
		FarmTiles = tiles;
	}

	public static void expandTiles(int newL){
		worldLength = newL;
		Dictionary<int,Dictionary<int,Tile>> tiles = new Dictionary<int, Dictionary<int, Tile>>();
		int a = -newL;
		Tile t;
		while(a < newL){
			int b = -newL;
			Dictionary<int,Tile> group = new Dictionary<int, Tile> ();
			while (b < newL) {
				t = getTile (a,b);
				if (t == null) {
					group[b] = new Tile(a,b);
				} else {
					group[b] = t;
				}
				b++;
			}
			tiles[a] = group;
			a++;
		}
		FarmTiles = tiles;
	}

	public static Vector2 TileToWorldPos (int iosx,int iosy){
		return new Vector2( (iosx-iosy) * 0.3f , (iosx+iosy)* 0.15f) ;
	}
	public static Vector2 TileCenterToWorldPos (int iosx,int iosy){
		return new Vector2( (iosx-iosy) * 0.3f , (iosx+iosy)* 0.15f +0.15f) ;
	}
	public static Tile WorldPosToTile (float x,float y){
		return getTile (Mathf.FloorToInt ((x /0.3f +y/0.15f) / 2),Mathf.FloorToInt ((y/0.15f - x/0.3f)/2));

	}
	public static Vector2 WorldPosToNearTile (float x,float y){
		
		return new Vector2(Mathf.Max(-worldLength,Mathf.Min(worldLength, Mathf.FloorToInt ((x /0.3f +y/0.15f) / 2))) , Mathf.Max(-worldLength,Mathf.Min(worldLength,Mathf.FloorToInt ((y/0.15f - x/0.3f)/2))));
	}

	public static Tile getTile(int px,int py){
		if (FarmTiles.ContainsKey (px) && FarmTiles [px].ContainsKey (py)) {
			return FarmTiles [px] [py];
		} else {
			return null;
		}
	}

	public static Tile getTileFromWorld(Vector3 vec){
		int px = Mathf.FloorToInt ((vec.x / 0.3f + vec.y / 0.15f) / 2);
		int py = Mathf.FloorToInt ((vec.y/0.15f - vec.x/0.3f)/2);
		if (FarmTiles.ContainsKey (px) && FarmTiles [px].ContainsKey (py)) {
			return FarmTiles [px] [py];
		} else {
			return null;
		}
	}
	public static Tile getNearTileFromWorld(Vector3 vec){
		int px = Mathf.Clamp (Mathf.FloorToInt ((vec.x / 0.3f + vec.y / 0.15f) / 2),-worldLength,worldLength-1);
		//int px =Mathf.Max(-worldLength,Mathf.Min(worldLength, Mathf.FloorToInt ((vec.x / 0.3f + vec.y / 0.15f) / 2)));
		//int py =Mathf.Max(-worldLength,Mathf.Min(worldLength, Mathf.FloorToInt ((vec.y/0.15f - vec.x/0.3f)/2)));
		int py = Mathf.Clamp (Mathf.FloorToInt ((vec.y/0.15f - vec.x/0.3f)/2),-worldLength,worldLength-1);
		if (FarmTiles.ContainsKey (px) && FarmTiles [px].ContainsKey (py)) {
			return FarmTiles [px] [py];
		} else {
			return null;
		}
	}

	//
	public static float getShowPosz(int x, int y , float w, float h ){
		return 1 + float.Parse(((( x + w/2  + y + h/2) * 1000 + x + w/2 ) / 1000000).ToString("F6"));
	}

	//friends
	private static Dictionary<string,PlayerData> _friendsList ;
	public static PlayerData getFriend(string _friendid){

		if (friendsList.ContainsKey (_friendid)) {
			return friendsList [_friendid];
		} else {
			return null;
		}
	}

	public static void addFriend(PlayerData data){
		if (friendsList.ContainsKey (data.gameUid)) {
			friendsList [data.gameUid] = data;
		} else {
			friendsList.Add (data.gameUid,data);
		}

		PlayerPrefs.SetString("LocalFriends",JsonUtility.ToJson(friendsList));
	}

	public static Dictionary<string,PlayerData> friendsList{
		get{
			if (_friendsList == null) {
				_friendsList = JsonUtility.FromJson<Dictionary<string,PlayerData>> (PlayerPrefs.GetString ("LocalFriends"));
			}
			if(_friendsList == null) {
				_friendsList = new  Dictionary<string, PlayerData> ();
			}
			return _friendsList;
		}
		set{ 
			_friendsList = value;
		}
	}

	//shu ju cunchu
	private static Dictionary<string,PlayerData> _localPlayers;
	public static PlayerData getLocalPlayer(string _gameuid){

		if (localPlayers.ContainsKey (_gameuid)) {
			return localPlayers [_gameuid];
		} else {
			return null;
		}
	}

	public static Dictionary<string,PlayerData> localPlayers{
		get{ 
			if(_localPlayers == null){
				_localPlayers = JsonUtility.FromJson<Dictionary<string, PlayerData>> (PlayerPrefs.GetString("LocalPlayers"));
			}
			return _localPlayers;
		}
	}

	public static void savePlayer(PlayerData data){
		if (friendsList.ContainsKey (data.gameUid)) {
			friendsList [data.gameUid] = data;
			PlayerPrefs.SetString ("LocalFriends", JsonUtility.ToJson (friendsList));
		} else {
			if (localPlayers.ContainsKey (data.gameUid)) {
				localPlayers [data.gameUid] = data;
			} else {
				localPlayers.Add (data.gameUid, data);
			}
			PlayerPrefs.SetString ("LocalPlayers", JsonUtility.ToJson (localPlayers));
		}
	}
	public static void saveMyPlayer(){
		Debug.Log (JsonMapper.ToJson(GameManager.MyPlayData));
		PlayerPrefs.SetString ("MyLocalPlayerData",JsonMapper.ToJson(GameManager.MyPlayData));
	}

}
