using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class GardenInfoPanel : MonoBehaviour
{
	public Transform FloorPart;
	public Transform CropPart;
	public Transform ExpandPart;
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	string[] expandNameArr = {"ExpandLeftB","ExpandRightB","ExpandLeftT","ExpandRightT" };
	Dictionary<string,OwnedItem> expandCostDic = new Dictionary<string, OwnedItem> ();
	GardenData gardenData;
	Garden garden;
	public void init (Garden  _garden){
		garden = _garden;
		gardenData = garden.gardenData;
		transform.FindChild ("TitleText").GetComponent<Text> ().text = gardenData.gardenItemSpec().itemName;
		transform.FindChild ("DestroyText").GetComponent<Text> ().text = LanController.getString("WarningDeleteGarden");
		transform.FindChild ("DestroyButton").FindChild ("Text").GetComponent<Text> ().text = LanController.getString("delete").ToUpper();

		if (gardenData.cropItemSpec() != null) {
			CropPart.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (gardenData.cropItemSpec().iconAddress);
			CropPart.FindChild ("NameText").GetComponent<Text> ().text = gardenData.cropItemSpec().itemName;
			//CropPart.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = gardenData.cropItemSpec.itemName;
		} else {
			
		}

		if (gardenData.floorItemSpec() != null) {
			FloorPart.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (gardenData.floorItemSpec().surfaceName);
			FloorPart.FindChild ("Icon").GetComponent<Image> ().SetNativeSize ();
			FloorPart.FindChild ("NameText").GetComponent<Text> ().text = gardenData.floorItemSpec().itemName;
			FloorPart.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = LanController.getString("change");
		} else {
			FloorPart.FindChild ("Icon").GetComponent<Image> ().sprite = null;
			FloorPart.FindChild ("NameText").GetComponent<Text> ().text = LanController.getString("Floor");
			FloorPart.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = LanController.getString("buy");
		}

		foreach(string expandName in expandNameArr){
			Transform expand = ExpandPart.FindChild (expandName);

			expand.FindChild ("CountText").GetComponent<Text> ().text =  "+" + fieldCount(expandName).ToString();

			int expandLength = expandTime (expandName);
			int price = 0;
			string priceT = "coin";
			if(expandLength == 1){
				price = 200;
			}else if(expandLength == 2){
				price = 1000;
			}else{
				price = (expandLength-1)*2;
				priceT = "gem";
			}


			if (priceT == "coin") {
				expand.FindChild ("CoinIcon").gameObject.SetActive (true);
			} else {
				expand.FindChild ("GemIcon").gameObject.SetActive (true);
			}
			int cost = (price * fieldCount (expandName));
			expand.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text =cost.ToString();

			OwnedItem costItem = new OwnedItem ();
			costItem.init (priceT,cost);
			expandCostDic.Add (expandName, costItem);
		}
	}

	int fieldCount(string towardName){
		int c = 0;
		switch(towardName){
		case "ExpandLeftB":
			c =gardenData.bound_n - gardenData.bound_y + 1;
			break;
		case "ExpandRightB":
			c =gardenData.bound_m - gardenData.bound_x + 1;
			break;
		case "ExpandLeftT":
			c =gardenData.bound_m - gardenData.bound_x + 1;
			break;
		case "ExpandRightT":
			c =gardenData.bound_n - gardenData.bound_y + 1;
			break;
		}
		return c;
	}

	int expandTime(string towardName){
		int c=0;
		switch(towardName){
		case "ExpandLeftB":
			c =Mathf.Abs( gardenData.bound_x );
			break;
		case "ExpandRightB":
			c =Mathf.Abs( gardenData.bound_y );
			break;
		case "ExpandLeftT":
			c =gardenData.bound_n ;
			break;
		case "ExpandRightT":
			c =gardenData.bound_m ;
			break;
		}
		return c;
	}


	public void onClickExpandButton(GameObject render){
		OwnedItem item = expandCostDic [render.name];
		if (item.item_id == "gem") {
			if (GameManager.MyPlayData.gem >= item.count) {
				garden.expandGarden (render.name,item);
				onClickOut ();
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughCoin"));
			}
		} else {
			if (GameManager.MyPlayData.coin >= item.count) {
				garden.expandGarden (render.name,item);
				onClickOut ();
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughCoin"));
			}
		}

	}

	public void onClickFloorButton(){
		Dictionary<string,ItemSpec> dic = SpecController.getGroup ("Decoration");
		List<ItemSpec> l = new List<ItemSpec> ();
		foreach (string id in dic.Keys) {
			if(dic[id].type == "floor" && id != gardenData.floorId){
				l.Add (dic[id]);
			}
		}
		DialogManager.showBuyItemPanel (l, garden.gameObject, BuyItemPanel.Type_change_garden_floor);
		onClickOut ();
	}

	public void onClickDeleteButton(){

	}

	public void onClickOut(){
		Destroy (gameObject);
	}

}

