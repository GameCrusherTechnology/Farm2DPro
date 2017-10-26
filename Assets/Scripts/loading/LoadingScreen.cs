using UnityEngine;
using System.Collections;

public class LoadingScreen: MonoBehaviour {

	public GameObject cloudPrefab;

	float maxWidth;
	float maxHeight;

	ArrayList list;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame

//	private int creatCount = 0;
	void Update () {
		if(isShowing){
			if(list.Count < 50){
				Vector3 randomPos = new Vector3 (Random.Range (-maxWidth,maxWidth), Random.Range (-maxHeight, maxHeight), 0);
				GameObject	cloud=(GameObject)Instantiate(cloudPrefab,randomPos,Quaternion.identity);
				cloud.transform.SetParent (transform);
				cloud.GetComponent<Renderer> ().sortingOrder = 1;
				list.Add (cloud);
			}
		}
	}

	bool isShowing = false;
	public void show(){
		Vector3 screenPos = new Vector3 (Screen.width,Screen.height, 0);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint (screenPos); 
		maxWidth = worldPos.x;
		maxHeight = worldPos.y;

		list = new ArrayList ();
		GameObject prefab;
		for (int i = 0; i < 50; i++) {
			Vector3 randomPos = new Vector3 (Random.Range (-maxWidth,maxWidth), Random.Range (-maxHeight, maxHeight), 0);
			prefab=(GameObject)Instantiate(cloudPrefab,randomPos,Quaternion.identity);
			prefab.transform.SetParent (transform);
			prefab.GetComponent<Renderer> ().sortingOrder = 1;
			list.Add (prefab);
		}

		isShowing = true;
	}
	public void dispose(){
		isShowing = false;

		foreach (GameObject cloud in list) {
			Destroy (cloud);
		}
	}
}
