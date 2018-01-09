using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FarmManager : MonoBehaviour
{
	public GameObject FarmWorldLayer;
	public GameObject loadingLayer;
	public GameObject landLayer;
	public GameObject cropLayer;
	public GameObject skyLayer;
	public GameObject backLayer;
	public GameObject effectLayer;
	//ui
	public GameObject UI;

	//mode
	public GameObject ModeCanvas;
	public GameObject DesignPart;
	//Dialog
	public Transform DialogCanvasTrans;

	float _ZoomDifference = 0;  
	float _ZoomSpeed = 100f; 
	float maxOSize ;
	float minOSize ;

	string CurrentMode = "Default";
	const string DefaultMode = "Default";
	const string DraggingMode = "Drag";
	const string ExpandMode = "Expand";
	const string BUYMode = "Buy";

	System.Action DraggingFunc;
	System.Action DesignConfirmFunc;
	System.Action DesignCancelFunc;


	Dictionary<string,GameObject> gardenList = new Dictionary<string, GameObject> ();
	Dictionary<int,GameObject> houseList = new Dictionary<int, GameObject> ();
	Dictionary<int,GameObject> decorationList = new Dictionary<int, GameObject> ();
	Dictionary<int,GameObject> treeList = new Dictionary<int, GameObject> ();
	List<GameObject> fenceList = new List<GameObject>();

	public static Vector3 FarmWorldPos = Vector3.zero;
	Vector3 farmBound = Vector3.zero;

	private static volatile FarmManager instance;   
	private static object syncRoot = new Object();   
	public static FarmManager Instance   
	{   
		get    
		{
			if (instance == null)    
			{
				lock (syncRoot)    
				{   
					if (instance == null)  {
						FarmManager[] instances = FindObjectsOfType<FarmManager>();  
						if (instances != null){  
							for (var i = 0; i < instances.Length; i++) {  
								Destroy(instances[i].gameObject);  
							}  
						}  
						GameObject go = new GameObject("_SingletonBehaviour");  
						instance = go.AddComponent<FarmManager>();  
						DontDestroyOnLoad(go);   
					}  
				}   
			}   
			return instance;   
		}   
	}   

	// Use this for initialization
	void Start ()
	{
		instance = GetComponent<FarmManager> ();

		float standard_aspect = 960f / 640f;
		float device_aspect = (float)Screen.width / Screen.height;
		float adjustor = 0f;
		if(standard_aspect > device_aspect){
			adjustor = standard_aspect / device_aspect;
			DialogCanvasTrans.GetComponent<CanvasScaler> ().matchWidthOrHeight = 0;
		}

		if(GameManager.isNewPlayer){
			DialogManager.showRenamePanel ();
		}
		currentPlayer = GameManager.CurPlayer;
		init ();
	}
		

	void init(){
		DataManager.InitTile (currentPlayer.MaxSceneLength);
		initBackground ();

		initEntities ();
	}

	public void intoFarm(){
		isInfarmMode = true;
	}

	public void showTiles(){
		int l = currentPlayer.MaxSceneLength;
		GameObject tile = (GameObject)GameManager.getPrefabByName ("BoardTile");
		for(int i = -l;i<l;i++){
			for(int j = -l;j<l;j++){
				GameObject xtile = (GameObject)Instantiate (tile,DataManager.TileCenterToWorldPos(i,j), Quaternion.identity, landLayer.transform);
				Tile t = DataManager.getTile (i,j);
				if(t != null){
					if(t.entityDic.Count>0){
						xtile.GetComponent<SpriteRenderer> ().color = Color.red;
					}
				}
			}
		}
	}

	float farmWidth;
	float farmHeight;

	void initBackground(){
		BeDragNum = (float)Screen.width / 10;

		float bt = Screen.width/2 +currentPlayer.MaxSceneLength/10 *300;	

		farmWidth = Mathf.Max(Screen.width*1.2f, currentPlayer.MaxSceneLength*120f + bt);
		farmHeight = Mathf.Max(Screen.height*1.2f, farmWidth / 1.5f);
		float bs = (float)farmWidth / 128;		
		backLayer.transform.localScale = new Vector3 (bs, (float)farmHeight/128, 1);

		skyLayer.transform.localScale = new Vector3 ((float)farmWidth / 128/25,(float)farmHeight / 128/12,1);


		float aspect = (float)Screen.height / Screen.width;
		float asp = (float) farmWidth*aspect/100;
		Camera.main.orthographicSize = maxOSize = (float)Mathf.Min( farmWidth/100 *aspect/2,farmHeight/200);
		minOSize = 2.5f;

		farmBound = new Vector3(farmWidth/200,farmHeight/200,0) - Camera.main.ViewportToWorldPoint(new Vector3(1,1,0));

		showFarmBackFence (currentPlayer.fenceItemSpec);

		moveToPos (Vector3.zero);

	//	showTiles ();
	}


	//data

	void initEntities(){


		Dictionary<string,GardenData> gardens =	currentPlayer.Gardens;
		GameObject garden = GameManager.getPrefabByName ("Garden");
		foreach(string id in gardens.Keys){
			GameObject g = (GameObject)Instantiate (garden, Vector3.zero, Quaternion.identity,cropLayer.transform);
			g.GetComponent<Garden> ().init (gardens[id]);
			gardenList.Add (id, g);
		}

		GameObject tree = GameManager.getPrefabByName ("TreeEntity");
		foreach(TreeData treeData in currentPlayer.Trees){
			GameObject t = (GameObject)Instantiate (tree, Vector3.zero, Quaternion.identity,cropLayer.transform);
			t.GetComponent<Tree> ().init (treeData);
			treeList.Add (treeData.data_id, t);
		}

		foreach( BuildingData data in currentPlayer.Buildings){
			switch(data.getItemType()){
			case SpecController.HouseType:
				GameObject house = (GameObject)Instantiate (GameManager.getPrefabByName ("Building"),  Vector3.zero, Quaternion.identity,cropLayer.transform);
				house.AddComponent<House> ().init (data);
				houseList.Add (data.data_id, house);
				break;
			case SpecController.DecorationType:
				GameObject building = (GameObject)Instantiate (GameManager.getPrefabByName ("Building"),  Vector3.zero, Quaternion.identity,cropLayer.transform);
				building.AddComponent<Decoration> ().init (data);
				decorationList.Add (data.data_id, building);
				break;
			}
		}


		//pet 
		foreach(PetData petData in currentPlayer.pets){
			GameObject pet =(GameObject)Instantiate (GameManager.getPrefabByName (petData.petItemSpec.name),  Vector3.zero, Quaternion.identity,cropLayer.transform);
			pet.GetComponent<Pet> ().init (petData);
		}


	}
	public void addEntity(int id,GameObject entity,string type){
		switch(type){
		case DataManager.ENTITY_HOUSE:
			if (houseList.ContainsKey (id)) {
				houseList.Remove (id);
			}
			houseList.Add (id, entity);
			break;
		case DataManager.ENTITY_DECORATION:
			if (decorationList.ContainsKey (id)) {
				decorationList.Remove (id);
			}
			decorationList.Add (id, entity);
			break;
		case DataManager.ENTITY_GARDEN:
			if (gardenList.ContainsKey (id.ToString())) {
				gardenList.Remove (id.ToString());
			}
			gardenList.Add (id.ToString(), entity);
			break;
		case DataManager.ENTITY_TREE:
			if (treeList.ContainsKey (id)) {
				treeList.Remove (id);
			}
			treeList.Add (id, entity);
			break;
		}
	}


	public void removeEntity(int id,string type){
		switch(type){
		case DataManager.ENTITY_HOUSE:
			if(houseList.ContainsKey(id)){
				houseList.Remove (id);
			}
			break;
		case DataManager.ENTITY_DECORATION:
			if(decorationList.ContainsKey(id)){
				decorationList.Remove (id);
			}
			break;
		case DataManager.ENTITY_GARDEN:
			if(gardenList.ContainsKey(id.ToString())){
				gardenList.Remove (id.ToString());
			}
			break;
		case DataManager.ENTITY_TREE:
			if(treeList.ContainsKey(id)){
				treeList.Remove (id);
			}
			break;
		}
	}

	// Update is called once per frame

	//鼠标左键
	Vector2 posLeftDown;//
	Vector2 posLeftPre;  
	bool isLeftPressed = false;  
	bool clickEnable = true;  


	//Vector3 mouseBeginPos = Vector3.zero;
	float BeDragNum = 60;
	bool isInfarmMode = false;
	void Update ()
	{
		//infarm
		if(isInfarmMode){
			if (Camera.main.orthographicSize - 0.1f >= 3.5f) {
				setCameraOrithgraphicSize (-0.1f);
			} else {
				isInfarmMode = false;
			}
		}

		//缩放
		if(Input.GetAxis("Mouse ScrollWheel") < 0){
				setCameraOrithgraphicSize (0.5f);
		}else if(Input.GetAxis("Mouse ScrollWheel") > 0){
			setCameraOrithgraphicSize (-0.5f);
		}

		//鼠标左键  
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())  
		{
			isLeftPressed=true;  
			clickEnable = true;  
			posLeftDown = Input.mousePosition;  
			doMouseDownEvent ();
		}

		if (isLeftPressed)  
		{  
			if(((Vector2)Input.mousePosition-posLeftDown).sqrMagnitude>BeDragNum)  
			{  
				clickEnable = false;  
			}  
			if (!clickEnable)  
			{  
				doDragEvent (); 
			}  
		}  

		if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())  
		{  
			isLeftPressed = false;  

			doMouseUpEvent();  
			if(clickEnable)  
			{  
				doClickEvent ();
			}  
		}  




	//缩放
		if (Input.touchCount == 2) {
			
			isLeftPressed = false; 
			Touch touch0 = Input.GetTouch (0);  
			Touch touch1 = Input.GetTouch (1);  

			float _zoomTempDifference;  

			if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began) {  

			
			}  

			if ((touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)) {  

				//缩放  
				_zoomTempDifference = (touch0.position - touch1.position).magnitude;  

				if (_ZoomDifference == 0)
					_ZoomDifference = _zoomTempDifference;  

				setCameraOrithgraphicSize( (_ZoomDifference - _zoomTempDifference) / _ZoomSpeed);  
				_ZoomDifference = _zoomTempDifference;  
			}  

			if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended) {  
				_ZoomDifference = 0; 
				clickEnable = true;
			}  
		}
	}

	void doMouseDownEvent(){
		if (DraggingFunc != null) {
			DraggingFunc ();
		}
	}

	void doClickEvent(){
		if(CurrentMode == DefaultMode){
			Vector3 vec = Camera.main.ScreenToWorldPoint (Input.mousePosition) - FarmWorldPos;
			Tile tile = DataManager.WorldPosToTile (vec.x, vec.y);
			if(tile != null){
				if(tile.entityDic.Count > 0){
					OwnedItem sarr = tile.getTopEntity();
					GameObject target = findEntityGameobject (sarr.count,sarr.item_id);
					if(target != null){
						UI.GetComponent<FarmUIController> ().showFarmToolTip (target,sarr.item_id);
					}
				}
			}
		}

	}
	void doDragEvent(){
		if (DraggingFunc != null) {
			DraggingFunc ();

			float px = 0;
			float py = 0;
			if(Input.mousePosition.x < Screen.width*0.2){
				px = 0.1f;
			}else if(Input.mousePosition.x  > Screen.width*0.8){
				px = -0.1f;
			}
			if(Input.mousePosition.y < Screen.height*0.2){
				py = 0.1f;
			}else if(Input.mousePosition.y  > Screen.height*0.8){
				py = -0.1f;
			}
			if(!(px==0 && py == 0)){
				moveToPos (new Vector2(px,py));
			}
		} else {
			Vector3 offset = Camera.main.ScreenToWorldPoint (Input.mousePosition)- Camera.main.ScreenToWorldPoint (posLeftDown);
			posLeftDown = Input.mousePosition;
			moveToPos (offset);
		}


	}
	void doMouseUpEvent(){
		
	}

	public void onDesignConfirm(){

		if (DesignConfirmFunc != null) {
			DesignConfirmFunc ();
		}
		outDesignMode ();
	}

	public void onDesignCancel(){
		
		if (DesignCancelFunc != null) {
			DesignCancelFunc ();
		}
		outDesignMode ();
	}


	void setCameraOrithgraphicSize(float t){
		float ae = Camera.main.orthographicSize ;
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize +t, minOSize, maxOSize);
		farmBound = new Vector3(farmWidth/200,farmHeight/200,0) - Camera.main.ViewportToWorldPoint(new Vector3(1,1,0));
		moveToPos (Vector3.zero);
	}


	void moveToPos(Vector3 offset){
		
		Vector3 vec = FarmWorldLayer.transform.position + offset;
		Vector3 newOffset = new Vector3 (Mathf.Min(farmBound.x,Mathf.Max(-farmBound.x,vec.x)),Mathf.Min(farmBound.y,Mathf.Max(-farmBound.y,vec.y)),0);

		FarmWorldLayer.transform.position = FarmWorldPos = newOffset;
	}

	public void intoBuyMode(System.Action buyConfirmCall,System.Action buyCancelCall){
		CurrentMode = BUYMode;
		DesignCancelFunc = buyCancelCall;
		DesignConfirmFunc = buyConfirmCall;

		UI.SetActive (false);
		ModeCanvas.SetActive (true);
		DesignPart.SetActive (true);
	}

	public void intoExpandMode(System.Action expandConfirmCall,System.Action expandCancelCall){
		CurrentMode = ExpandMode;
		DesignCancelFunc = expandCancelCall;
		DesignConfirmFunc = expandConfirmCall;

		UI.SetActive (false);
		ModeCanvas.SetActive (true);
		DesignPart.SetActive (true);
	}
	public void intoDraggingMode(System.Action draggingCall,System.Action draggingConfirmCall,System.Action draggingCancelCall){
		CurrentMode = DraggingMode;

		DraggingFunc = draggingCall;
		DesignCancelFunc = draggingCancelCall;
		DesignConfirmFunc = draggingConfirmCall;

		UI.SetActive (false);
		ModeCanvas.SetActive (true);
		DesignPart.SetActive (true);
	}

	public void outDesignMode(){
		CurrentMode = DefaultMode;

		UI.SetActive (true);
		ModeCanvas.SetActive (false);
		DesignPart.SetActive (false);

		DraggingFunc = null;
		DesignConfirmFunc = null;
		DesignCancelFunc = null;
	}

	GameObject findEntityGameobject(int id,string type){
		switch (type) {
		case DataManager.ENTITY_GARDEN:
			if(gardenList.ContainsKey(id.ToString())){
				return gardenList[id.ToString()];
			}
			break;
		case DataManager.ENTITY_DECORATION:
			if(decorationList.ContainsKey(id)){
				return decorationList[id];
			}
			break;
		case DataManager.ENTITY_HOUSE:
			if(houseList.ContainsKey(id)){
				return houseList[id];
			}
			break;
		case DataManager.ENTITY_TREE:
			if(treeList.ContainsKey(id)){
				return treeList[id];
			}
			break;
		}
		return null;
	}
	//showEffect
	public void showHarvestEffect(ItemSpec spec,int count, Vector2 pos){
		Vector3 screenPos = Camera.main.WorldToScreenPoint (pos);
		GameObject hEffect = GameManager.getPrefabByName ("HarvestEffect");
		while(count >0){
			GameObject effect = (GameObject)Instantiate (hEffect, screenPos, Quaternion.identity, effectLayer.transform);
			effect.GetComponent<HarvestEffect> ().init (spec,new Vector3(Screen.width,0,0));
			count--;
		}
			

	}


	//expand
	public void expandFarm(){
		DataManager.expandTiles (currentPlayer.MaxSceneLength);
		initBackground ();
	}
	//changeFence

	DecorationItemSpec lastFenceSpec;
	public void intoChangeFenceMode(DecorationItemSpec spec){
		lastFenceSpec = currentPlayer.fenceItemSpec;
		currentPlayer.changetFence (spec);
		showFarmBackFence (spec);
		intoExpandMode(changeFenceConfirm,changeFenceCancel);
	}
	void changeFenceConfirm(){
		//save

	}

	void changeFenceCancel(){
		currentPlayer.changetFence (lastFenceSpec);
		showFarmBackFence (lastFenceSpec);
	}

	void showFarmBackFence(DecorationItemSpec fenceSpec){

		foreach(GameObject fenceEntity in fenceList){
			Destroy (fenceEntity);
		}
		if(fenceSpec == null){
			return;
		}
		fenceList = new List<GameObject> ();
		int l = fenceSpec.expand_x ;
		int farmL = currentPlayer.MaxSceneLength;
		GameObject newEntity = GameManager.getPrefabByName ("Decoration");
		newEntity.GetComponent<SpriteRenderer> ().sprite = GameManager.getSpByName (fenceSpec.surfaceName);


		int[] boundxARR =  new int[]{ -farmL-1,(farmL) };
		int[] boundyARR =  new int[]{ -farmL-1,(farmL) };

		int nextX = boundxARR[0];
		for (int px = boundxARR[0]; px <=boundxARR[1]; px++) {
			if(px == nextX){
				Vector3 p = DataManager.TileToWorldPos (px, boundyARR[0]);
				p.z =  DataManager.getShowPosz (px,boundyARR[0],l,1) -1;
				GameObject fenceEntity = (GameObject)Instantiate (newEntity, p, Quaternion.identity, landLayer.transform);
				fenceEntity.transform.localScale = new Vector3 (-1,1,1);
				fenceEntity.transform.localPosition = p;
				fenceList.Add (fenceEntity);
				p = DataManager.TileToWorldPos (px, boundyARR[1]);
				p.z = DataManager.getShowPosz (px,boundyARR[1],l,1) +1;
				fenceEntity = (GameObject)Instantiate (newEntity, p, Quaternion.identity, landLayer.transform);
				fenceEntity.transform.localScale = new Vector3 (-1,1,1);
				fenceEntity.transform.localPosition = p;
				fenceList.Add (fenceEntity);
				if (l == 1) {
					nextX = px + l;
				} else {
					if (px + l - 1 >= boundxARR [1]) {
						break;
					} else {
						if(boundxARR[1] - px < (l-1)*2){
							nextX = boundxARR[1] - l+1;
						}else{
							nextX = px + l - 1;
						}
					}
				}

			}
		}

		int nextY = boundyARR[0];
		for (int py = boundyARR[0]; py <= boundyARR[1]; py++) {

			if(l==1 && (py == boundyARR[0] || py == boundyARR[1])){
				nextY = py + 1;
			}
			else if(py == nextY){
				Vector3 p = DataManager.TileToWorldPos (boundxARR[0], py);
				p.z = DataManager.getShowPosz (boundxARR[0],py,1,l)-1;
				GameObject fenceEntity = (GameObject)Instantiate (newEntity, p, Quaternion.identity, landLayer.transform);
				fenceEntity.transform.localPosition = p;
				fenceList.Add (fenceEntity);
				p = DataManager.TileToWorldPos (boundxARR[1], py);
				p.z = DataManager.getShowPosz (boundxARR[1],py,1,l)+1;
				fenceEntity = (GameObject)Instantiate (newEntity, p, Quaternion.identity,landLayer.transform);
				fenceEntity.transform.localPosition = p;
				fenceList.Add (fenceEntity);
				if (l == 1) {
					nextY = py + l;
				} else {
					if (py + l - 1 >= boundyARR [1]) {
						break;
					} else {
						if(boundyARR[1] - py < (l-1)*2){
							nextY = boundyARR[1] - l+1;
						}else{
							nextY = py + l - 1;
						}
					}
				}
			}
		}
	}

	PlayerData currentPlayer{
		get{ 
			return GameManager.CurPlayer;
		}
		set{ 
			GameManager.CurPlayer = value;
		}
	}


	void returnHome(){
		currentPlayer = GameManager.MyPlayData;	

	}


	public void onClickShop(){
		DialogManager.showShopPanel ();
	}

	public void onClickSocial(){
		DialogManager.showFriendPanel ();
	}

	public void onClickWareHouse(){
		DialogManager.showWareHousePanel ();
	}

	public void onClickHome(){
		GameManager.returnHome ();
	}
	public void onClickHelp(){
		
	}
}

