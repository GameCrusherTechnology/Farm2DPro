using UnityEngine;
using System.Collections.Generic;

public class DataManager {

	public static int getGameUid(){
		return 0;
	}


	public static PlayerData getMyData(){
		List< GardenData> gardens = JsonUtility.FromJson<List< GardenData>> (PlayerPrefs.GetString("myGardens"));

		PlayerData user = new PlayerData ();
		user.Gardens = gardens;
		return user;
	}


	public static PlayerData creatNewer(){
		List< GardenData> gardens = new List<GardenData> ();
		int t = 0;
		while(t < 30){
			GardenData newGarden = new GardenData ();
			newGarden.creatGarden ("",(int)Random.Range(-30f,30f),(int)Random.Range(-30f,30f));
			gardens.Add (newGarden);
			t++;
		}


		PlayerData user = new PlayerData ();
		user.Gardens = gardens;
		return user;
	}



}
