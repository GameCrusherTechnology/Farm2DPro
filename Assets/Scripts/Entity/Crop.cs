using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void validateSurface(Sprite sp){
		transform.FindChild ("Crop").GetComponent<SpriteRenderer> ().sprite = sp;
	}
}
