using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager 
{
	static GameManager _manager;
	public static GameManager getInstance(){
		if(_manager == null){
			_manager = new GameManager ();
		}
		return _manager;
	}

	public static string Server_Url = "http://127.0.0.1/SuperFarmerServer/data/FarmCommand.php";
	//public static string Server_Url = "http://192.168.1.1/SuperFarmerServer/data/FarmCommand.php";
	public static PlayerData MyPlayData ;
	public static PlayerData CurPlayer ;
	public static string visitId;

	public static void visitPlayer(string friendId){
		visitId = friendId;
		SceneManager.LoadScene (0);
	}
	public static void returnHome(){
		visitId = MyPlayData.gameUid;
		SceneManager.LoadScene (0);
	}

	public static bool isMyFarm = true ; 
	public static bool isNewPlayer = false;

	//command
	public static string LoginUser = "user/UserLogin";
	public static string VisitFriend = "user/VisitFriend";
	//
	public const int MAX_FARM_LENGTH = 100;
	//workshop
	public static Dictionary<string,TreasureData> Treasures = new Dictionary<string, TreasureData>();
	public static string[] TreasureNames = {"LittleCoin","LargeCoin","LittleGem","LargeGem"};

	public static void initTreasures(){
		Treasures.Add ("LittleCoin", new TreasureData ("LittleCoin","coin",100000,20.6f));
		Treasures.Add ("LargeCoin", new TreasureData ("LargeCoin","coin",1000000,120.6f));
		Treasures.Add ("LittleGem", new TreasureData ("LittleGem","gem",100,20.6f));
		Treasures.Add ("LargeGem", new TreasureData ("LargeGem","gem",1000,120.6f));
	}

	//prefabs
	static Dictionary<string,GameObject> prefabs = new Dictionary<string, GameObject>();
	public static GameObject getPrefabByName(string name){
		if (prefabs.ContainsKey (name)) {
			return prefabs [name];
		} else {
			GameObject loadPre = Resources.Load ("prefabs/" + name) as GameObject;
			prefabs [name] = loadPre;
			return loadPre;
		}
	}

	static Dictionary<string,Sprite> textures = new Dictionary<string, Sprite>();
	public static Sprite getSpByName(string name){
		if (textures.ContainsKey (name)) {
			return textures [name];
		} else {
			Sprite loadPre =	Resources.Load (name,typeof(Sprite)) as Sprite;
			textures [name] = loadPre;
			return loadPre;
		}
	}
	public static void cleanMemory(){
		Resources.UnloadUnusedAssets ();
	}




}

