using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FriendPanel : MonoBehaviour
{
	public GameObject Render;

	public GameObject strangerList;
	public GameObject friendList;
	public RectTransform strangerListGrid;
	public RectTransform friendListGrid;

	public GameObject friendBut;
	public GameObject strangerBut;


	// Use this for initialization
	void Start ()
	{
	
	}
	// Update is called once per frame
	void Update ()
	{
	
	}



	public void init(){
		onClickTabFriend ();
	}

	void initFriendList(){
		if (friendListGrid.childCount == 0) {
			Dictionary<string,PlayerData> friendList = DataManager.friendsList;
			foreach (PlayerData player in friendList.Values) {
				GameObject r = (GameObject)Instantiate (Render, Vector3.zero, Quaternion.identity, friendListGrid);
				r.GetComponent<RectTransform> ().localScale = Vector3.one;
				r.SetActive (true);

				r.name = player.gameUid;
				r.transform.FindChild ("NameText").GetComponent<Text> ().text = player.name;
				r.transform.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (player.charactorIconAddress);
				r.transform.FindChild ("Button").FindChild("ButtonText").GetComponent<Text> ().text = LanController.getString ("visit").ToUpper ();

			}
			float w = friendList.Count * 110 + 10 * (friendList.Count + 1);
			friendListGrid.sizeDelta = new Vector2 (w,110);
			friendListGrid.localPosition = new Vector3(w/2 - 250 ,6,0);
		} else {

		}
	}

	void initStrangerList(){
		
	}

	public void onClickTabFriend(){
		strangerBut.transform.GetComponent<Image>().color = Color.white;
		strangerBut.transform.localScale = new Vector3 (1,1,1);

		friendBut.transform.GetComponent<Image>().color = Color.yellow;
		friendBut.transform.localScale = new Vector3 (1.2f,1.2f,1);

		strangerList.SetActive (false);
		friendList.SetActive (true);

		initFriendList ();
	}
	public void onClickTabStranger(){
		friendBut.transform.GetComponent<Image>().color = Color.white;
		friendBut.transform.localScale = new Vector3 (1,1,1);

		strangerBut.transform.GetComponent<Image>().color = Color.yellow;
		strangerBut.transform.localScale = new Vector3 (1.2f,1.2f,1);

		strangerList.SetActive (true);
		friendList.SetActive (false);
		initStrangerList ();
	}

	public void onClickVisit(GameObject render){
		GameManager.visitPlayer (render.name);
	}

	IEnumerator visitFriend(string friendid){
		PlayerData friendData = DataManager.getFriend (friendid);
		if (friendData == null || (TimeManager.CurrentSystemNum - friendData.lastTime) >= 24 * 3600) {

			WWWForm form = new WWWForm ();
			form.AddField ("command", GameManager.VisitFriend);

			AmfObject amf = new AmfObject ();
			amf.gameuid = friendid;

			form.AddField ("data", JsonUtility.ToJson (amf));		
			WWW www = new WWW (GameManager.Server_Url, form);
			while (!www.isDone) {
				Debug.Log ("wait...");
			}
			yield return www;

			if (www.error != null) {
				Debug.LogError (www.error);
				if(friendData == null){
					friendData = DataManager.creatNewer ();
				}
				GameManager.CurPlayer = friendData;

			} else {
				VisitAmfObject amfData = JsonUtility.FromJson<VisitAmfObject> (www.text);
				GameManager.CurPlayer = amfData.player;

			}
		} else {
			yield return new WaitForSeconds(2f);
			GameManager.CurPlayer = friendData;
		}

	}


	public void onClickOut(){
		gameObject.SetActive (false);
	}
	PlayerData myPlayerData{
		get{ 
			return GameManager.MyPlayData;
		}
	}
}

