using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Pet : MonoBehaviour
{
	protected PetData petData;

	int speed = 2;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame

	public void Update ()
	{
		checkPosz ();
	}


	public void init(PetData data){
		petData = data;
		doNextAction ();
	}

	Tile lastTile;
	void checkPosz(){
		Vector2 vec = transform.localPosition;
		Tile t = DataManager.WorldPosToTile (vec.x, vec.y);
		if(t!= null && t!= lastTile){
			
			transform.transform.position = new Vector3 (transform.position.x,transform.position.y,DataManager.getShowPosz (t.x, t.y, 1, 1));

			lastTile = t;
		}
	}

	void moveToPoint(Vector2 targetPos){
		float T = Vector2.Distance (targetPos, transform.position) / speed;
		transform.DOLocalMoveX (targetPos.x, T).OnComplete(delegate {
			doNextAction();
		});
		transform.DOLocalMoveY (targetPos.y, T);
	}

	void doNextAction(){
		Vector2 vec = new Vector2 (Random.Range (-3f, 3f), Random.Range (-3f, 3f));
		moveToPoint (vec);
	}
}

