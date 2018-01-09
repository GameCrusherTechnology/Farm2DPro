using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FarmToolTip : MonoBehaviour
{
	public GameObject DragButton;
	public GameObject RotateButton;
	public GameObject DeleteButton;
	public GameObject InfoButton;
	public GameObject SpeedButton;
	public GameObject TimeObject;
	public GameObject SeedButton;
	public GameObject HarvestButton;
	public Text NameText;

	Text timeText;
	string entityType;
	GameObject curTarget;

	// Use this for initialization
	void Start ()
	{
		//Dictionary<string,GameObject
	}
	
	// Update is called once per frame
	int frameCount = 30;
	bool needCheckCrop = false;
	bool needCheckTree = false;
	void Update ()
	{
		if(needCheckCrop || needCheckTree){
			if (frameCount > 0) {
				frameCount--;
			} else {
				frameCount = 30;
				if (needCheckCrop) {
					checkSpeedCrop ();
				} else {
					checkTreeCrop ();
				}
			}
		}
	}


	public void init(GameObject target,string type){
		needCheckCrop = false;
		needCheckTree = false;
		CancelInvoke ();
		curTarget = target;
		entityType = type;
		transform.position = Camera.main.WorldToScreenPoint (target.transform.position);
		timeText = TimeObject.GetComponent<Text> ();
		Transform content = transform.FindChild ("Content");
		for(int i=0;i<content.childCount;i++){
			Destroy (content.GetChild(i).gameObject);
		}

		TimeObject.SetActive(false);
		switch(type){
		case DataManager.ENTITY_GARDEN:
			GameObject infoGarden = (GameObject)Instantiate (InfoButton, Vector3.zero, Quaternion.identity, content);
			infoGarden.SetActive (true);

			GameObject dragGarden = (GameObject)Instantiate (DragButton, Vector3.zero, Quaternion.identity, content);
			dragGarden.SetActive (true);

			//GameObject infoGarden = (GameObject)Instantiate (DragButton, Vector3.zero, Quaternion.identity, content);
			//infoGarden.SetActive (true);
			string cropID = target.GetComponent<Garden> ().CropId;
			if (cropID == null) {
			//seed
				GameObject SeedGarden = (GameObject)Instantiate (SeedButton, Vector3.zero, Quaternion.identity, content);
				SeedGarden.SetActive (true);
			} else {
				if (target.GetComponent<Garden> ().canHavest) {
					//harvest
					GameObject HarvestGarden = (GameObject)Instantiate (HarvestButton, Vector3.zero, Quaternion.identity, content);
					HarvestGarden.SetActive (true);
				} else {
					//speed
					GameObject SpeedGarden = (GameObject)Instantiate (SpeedButton, Vector3.zero, Quaternion.identity, content);
					SpeedGarden.SetActive (true);
					needCheckCrop = true;
					checkSpeedCrop ();
					TimeObject.SetActive(true);
				}
			}
			NameText.text = curTarget.GetComponent<Garden> ().getName ();
			break;
		case DataManager.ENTITY_DECORATION:
			GameObject dragDec = (GameObject)Instantiate (DragButton, Vector3.zero, Quaternion.identity, content);
			dragDec.SetActive (true);

			GameObject rotDec = (GameObject)Instantiate (RotateButton, Vector3.zero, Quaternion.identity, content);
			rotDec.SetActive (true);

			GameObject deleteDec = (GameObject)Instantiate (DeleteButton, Vector3.zero, Quaternion.identity, content);
			deleteDec.SetActive (true);

			NameText.text = target.GetComponent<Decoration> ().getName ();
			break;
		case DataManager.ENTITY_HOUSE:
			GameObject infoHouse = (GameObject)Instantiate (InfoButton, Vector3.zero, Quaternion.identity, content);
			infoHouse.SetActive (true);

			GameObject dragHouse = (GameObject)Instantiate (DragButton, Vector3.zero, Quaternion.identity, content);
			dragHouse.SetActive (true);

			GameObject rotHouse = (GameObject)Instantiate (RotateButton, Vector3.zero, Quaternion.identity, content);
			rotHouse.SetActive (true);

			NameText.text = target.GetComponent<House> ().getName ();
			break;
		case DataManager.ENTITY_TREE:

			string treeID = target.GetComponent<Tree> ().treeId;
			if (treeID == null) {
				//seed
				GameObject SeedTree = (GameObject)Instantiate (SeedButton, Vector3.zero, Quaternion.identity, content);
				SeedTree.SetActive (true);

				GameObject deleteTree = (GameObject)Instantiate (DeleteButton, Vector3.zero, Quaternion.identity, content);
				deleteTree.SetActive (true);
			} else {
				if (target.GetComponent<Tree> ().canHavest) {
					//harvest
					GameObject HarvestTree = (GameObject)Instantiate (HarvestButton, Vector3.zero, Quaternion.identity, content);
					HarvestTree.SetActive (true);
				} else {

					GameObject deleteTree = (GameObject)Instantiate (DeleteButton, Vector3.zero, Quaternion.identity, content);
					deleteTree.SetActive (true);

					//speed
					GameObject SpeedTree = (GameObject)Instantiate (SpeedButton, Vector3.zero, Quaternion.identity, content);
					SpeedTree.SetActive (true);

					needCheckTree = true;
					checkTreeCrop ();

					TimeObject.SetActive(true);
				}
			}
			GameObject dragTree = (GameObject)Instantiate (DragButton, Vector3.zero, Quaternion.identity, content);
			dragTree.SetActive (true);
			NameText.text = curTarget.GetComponent<Tree> ().getName ();
			break;
		}


		Invoke ("hide", 2f);
	}
		

	void checkSpeedCrop(){
		int leftTime = curTarget.GetComponent<Garden> ().LeftTime;
		if (leftTime > 0) {
			timeText.text = TimeManager.getTimeLeftString (leftTime);
		} else if(curTarget.GetComponent<Garden> ().canHavest){
			init (curTarget, DataManager.ENTITY_GARDEN);
		}
	}
	void checkTreeCrop(){
		int leftTime = curTarget.GetComponent<Tree> ().LeftTime;
		if (leftTime > 0) {
			timeText.text = TimeManager.getTimeLeftString (leftTime);
		} else if(curTarget.GetComponent<Tree> ().canHavest){
			init (curTarget, DataManager.ENTITY_TREE);
		}
	}

	public void onDragClick(){
		GameObject boardT = (GameObject)Instantiate (GameManager.getPrefabByName ("BoardTile"), Vector3.zero, Quaternion.identity, curTarget.transform);
		boardT.name = "BoardTile";

		hide ();
		switch(entityType){
		case DataManager.ENTITY_HOUSE:
			curTarget.GetComponent<House> ().intoDrag (boardT);
			break;
		case DataManager.ENTITY_DECORATION:
			curTarget.GetComponent<Decoration> ().intoDrag (boardT);
			break;
		case DataManager.ENTITY_GARDEN:
			curTarget.GetComponent<Garden> ().intoDrag (boardT);
			break;
		case DataManager.ENTITY_TREE:
			curTarget.GetComponent<Tree> ().intoDrag (boardT);
			break;
		}
	}

	public void onRotateClick(){
		
		GameObject boardT = (GameObject)Instantiate (GameManager.getPrefabByName ("BoardTile"), Vector3.zero, Quaternion.identity, curTarget.transform);
		boardT.name = "BoardTile";

		hide ();
		switch(entityType){
		case DataManager.ENTITY_HOUSE:
			curTarget.GetComponent<House> ().intoRotation (boardT);
			break;
		case DataManager.ENTITY_DECORATION:
			curTarget.GetComponent<Decoration> ().intoRotation (boardT);
			break;
		}

	}

	public void onDeleteClick(){
		DialogManager.showMessagePanel (LanController.getString("warningDelete") ,delegate {
			switch(entityType){

			case DataManager.ENTITY_DECORATION:
				curTarget.GetComponent<Decoration> ().delete ();
				break;

			case DataManager.ENTITY_TREE:
				curTarget.GetComponent<Tree> ().delete();
				break;
			}
		});
		hide ();
	}
	public void onInfoButtonClick(){
		
		switch(entityType){
		case DataManager.ENTITY_GARDEN:
			DialogManager.showGardenInfoPanel (curTarget.GetComponent<Garden>());
			break;
		case DataManager.ENTITY_HOUSE:
			DialogManager.showHouseInfoPanel (curTarget.GetComponent<House>());
			break;
		}

		hide ();
	}

	public void onSeedClick(){
		if (curTarget.GetComponent<Garden> () != null) {
			DialogManager.showBuyItemPanel (null, curTarget, BuyItemPanel.Type_Buy_Seed_Garden);
		} else if (curTarget.GetComponent<Tree> () != null){
			DialogManager.showBuyItemPanel (null, curTarget, BuyItemPanel.Type_Buy_Seed_Tree);
		}
		hide ();
	}
	public void onSpeedClick(){
		if (curTarget.GetComponent<Garden> () != null) {
			curTarget.GetComponent<Garden> ().speedCrop ();
		} else if (curTarget.GetComponent<Tree> () != null){
			curTarget.GetComponent<Tree> ().speedTree ();
		}

		hide ();
	}
	public void onHarvestClick(){
		if (curTarget.GetComponent<Garden> () != null) {
			curTarget.GetComponent<Garden> ().harvestCrop ();
		} else if (curTarget.GetComponent<Tree> () != null){
			curTarget.GetComponent<Tree> ().harvestTree ();
		}
		hide ();
	}
	void hide(){
		gameObject.SetActive (false);
	}

}

