using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {
	// Use this for initialization
	private float sx =0;
	private float sy =0;
	private int count;
	void Start () {
		initScale ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (count <=0) {
			initScale ();

		} else {
			count--;
			sx += 0.01f;
			sy += 0.01f;
			transform.localScale = new Vector3(sx,sy,0);
		}
	}
	void initScale(){
		count = Random.Range (200, 300);
		sx = sy =  Random.value ;
	}


}
