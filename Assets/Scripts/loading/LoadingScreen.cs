using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreen: MonoBehaviour {

	public GameObject cloudPrefab;
	public Slider progressBar;
	public Animator effectAnimator;
	public Transform skyLayer;
	public Text loadingText;

	float maxWidth;
	float maxHeight;
	float tarPos;

	ArrayList list= new ArrayList ();
	// Use this for initialization
	void Start () {
		
		show ();
		progressBar.value = 0;
		tarPos = 0.95f;
		effectAnimator.Play ("DogLeft");
		string visitId = GameManager.visitId;

		if (visitId == null) {
			//首次登陆
			SpecController.initGameXML ();
			LanController.ReadAndLoadXml ();
			GameManager.initTreasures ();

			loadingText.text = LanController.getString ("loadingdata").ToUpper();
		//	GameManager.MyPlayData = DataManager.getMyData ();

			if (GameManager.MyPlayData == null) {
				GameManager.isNewPlayer = true;
				GameManager.MyPlayData = DataManager.creatNewer ();
				DataManager.saveMyPlayer ();
			} 

			StartCoroutine (UserLogin ());
			GameManager.CurPlayer = GameManager.MyPlayData;
		} else {

			loadingText.text = LanController.getString ("loadingdata").ToUpper();
			if (visitId == GameManager.MyPlayData.gameUid) {
				StartCoroutine (UserLogin ());
				GameManager.CurPlayer = GameManager.MyPlayData;
			} else {
				StartCoroutine (UserVisit (visitId));
			}
		}

		loadingText.text = LanController.getString ("loadinginit").ToUpper();
		StartCoroutine (wait(1f));

	}

	IEnumerator wait(float t){
		yield return new WaitForSeconds (t);
		tarPos = 1;
	}
		
	// Update is called once per frame

//	private int creatCount = 0;
	void Update () {
			if(list.Count < 30){
				Vector3 randomPos = new Vector3 (Random.Range (-maxWidth,maxWidth), Random.Range (-maxHeight, maxHeight), 0);
				GameObject	cloud=(GameObject)Instantiate(cloudPrefab,randomPos,Quaternion.identity);
				cloud.transform.SetParent (skyLayer);
				cloud.GetComponent<Renderer> ().sortingOrder = 1;
				list.Add (cloud);
			}
		if (progressBar.value < tarPos) {
			progressBar.value += 0.005f;
		} else if(progressBar.value  == 1){
			SceneManager.LoadScene (1);
		}
	}

	void show(){
		Vector3 screenPos = new Vector3 (Screen.width,Screen.height, 0);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint (screenPos); 
		maxWidth = worldPos.x;
		maxHeight = worldPos.y;

		list = new ArrayList ();
		GameObject prefab;
		for (int i = 0; i < 30; i++) {
			Vector3 randomPos = new Vector3 (Random.Range (-maxWidth,maxWidth), Random.Range (-maxHeight, maxHeight), 0);
			prefab=(GameObject)Instantiate(cloudPrefab,randomPos,Quaternion.identity);
			prefab.transform.SetParent (skyLayer);
			prefab.GetComponent<Renderer> ().sortingOrder = 1;
			list.Add (prefab);
		}

	}

	IEnumerator UserLogin(){
		WWWForm form = new WWWForm ();
		form.AddField ("command",GameManager.LoginUser);

		AmfObject amf = new AmfObject();
		amf.uid = DataManager.getUid();

		Debug.Log ("uid :" +amf.uid);

		form.AddField ("data",JsonUtility.ToJson(amf));		


		WWW www = new WWW(GameManager.Server_Url, form);
		while(!www.isDone){
		}
		yield return www;



		if (www.error != null) {
			//Debug.LogError (www.error);
			//Invoke ("UserLogin",2f);

		} else {
			LoginAmfObject loginData =JsonUtility.FromJson<LoginAmfObject> (www.text);
			GameManager.MyPlayData.gameUid  =loginData.gameuid;
		}

		GameManager.CurPlayer = GameManager.MyPlayData;

	}
	IEnumerator UserVisit(string friendId){
		
		PlayerData friendData = DataManager.getFriend (friendId);
		if (friendData == null || (TimeManager.CurrentSystemNum - friendData.lastTime) >= 24 * 3600) {

			WWWForm form = new WWWForm ();
			form.AddField ("command", GameManager.VisitFriend);

			AmfObject amf = new AmfObject ();
			amf.gameuid = friendId;

			form.AddField ("data", JsonUtility.ToJson (amf));		
			WWW www = new WWW (GameManager.Server_Url, form);
			while (!www.isDone) {
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
			GameManager.CurPlayer = friendData;
		}

	}
}
