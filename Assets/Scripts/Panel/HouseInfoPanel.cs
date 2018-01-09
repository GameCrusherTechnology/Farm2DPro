using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HouseInfoPanel : MonoBehaviour
{
	public Transform FencePart;
	public Transform PlayerPart;
	public Transform ExpandPart;
	public Transform HousePart;


	House houseEntity;
	BuildingData houseData;
	PlayerData player;
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void init(House house){
		houseEntity = house;
		houseData = house.buildingData;
		player = GameManager.MyPlayData;

		transform.FindChild ("TitleText").GetComponent<Text> ().text = LanController.getString("homestead").ToUpper();

		//player
		PlayerPart.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName( player.charactorIconAddress );
		PlayerPart.FindChild ("Icon").GetComponent<Image> ().SetNativeSize ();

		PlayerPart.FindChild ("LevelText").GetComponent<Text> ().text = player.level.ToString();
		PlayerPart.FindChild ("ExpText").GetComponent<Text> ().text = player.exp.ToString();
		PlayerPart.FindChild ("CoinText").GetComponent<Text> ().text = player.coin.ToString();
		PlayerPart.FindChild ("GemText").GetComponent<Text> ().text = player.gem.ToString();
		PlayerPart.FindChild ("Name").GetComponent<Text> ().text = player.name;

		//fence
		if (player.fenceItemSpec != null) {
			FencePart.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (player.fenceItemSpec.surfaceName);
			FencePart.FindChild ("Icon").GetComponent<Image> ().SetNativeSize ();

			Vector2 vec = FencePart.FindChild ("Icon").GetComponent<RectTransform> ().sizeDelta;
			if (vec.x > 120 || vec.y > 120) {
				float s = Mathf.Min ((float)120/vec.x,(float)120/vec.y);
				FencePart.FindChild ("Icon").GetComponent<RectTransform> ().localScale = new Vector3 (s,s,1);
			};

			FencePart.FindChild ("NameText").GetComponent<Text> ().text = player.fenceItemSpec.itemName;
			FencePart.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = LanController.getString("change");
		}

		//expand
		ExpandPart.FindChild ("Name").GetComponent<Text> ().text = LanController.getString("farmExpand");
		int farmL = player.MaxSceneLength;
		int newFarmL = farmL + 2;
		Transform ePart = ExpandPart.FindChild ("EPart");

		ExpandPart.FindChild ("ContentText").GetComponent<Text> ().text = farmL*2 +"×" + farmL*2;
		if (farmL >= GameManager.MAX_FARM_LENGTH) {
			ePart.gameObject.SetActive (false);
		} else {
			ePart.gameObject.SetActive (true);
			ePart.FindChild ("ContentT").GetComponent<Text> ().text = (newFarmL*2) +"×" + (newFarmL*2);

		}

		//House
		if (houseData.itemSpec != null) {
			HousePart.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (houseData.itemSpec.surfaceName);
			HousePart.FindChild ("Icon").GetComponent<Image> ().SetNativeSize ();
			Vector2 vec = HousePart.FindChild ("Icon").GetComponent<RectTransform> ().sizeDelta;
			if (vec.x > 120 || vec.y > 120) {
				float s = Mathf.Min ((float)120/vec.x,(float)120/vec.y);
				HousePart.FindChild ("Icon").GetComponent<RectTransform> ().localScale = new Vector3 (s,s,1);
			};

			HousePart.FindChild ("NameText").GetComponent<Text> ().text = houseData.itemSpec.itemName;
			HousePart.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = LanController.getString("change");
		}

	}


	public void onClickExpandButton(){
		int expand = player.sceneExpand;
		OwnedItem item = new OwnedItem() ;
		if (item.item_id == "gem") {
			if (GameManager.MyPlayData.gem >= item.count) {
				DialogManager.showMessagePanel (LanController.getString ("confirmExpand"),delegate {
					onExpandConfirmHandle();
				});
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughCoin"));
			}
		} else {
			if (GameManager.MyPlayData.coin >= item.count) {
				DialogManager.showMessagePanel (LanController.getString ("confirmExpand"),onExpandConfirmHandle);
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughCoin"));
			}
		}

	}

	void onExpandConfirmHandle(){
		player.sceneExpand += 2;
		FarmManager.Instance.expandFarm ();
	}

	public void onClickFenceButton(){
		Dictionary<string,ItemSpec> dic = SpecController.getGroup ("Decoration");
		List<ItemSpec> l = new List<ItemSpec> ();
		foreach (string id in dic.Keys) {
			if(dic[id].type == "fence" && id != player.fenceId){
				l.Add (dic[id]);
			}
		}
		DialogManager.showBuyItemPanel (l, null, BuyItemPanel.Type_change_farm_fence);
		onClickOut ();
	}

	public void onClickTitleButton(){
		Dictionary<string,ItemSpec> dic = SpecController.getGroup ("Decoration");
		List<ItemSpec> l = new List<ItemSpec> ();
		foreach (string id in dic.Keys) {
			if(dic[id].type == "fence" && id != player.fenceId){
				l.Add (dic[id]);
			}
		}
		DialogManager.showBuyItemPanel (l, null, BuyItemPanel.Type_change_farm_fence);
		onClickOut ();
	}

	public void onClickOut(){
		Destroy (gameObject);
	}
}

