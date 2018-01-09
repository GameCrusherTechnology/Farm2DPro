using UnityEngine;
using System.Collections;

public class EntityPosition : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	int posX;
	int posY;
	int boundX;
	int boundY;

	public void init(int px,int py,int bx,int by){
		posX = px;
		posY = py;
		boundX = bx;
		boundY = by;
	}

	public void validatePos(int gardenPosx,int gardenPosy){
		Vector3 p = DataManager.TileToWorldPos (posX, posY) ;
		p.z = DataManager.getShowPosz (gardenPosx+posX, gardenPosy + posY,boundX,boundY);
		transform.localPosition = p;
	}

}

