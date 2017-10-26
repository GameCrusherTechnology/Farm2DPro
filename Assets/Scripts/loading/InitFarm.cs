using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.IO;  

public class InitFarm : MonoBehaviour {
	public GameObject loadingScene;
	private string url = "http://192.168.1.101/NatureServer/test.php"; 

	// Use this for initialization
	void Start () {
		showLoading ();
		StartCoroutine(Login());	
		Login();
	}

	IEnumerator Login(){
//		StartCoroutine (DelayCall ());
		WWWForm form = new WWWForm ();
		url = url + "?Target=BaseCommand.callCommand";
		form.AddField ("Target","BaseCommand.callCommand");
//		Dictionary<string,string> dic = new Dictionary<string,string> ();
//
//		form.AddField ("Content",JsonUtility.ToJson(dic));
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
		}
	}


	IEnumerator DelayCall(){
		yield return new WaitForSeconds(1);
		loginFarm ();
	}
	void loginFarm(){
		if (curLoading) {
			curLoading.GetComponent<LoadingScreen> ().dispose ();
			Destroy (curLoading);
			curLoading = null;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	private GameObject curLoading;
	void showLoading(){
		if (!curLoading ) {
			curLoading=(GameObject)Instantiate(loadingScene);
		}

	}
}
