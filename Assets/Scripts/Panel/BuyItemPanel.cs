using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuyItemPanel : MonoBehaviour
{
	public GameObject Render;
	public RectTransform listGrid;

	public const string Type_item = "itemType";
	public const string Type_change_garden_floor = "change_garden_floor";
	public const string Type_change_farm_fence = "change_farm_fence";
	public const string Type_Buy_Seed_Garden = "buy_seed_for_garden";
	public const string Type_Buy_Seed_Tree = "buy_seed_for_tree";
	GameObject curTarget;
	string curBuyType;

	Garden curGarden;
	int gardenCropCount;

	Tree curTree;
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void init(List<ItemSpec> itemList,GameObject target = null,string buyType = "itemType"){

		for(int i=0;i<listGrid.childCount;i++){
			Destroy (listGrid.GetChild (i).gameObject);
		}
			
		curTarget = target;
		curBuyType = buyType;

		if(buyType == Type_Buy_Seed_Garden){
			curGarden = target.GetComponent<Garden> ();
			itemList = cropSeedList;
			gardenCropCount = curGarden.gardenData.cropCount();
		}else if(buyType == Type_Buy_Seed_Tree){
			curTree = target.GetComponent<Tree> ();
			itemList = treeSeedList;
		}
		Dictionary<string,OwnedItem> ownedDic = curPlayer.ownedItems;



		foreach(ItemSpec spec in itemList){
			int ownedC = 0;
			GameObject r = (GameObject)Instantiate (Render, Vector3.zero, Quaternion.identity, listGrid);
			r.GetComponent<RectTransform>().localScale = Vector3.one;
			r.SetActive (true);
			r.name = spec.item_id;

			r.transform.FindChild ("NameText").GetComponent<Text> ().text = spec.itemName;
			r.transform.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (spec.iconAddress);
			r.transform.FindChild ("Icon").GetComponent<Image> ().SetNativeSize ();
			Vector2 vec = r.transform.FindChild ("Icon").GetComponent<RectTransform> ().sizeDelta;
			if (vec.x > 120) {
				r.transform.FindChild ("Icon").GetComponent<RectTransform> ().localScale = new Vector3 ((float)120/vec.x,(float)120/vec.x,1);
			}

			if (buyType == Type_Buy_Seed_Garden) {
				r.transform.FindChild ("SeedIcon").gameObject.SetActive (true);
				if (ownedDic.ContainsKey (spec.item_id)) {
					ownedC = ownedDic [spec.item_id].count;
					if (ownedC > 0) {
						r.transform.FindChild ("CountText").GetComponent<Text> ().text = "×" + ownedC.ToString ();
					}
				}


				if (spec.coinPrice > 0) {
					r.transform.FindChild ("CoinIcon").gameObject.SetActive (true);
					r.transform.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = spec.coinPrice.ToString ()+ " ×" + Mathf.Max(0,gardenCropCount - ownedC);
				} else {
					r.transform.FindChild ("GemIcon").gameObject.SetActive (true);
					r.transform.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = spec.gemPrice.ToString ()+ " ×" + Mathf.Max(0,gardenCropCount - ownedC);
				}
			}else if (buyType == Type_Buy_Seed_Tree) {
				r.transform.FindChild ("SeedIcon").gameObject.SetActive (true);
				if (ownedDic.ContainsKey (spec.item_id)) {
					ownedC = ownedDic [spec.item_id].count;
					if (ownedC > 0) {
						r.transform.FindChild ("CountText").GetComponent<Text> ().text = "×" + ownedC.ToString ();
					}
				}

				if (spec.coinPrice > 0) {
					r.transform.FindChild ("CoinIcon").gameObject.SetActive (true);
					r.transform.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = spec.coinPrice.ToString ();
				} else {
					r.transform.FindChild ("GemIcon").gameObject.SetActive (true);
					r.transform.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = spec.gemPrice.ToString ();
				}
			} else {
				if (spec.coinPrice > 0) {
					r.transform.FindChild ("CoinIcon").gameObject.SetActive (true);
					r.transform.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = spec.coinPrice.ToString ();
				} else {
					r.transform.FindChild ("GemIcon").gameObject.SetActive (true);
					r.transform.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = spec.gemPrice.ToString ();
				}
			}
		

		
		}

		float w = itemList.Count * 110 + 10 * (itemList.Count + 1);
		listGrid.sizeDelta = new Vector2 (w,110);
		listGrid.localPosition = new Vector3(w/2 - 250 ,6,0);

		CancelInvoke ();
		Invoke ("hide", 3f);
	}

	List<ItemSpec> cropSeedList{
		get{ 
			int playerLevel = curPlayer.level;
			Debug.Log (playerLevel);
			Dictionary<string,ItemSpec> dic = SpecController.getGroup ("Crop");
			List<ItemSpec> list = new List<ItemSpec> ();
			CropItemSpec cropSpec;
			foreach (string str in dic.Keys) {
				cropSpec = dic [str] as CropItemSpec;
				if (str == curGarden.gardenData.last_id) {
					list.Insert (0, cropSpec);
				}
				if(cropSpec.level <= playerLevel  && cropSpec.type == "Crop"){
					list.Add(dic[str]);
				}
			}
			return list;

		}
	}
	List<ItemSpec> treeSeedList{
		get{ 
			int playerLevel = curPlayer.level;
			Debug.Log (playerLevel);
			Dictionary<string,ItemSpec> dic = SpecController.getGroup ("Crop");
			CropItemSpec cropSpec;
			List<ItemSpec> list = new List<ItemSpec> ();
			foreach (string str in dic.Keys) {
				cropSpec = dic [str] as CropItemSpec;

				if(cropSpec.level <= playerLevel && cropSpec.type == "Tree"){
					list.Add(dic[str]);
				}
			}
			return list;

		}
	}

	public void onClickBuy(GameObject render){
		
		ItemSpec spec = SpecController.getItemById (render.name);
		OwnedItem item = curPlayer.getOwnedItem(spec.item_id);

		if (spec.coinPrice > 0) {
			int cost = (curBuyType == Type_Buy_Seed_Garden)? Mathf.Max(0, gardenCropCount - item.count)*spec.coinPrice:spec.coinPrice;
			if (curPlayer.coin >=  cost) {
				toBuyItem (spec);
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughCoin"));
				CancelInvoke ();
				Invoke ("hide", 3f);
			}
		} else {
			int gemcost = (curBuyType == Type_Buy_Seed_Garden)? Mathf.Max(0, gardenCropCount - item.count)*spec.gemPrice:spec.gemPrice;
			if (curPlayer.gem >=  gemcost) {
				toBuyItem (spec);
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughGem"));
				CancelInvoke ();
				Invoke ("hide", 3f);
			}
		}
	}

	void toBuyItem(ItemSpec spec){
		OwnedItem item;
		switch(curBuyType){
		case Type_item:
			
			break;
		case Type_change_garden_floor:
			if(curTarget!= null && curTarget.GetComponent<Garden>() != null){
				curTarget.GetComponent<Garden> ().intoChangeSurfaceMode (spec as DecorationItemSpec);
			}
			break;
		case Type_change_farm_fence:
			FarmManager.Instance.intoChangeFenceMode (spec as DecorationItemSpec);
			break;
		case Type_Buy_Seed_Garden:
			curGarden.plantCrop (spec.item_id);

			item = curPlayer.getOwnedItem (spec.item_id);
			if (item.count >= gardenCropCount) {
				curPlayer.reduceOwnedItem (spec.item_id, gardenCropCount);
			} else {
				curPlayer.reduceOwnedItem (spec.item_id, item.count);
				int needCostC =  (gardenCropCount - item.count);
				if (spec.coinPrice > 0) {
					curPlayer.coin -= spec.coinPrice * needCostC;
				} else {
					curPlayer.gem -= spec.gemPrice * needCostC;
				}
			}
			break;
		case Type_Buy_Seed_Tree:
			curTree.plantTree (spec.item_id);

			item = curPlayer.getOwnedItem (spec.item_id);
			if (item.count >= 1) {
				curPlayer.reduceOwnedItem (spec.item_id, 1);
			} else {
				if (spec.coinPrice > 0) {
					curPlayer.coin -= spec.coinPrice ;
				} else {
					curPlayer.gem -= spec.gemPrice ;
				}
			}
			break;
		}
		onClickOut ();
	}


	public void onScollChanged(Vector2 vec){
		CancelInvoke ();
		Invoke ("hide", 3f);
	}

	PlayerData curPlayer{
		get{
			return GameManager.MyPlayData;
		}
	}
	public void onClickOut(){
		hide();
	}
	void hide(){
		CancelInvoke ();
		gameObject.SetActive (false);
	}
}

