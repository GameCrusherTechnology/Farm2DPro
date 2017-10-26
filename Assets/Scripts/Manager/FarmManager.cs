using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
	public GameObject loadingLayer;
	public GameObject cropLayer;

	PlayerData currentPlayerData ;
	// Use this for initialization
	void Start ()
	{
		showLoading ();
		int gameuid = DataManager.getGameUid ();
		if( gameuid == 0){
			GameManager.isNewPlayer = true;
			GameManager.MyPlayData = DataManager.creatNewer ();
		}else{
			GameManager.MyPlayData = DataManager.getMyData ();
		}
		currentPlayerData = GameManager.MyPlayData;
		init ();
	}


	//获取数据
	IEnumerator UserLogin(){
		WWWForm form = new WWWForm ();
		string url = GameManager.Server_Url + "?Target=BaseCommand.callCommand";
		form.AddField ("Target","BaseCommand.callCommand");
		WWW w  = new WWW(url);
		yield return w;

		if(w.error!=null)
		{
			print("错误:"+w.error);
		}
		else
		{
			print("OK");
			print(w.text); //服务器端返回的数据
			print("长度:"+w.text.Length.ToString());


			init ();
		}
	}	


	IEnumerator DelayCall(){
		yield return new WaitForSeconds(1);
	}
	void init(){
		initEntities ();

		hideLoading ();
	}

	void initBackground(){
		
	}

	void initEntities(){
		List<GardenData> gardens =	currentPlayerData.Gardens;
		GameObject garden = GameManager.getPrefabByName ("Garden");
		foreach(GardenData data in gardens){
			Vector2 pos = GameManager.TileToWorldPos (data.pos_x, data.pos_y);
			GameObject g = (GameObject)Instantiate (garden, pos, Quaternion.identity);
			g.transform.SetParent (cropLayer.transform);
			g.GetComponent<Garden> ().init (data);
		}
	}

	//loading
	void showLoading(){
		loadingLayer.SetActive (true);
		loadingLayer.GetComponent<LoadingScreen> ().show ();


	}
	void hideLoading(){
		loadingLayer.GetComponent<LoadingScreen> ().dispose ();
	}

	// Update is called once per frame
	void Update ()
	{
	
	}


}

