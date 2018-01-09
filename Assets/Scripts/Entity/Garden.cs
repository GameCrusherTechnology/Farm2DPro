using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Garden : MonoBehaviour {
	
	List<Crop> crops = new List<Crop>();
	List<EntityPosition> floors = new List<EntityPosition>();
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	int refreshFrame = 30;
	void Update () {
		if (curStep >0 && curStep < maxStep) {
			if (refreshFrame > 0) {
				refreshFrame--;
			} else {
				refreshFrame = 30;
				checkCurStep ();
			}
		} 
			
	}

	GardenData gData;

	public void init(GardenData data){
		gData = data;

		setPosition ();
		setTiles ();
		initCrop ();
		showBackFloor ();
	}
		
	void showBackFloor(){
		
		floors = new List<EntityPosition> ();
		if(gardenData.floorItemSpec() != null){
			int l = gardenData.floorItemSpec().expand_x;
			GameObject newEntity = GameManager.getPrefabByName ("Floor");
			newEntity.GetComponent<SpriteRenderer> ().sprite = GameManager.getSpByName (gardenData.floorItemSpec().surfaceName);
			showBack (newEntity,l);
		}

	}

	void showBack(GameObject entity,int length = 1){
		
		int wT = (gData.bound_m - gData.bound_x + 1) * 2;
		int hT = (gData.bound_n - gData.bound_y + 1) * 2;
		int[] boundxARR =  new int[]{ gData.bound_x*2-1,(gData.bound_m+1)*2 };
		int[] boundyARR =  new int[]{ gData.bound_y*2-1,(gData.bound_n+1)*2 };

		int nextX = boundxARR[0];
		EntityPosition ePos;
		for (int px = boundxARR[0]; px <=boundxARR[1]; px++) {
			if(px == nextX){
				GameObject floorEntity = (GameObject)Instantiate (entity, Vector3.zero, Quaternion.identity, transform);
				floorEntity.transform.localScale = new Vector3 (-1,1,1);
				ePos = floorEntity.AddComponent<EntityPosition>();
				ePos.init (px,boundyARR[0],length,1);
				ePos.validatePos (gData.pos_x,gData.pos_y);
				floors.Add (ePos);

				floorEntity = (GameObject)Instantiate (entity, Vector3.zero, Quaternion.identity, transform);
				floorEntity.transform.localScale = new Vector3 (-1,1,1);
				ePos = floorEntity.AddComponent<EntityPosition>();
				ePos.init (px,boundyARR[1],length,1);
				ePos.validatePos (gData.pos_x,gData.pos_y);
				floors.Add (ePos);

				if (length == 1) {
					nextX = px + length;
				} else {
					if (px + length - 1 >= boundxARR [1]) {
						break;
					} else {
						if(boundxARR[1] - px < (length-1)*2){
							nextX = boundxARR[1] - length+1;
						}else{
							nextX = px + length - 1;
						}
					}
				}

			}
		}

		int nextY = boundyARR[0];
		for (int py = boundyARR[0]; py <= boundyARR[1]; py++) {

			if(length==1 && (py == boundyARR[0] || py == boundyARR[1])){
				nextY = py + 1;
			}
			else if(py == nextY){
				
				GameObject floorEntity = (GameObject)Instantiate (entity, Vector3.zero, Quaternion.identity, transform);
				ePos = floorEntity.AddComponent<EntityPosition>();
				ePos.init (boundxARR[0],py,1,length);
				ePos.validatePos (gData.pos_x,gData.pos_y);
				floors.Add (ePos);

			
				floorEntity = (GameObject)Instantiate (entity, Vector3.zero, Quaternion.identity, transform);
				ePos = floorEntity.AddComponent<EntityPosition>();
				ePos.init (boundxARR[1],py,1,length);
				ePos.validatePos (gData.pos_x,gData.pos_y);
				floors.Add (ePos);

				if (length == 1) {
					nextY = py + length;
				} else {
					if (py + length - 1 >= boundyARR [1]) {
						break;
					} else {
						if(boundyARR[1] - py < (length-1)*2){
							nextY = boundyARR[1] - length + 1;
						}else{
							nextY = py + length - 1;
						}
					}
				}
			}
		}
	}

	void initCrop(){
		curStep = 0;
		GameObject newEntity = GameManager.getPrefabByName ("CropEntity");
		for(int px = gData.bound_x;px<=gData.bound_m;px++){
			for(int py = gData.bound_y;py<=gData.bound_n;py++){

				GameObject cropEntity = (GameObject)Instantiate (newEntity, Vector3.zero, Quaternion.identity, transform);
				crops.Add (cropEntity.GetComponent<Crop> ());
				cropEntity.AddComponent<EntityPosition> ().init (px*2,py*2,2,2);
				cropEntity.GetComponent<EntityPosition> ().validatePos (gData.pos_x,gData.pos_y);
			}
		}
		if(gardenData.crop_id != null){
			checkCurStep ();
		}
	}

	void setPosition(){
		Vector3 p = DataManager.TileToWorldPos (gardenData.pos_x, gardenData.pos_y);
		transform.localPosition = p ;

	}

	void setTiles(){
		for (int px = gData.bound_x * 2 - 1; px <= (gData.bound_m + 1) * 2; px++) {
			for (int py = gData.bound_y* 2 - 1; py <= (gData.bound_n+1)*2; py++) {
				Tile tile = DataManager.getTile (gData.pos_x + px, gData.pos_y + py );
				if (tile != null) {
					tile.addEntity (gData.data_id,DataManager.ENTITY_GARDEN);
				} else {
					Debug.Log ("notile");
				}
			}
		}
	}
	void cleanTiles(){
		for (int px = gData.bound_x*2-1; px <= gData.bound_m*2+2 ; px++) {
			for (int py = gData.bound_y*2-1; py <= gData.bound_n*2+2; py++) {
				Tile tile = DataManager.getTile (gData.pos_x + px, gData.pos_y + py );
				if (tile != null) {
					tile.removeEntity (gardenData.data_id,DataManager.ENTITY_GARDEN);
				} else {
					Debug.Log ("notile");
				}
			}
		}
	}

	//expand
	string expandToward;
	OwnedItem costItem;
	bool canExpand = false;
	public void expandGarden(string expandName,OwnedItem _costItem){
		costItem = _costItem;
		expandToward = expandName;
		boardTile = (GameObject)Instantiate (GameManager.getPrefabByName ("BoardTile"), Vector3.zero, Quaternion.identity, transform);
		boardTile.name = "BoardTile";
		boardTile.GetComponent<BoardTile> ().init ((gardenData.bound_x+(expandName=="ExpandLeftB"? -1:0 ))*2-1,(gardenData.bound_y+(expandName=="ExpandRightB"? -1:0 ))*2-1,
			(gardenData.bound_m+(expandName=="ExpandRightT"? 1:0 ))*2+3 ,(gardenData.bound_n+(expandName=="ExpandLeftT"? 1:0 ))*2+3,gardenData.data_id,DataManager.ENTITY_GARDEN);
		canExpand = boardTile.GetComponent<BoardTile> ().checkTiles (new Vector2(gardenData.pos_x,gardenData.pos_y));

		FarmManager.Instance.intoExpandMode(expandConfirm,expandCancel);
	}

	bool isBuyMode = false;
	public void intoBuyMode(GardenData data){
		gData = data;
		setPosition ();

		initCrop ();
		showBackFloor ();

		isBuyMode = true;
		GameObject boardT = (GameObject)Instantiate (GameManager.getPrefabByName ("BoardTile"), Vector3.zero, Quaternion.identity, transform);
		boardT.name = "BoardTile";
		intoDrag (boardT);
	}

	DecorationItemSpec lastFloorSpec;
	public void intoChangeSurfaceMode(DecorationItemSpec spec){
		lastFloorSpec = gardenData.floorItemSpec();
		gardenData.changetFloor (spec);
		showBackFloor ();
		FarmManager.Instance.intoExpandMode(changeFloorConfirm,changeFloorCancel);
	}

	BoardTile dragTile;
	GameObject boardTile;
	Tile lastTile;
	public void intoDrag(GameObject _boardTile){
		boardTile = _boardTile;
		dragTile = boardTile.GetComponent<BoardTile> ();
		dragTile.init (gardenData.bound_x*2-1,gardenData.bound_y*2-1,gardenData.bound_m*2+3 ,gardenData.bound_n*2+3,gardenData.data_id,DataManager.ENTITY_GARDEN);
		FarmManager.Instance.intoDraggingMode(checkDragging,draggingConfirm,draggingCancel);

		checkingDrag (DataManager.getTile (gardenData.pos_x,gardenData.pos_y));
	}
		

	void checkDragging(){
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
			gardenData.pos_x = lastTile.x;
			gardenData.pos_y = lastTile.y;

			setPosition();
			setTiles ();
			refreshEntityPos ();

			//保存数据
			if (isBuyMode){
				FarmManager.Instance.addEntity (gardenData.data_id, gameObject, DataManager.ENTITY_GARDEN);
			}
			GameManager.MyPlayData.upDataGarden(gardenData);
		} else {
			if (isBuyMode) {
				cleanTiles ();
				Destroy (gameObject);
			} else {
				//放回原地 不保存
				setPosition();
			}

		}
		Destroy (boardTile);
	}

	void draggingCancel(){

		setPosition();

		if(isBuyMode){
			cleanTiles();
			Destroy (gameObject);
		}

		if(isBuyMode){
			cleanTiles();
			Destroy (gameObject);
		}
		Destroy (boardTile);
	}

	//expand
	void expandConfirm(){
		if (canExpand) {
			switch (expandToward) {
			case "ExpandLeftB":
				gardenData.bound_x -= 1;
				break;
			case "ExpandRightB":
				gardenData.bound_y -= 1;
				break;
			case "ExpandLeftT":
				gardenData.bound_n += 1;
				break;
			case "ExpandRightT":
				gardenData.bound_m += 1;
				break;
			}
			cleanTiles();
			refreshGarden ();
			GameManager.MyPlayData.upDataGarden(gardenData);
		} else {
			DialogManager.showMessagePanel (LanController.getString("warningCannotExpandGarden"));
			Destroy (boardTile);
		}
	}
	void expandCancel(){
		Destroy (boardTile);
	}
	//CHANGESURFACE
	void changeFloorConfirm(){
		//save
		GameManager.MyPlayData.upDataGarden(gardenData);
	}
	void changeFloorCancel(){
		gardenData.changetFloor (lastFloorSpec);
		showBackFloor ();
			
	}

	void refreshGarden(){
		for(int i=0;i < transform.childCount;i++){
			Destroy (transform.GetChild (i).gameObject);
		}

		setPosition();
		setTiles ();

		crops = new List<Crop> ();
		initCrop ();
		showBackFloor ();
	}
	void refreshEntityPos(){
		foreach(Crop crop in crops){
			crop.GetComponent<EntityPosition> ().validatePos (gardenData.pos_x, gardenData.pos_y);
		}
		foreach (EntityPosition ePos in floors) {
			ePos.validatePos (gardenData.pos_x, gardenData.pos_y);
		}
	}

	//operate
	public void plantCrop(string id){

		gData.setCrop(id);
		gData.plantTime = TimeManager.CurrentSystemNum;
		curStep = 0;
		checkCurStep ();
	}
	public void harvestCrop(){
		//存储
		int harvestCount = gData.cropItemSpec().productCount;
		OwnedItem harvestCrop = new OwnedItem();
		harvestCrop.item_id = gData.cropItemSpec().productId;
		harvestCrop.count =	gData.cropCount() * harvestCount;
		curPlayer.addOwnedItem (harvestCrop);


		foreach(Crop crop in crops){
			FarmManager.Instance.showHarvestEffect (gData.cropItemSpec().productSpec, harvestCount, crop.transform.position);
		}
		gData.crop_id = null;
		curStep = 0;
		validateSurface ();
	}
	public void speedCrop(){
		gData.plantTime -= 200;
		checkCurStep ();
	}

	void validateSurface(){
		if (curStep == 0) {
			foreach(Crop crop in crops){
				crop.validateSurface (null);
			}
		} else {
			CropItemSpec spec = SpecController.getItemById (gData.crop_id) as CropItemSpec;
			Sprite sp = GameManager.getSpByName ("Texture/crop/"+spec.name +"Static000" + curStep);
			foreach(Crop crop in crops){
				crop.validateSurface (sp);
			}
		}

	}

	int curStep =0;
	int maxStep =5;
	void checkCurStep(){
		int lTime = (TimeManager.CurrentSystemNum - gData.plantTime);
		int stepT =(int) gData.cropItemSpec().growStepTime;
		Debug.Log (lTime);
		int newStep = curStep;
		while(newStep < maxStep){
			if (lTime >= stepT * newStep) {
				newStep++;
			} else {
				break;
			}
		}

		if(newStep != curStep){
			Debug.Log (newStep);
			curStep = newStep;
			validateSurface ();
		}
	}

	public bool canHavest{
		get{ 
			return CropId!= null && curStep >= maxStep;
		}
	}
	public string CropId{
		get{ 
			return gData.crop_id;
		}
	}
	public GardenData gardenData{
		get{ 
			return gData;
		}
	}
	public int LeftTime{
		get{ 
			if (gData.cropItemSpec() != null) {
				return (int)Mathf.Max (0, gData.cropItemSpec().growStepTime * (maxStep-1) -( TimeManager.CurrentSystemNum - gData.plantTime ));
			} else {
				return 0;
			}

		}
	}

	public string getName(){
		if (gardenData.cropItemSpec() != null) {
			return gardenData.cropItemSpec().itemName;
		} else {
			return gardenData.gardenItemSpec().itemName;
		}
	}
	PlayerData curPlayer{
		get{ 
			return GameManager.MyPlayData;
		}
	}

	void deleteGarden(){
		
	}

}
