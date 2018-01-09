using UnityEngine;
using System.Collections;

public class Tree : Decoration
{
	int maxStep =5;
	int curStep = 0;

	// Use this for initialization
	void Start ()
	{
	
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

	void validateSurface(){
		if (treeData.treeId != null) {
			CropItemSpec spec = SpecController.getItemById (treeData.treeId) as CropItemSpec;
			Sprite sp = GameManager.getSpByName ("Texture/tree/" + spec.name + "000" + curStep);
			transform.FindChild ("Surface").GetComponent<SpriteRenderer> ().sprite = sp;
		} else {
			transform.FindChild ("Surface").GetComponent<SpriteRenderer> ().sprite = null;
		}
	}



	void checkCurStep(){
		int lTime = (TimeManager.CurrentSystemNum - treeData.plantTime);
		int stepT =(int) treeData.treeItemSpec.growStepTime;
		Debug.Log (lTime);
		int newStep = curStep;
		while(newStep <= maxStep){
			if (lTime >= stepT * newStep) {
				newStep++;
			} else {
				break;
			}
		}

		if(newStep != curStep){
			curStep = newStep;
			validateSurface ();
		}
	}

	//operate
	public void plantTree(string id){

		treeData.treeId = id;
		treeData.plantTime = TimeManager.CurrentSystemNum;
		curStep = 0;

		checkCurStep ();
	}
	public void harvestTree(){
		//存储
		OwnedItem harvestCrop = new OwnedItem();
		harvestCrop.item_id = treeData.treeItemSpec.productId;
		harvestCrop.count =	 treeData.treeItemSpec.productCount;
		curPlayer.addOwnedItem (harvestCrop);


		FarmManager.Instance.showHarvestEffect (treeData.treeItemSpec.productSpec, harvestCrop.count, transform.position);

		curStep = 0;
		treeData.plantTime = TimeManager.CurrentSystemNum;
		checkCurStep ();
	}
	public void speedTree(){
		treeData.plantTime -= 100;

		checkCurStep ();
	}


	override public void delete(){
		if (treeId != null) {
			treeData.treeId = null;
			curStep = 0;
			checkCurStep ();

		} else {
			curPlayer.removeTree (treeData.data_id);

			cleanTiles();
			FarmManager.Instance.removeEntity (buildingData.data_id,entityType);
			Destroy (gameObject);
		}
	}

	public bool canHavest{
		get{ 
			return treeId!= null && curStep >= maxStep;
		}
	}

	public string treeId{
		get{ 
			return treeData.treeId;
		}
	}

	public TreeData treeData{
		get{
			return buildingData as TreeData;
		}
	}

	public int LeftTime{
		get{ 
			if (treeData.treeItemSpec != null) {
				return (int)Mathf.Max (0, treeData.treeItemSpec.growStepTime * (maxStep-1) -( TimeManager.CurrentSystemNum - treeData.plantTime ));
			} else {
				return 0;
			}

		}
	}

	override public string getName(){
		if (treeData.treeItemSpec != null) {
			return treeData.treeItemSpec.itemName;
		} else {
			return treeData.itemSpec.itemName;
		}
	}
	override public string entityType{
		get{ 
			return DataManager.ENTITY_TREE;
		}
	}

}

