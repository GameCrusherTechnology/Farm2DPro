using UnityEngine;
using System.Collections;

public class Decoration : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	DecorationItemSpec decSpec;
	public BuildingData buildingData;
	Transform surfaceTran;
	Transform boardTran;

	public void init(BuildingData data){
		buildingData = data;
		decSpec = SpecController.getItemById (data.item_id) as DecorationItemSpec;
		surfaceTran = transform.FindChild ("Surface");
		if(decSpec.surfaceName != null){
			surfaceTran.GetComponent<SpriteRenderer> ().sprite = GameManager.getSpByName ( decSpec.surfaceName);
			surfaceTran.localScale = new Vector3 (buildingData.isTurnOver?-1:1,1,1);
		}else {
			surfaceTran.gameObject.SetActive (false);
		}
		boardTran = transform.FindChild ("Board");
		if (decSpec.boardName != null) {
			boardTran.GetComponent<SpriteRenderer> ().sprite = GameManager.getSpByName (decSpec.boardName);
			boardTran.localScale = new Vector3 (buildingData.isTurnOver?-1:1,1,1);
		} else {
			boardTran.gameObject.SetActive (false);
		}

		setPosition ();
		setTiles ();
	}

	public void setPosition(){
		Vector3 p = DataManager.TileToWorldPos (buildingData.pos_x, buildingData.pos_y);
		p.z = DataManager.getShowPosz (buildingData.pos_x,buildingData.pos_y,buildingData.expand_x,buildingData.expand_y);
		transform.localPosition = p;
	}

	public void setTiles(){
		for (int px = 0; px < buildingData.expand_x ; px++) {
			for (int py = 0; py < buildingData.expand_y; py++) {
				Tile tile = DataManager.getTile (buildingData.pos_x + px, buildingData.pos_y + py);
				if (tile != null){
					tile.addEntity (buildingData.data_id,entityType);
				}else {
					Debug.Log ("notile");
				}
			}
		}
	}


	//buy
	bool isBuyMode = false;
	public void intoBuyMode(TreeData data){
		buildingData = data;
		setPosition ();

		isBuyMode = true;
		GameObject boardT = (GameObject)Instantiate (GameManager.getPrefabByName ("BoardTile"), Vector3.zero, Quaternion.identity, transform);
		boardT.name = "BoardTile";
		intoDrag (boardT);
	}



	BoardTile dragTile;
	GameObject boardTile;
	Tile lastTile;
	bool isRotation = false;
	public void intoDrag(GameObject _boardTile){
		boardTile = _boardTile;
		dragTile = boardTile.GetComponent<BoardTile> ();
		dragTile.init (0,0,buildingData.expand_x,buildingData.expand_y,buildingData.data_id,entityType);
		FarmManager.Instance.intoDraggingMode(checkDragging,draggingConfirm,draggingCancel);

		checkingDrag (DataManager.getTile (buildingData.pos_x,buildingData.pos_y));
	}

	void checkDragging( ){
		Tile t = DataManager.getNearTileFromWorld(Camera.main.ScreenToWorldPoint( Input.mousePosition) - FarmManager.FarmWorldPos);
		checkingDrag (t);
	}
	void checkingDrag(Tile t){
		if(t!= null && t!= lastTile){
			Vector3 p = DataManager.TileToWorldPos (t.x, t.y);
			p.z = 1;
			transform.localPosition = p;
			dragTile.checkTiles (new Vector2(t.x,t.y));
			lastTile = t;
		}
	}

	public void intoRotation(GameObject _boardTile){
		buildingData.isTurnOver = !buildingData.isTurnOver;
		isRotation = true;
		surfaceTran.localScale = new Vector3 (buildingData.isTurnOver?-1:1,1,1);
		boardTran.localScale = new Vector3 (buildingData.isTurnOver?-1:1,1,1);

		intoDrag (_boardTile);
	}

	virtual public void delete(){
		GameManager.MyPlayData.removeBuildings (buildingData.data_id);
		cleanTiles();
		FarmManager.Instance.removeEntity (buildingData.data_id,entityType);
		Destroy (gameObject);
	}

	void draggingCancel(){
		setPosition();
		cancelRotate ();
		Destroy (boardTile);

		if(isBuyMode){
			cleanTiles();
			Destroy (gameObject);
		}
	}

	void draggingConfirm(){
		bool canReset = false;
		if (lastTile != null) {
			canReset = dragTile.checkTiles (new Vector2 (lastTile.x, lastTile.y));
		} else {
			canReset = false;
		}
		if (canReset) {
			//移动成功
			cleanTiles();
			buildingData.pos_x = lastTile.x;
			buildingData.pos_y = lastTile.y;
			setPosition();
			setTiles ();
			//保存数据

			if(isBuyMode){
				FarmManager.Instance.addEntity (buildingData.data_id, gameObject, entityType);
			}
			Destroy (boardTile);
		} else {
			draggingCancel ();

		}
	}
	public void cancelRotate(){
		if(isRotation){
			buildingData.isTurnOver = !buildingData.isTurnOver;
			surfaceTran.localScale = new Vector3 (buildingData.isTurnOver?-1:1,1,1);
			boardTran.localScale = new Vector3 (buildingData.isTurnOver?-1:1,1,1);
		}
		isRotation = false;
	}

	public void cleanTiles(){
		int ep_x = isRotation ? buildingData.expand_y : buildingData.expand_x;
		int ep_y = isRotation ? buildingData.expand_x : buildingData.expand_y;


		for (int px = 0; px < ep_x; px++) {
			for (int py = 0; py <ep_y; py++) {
				Tile tile = DataManager.getTile (buildingData.pos_x + px, buildingData.pos_y + py );
				if (tile != null) {
					tile.removeEntity (buildingData.data_id,entityType);
				} else {
					Debug.Log ("notile");
				}
			}
		}
	}

	protected PlayerData curPlayer{
		get { 
			return GameManager.MyPlayData;
		}
	}

	virtual public string getName(){
		return decSpec.itemName;
	}

	virtual public string entityType{
		get{ 
			return DataManager.ENTITY_DECORATION;
			}
	}
}

