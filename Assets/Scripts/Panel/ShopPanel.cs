using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class ShopPanel : MonoBehaviour
{
	public const string TreasureTab = "Treasure";
	public const string HouseTab = "House";


	public GameObject ItemRender;
	public GameObject ItemList;
	public GameObject Tab;
	public Transform playerPart;

	Dictionary<string,TreasureData> treasureDic;
	List<ItemSpec> houseItemList = new List<ItemSpec>();

	string tabName = "House";
	// Use this for initialization
	void Start ()
	{
		transform.FindChild ("TitleText").GetComponent<Text> ().text = LanController.getString ("shop").ToUpper();

		int playerLevel = curPlayer.level;
		treasureDic = GameManager.Treasures;


		Dictionary<string,ItemSpec> houseItemDic = SpecController.getGroup ("Building");

		foreach(ItemSpec spec in houseItemDic.Values){
			if(spec.showInShop() &&( spec.level  == -1 ||  playerLevel >= spec.level)){
				houseItemList.Add (spec);
			}
		}


		refreshPlayer ();
		onClickTab (Tab.transform.GetChild(0).gameObject);
		showItemList ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	void refreshPlayer(){
		playerPart.FindChild("CoinText").GetComponent<Text>().text = curPlayer.coin.ToString();
		playerPart.FindChild("GemText").GetComponent<Text>().text = curPlayer.gem.ToString();
		playerPart.FindChild("ExpText").GetComponent<Text>().text = curPlayer.exp.ToString();
		playerPart.FindChild("LevelText").GetComponent<Text>().text = curPlayer.level.ToString();
	}

	GameObject lastTabBut;
	public void onClickTab(GameObject tabBut){
		if(lastTabBut != tabBut){
			if(lastTabBut != null){
				lastTabBut.transform.localScale = new Vector3 (1,1,1);
				lastTabBut.transform.FindChild ("SelectedSkin").gameObject.SetActive (false);
				lastTabBut.transform.FindChild ("UnSelectedSkin").gameObject.SetActive (true);
			}


			tabBut.transform.localScale = new Vector3 (1.2f, 1.2f, 0);
			tabBut.transform.FindChild ("SelectedSkin").gameObject.SetActive (true);
			tabBut.transform.FindChild ("UnSelectedSkin").gameObject.SetActive (false);
			lastTabBut = tabBut;
			tabName = tabBut.name;
			showItemList ();
		}
	}
		
	void showItemList(){
		Dictionary<string,ItemSpec> dic = new Dictionary<string, ItemSpec>();

		for (int i = 0; i < ItemList.transform.childCount; i++) {
			GameObject go = ItemList.transform.GetChild (i).gameObject;
			Destroy (go);
		}

		switch(tabName){
		case TreasureTab:
			creatTreasureItemList(treasureDic);
			break;
		case HouseTab:
			creatItemList (houseItemList);
			break;

		}
	}

	void creatItemList(List<ItemSpec> dic){
		int renderCount = 0;
		foreach (ItemSpec spec in dic) {
			int ownC = 0;
			if(spec.maxCount >0){
				if (spec.type == DataManager.ENTITY_GARDEN) {
					ownC = curPlayer.getGardenCount (spec.item_id);
					if(ownC >= spec.maxCount){
						break;
					}

				} else if(spec.type == DataManager.ENTITY_TREE){
					ownC = curPlayer.getTreeBoardCount (spec.item_id);
					if(ownC >= spec.maxCount){
						break;
					}
					break;
				}

			}


			GameObject bt = Instantiate(ItemRender);
			bt.GetComponent<RectTransform>().SetParent(ItemList.transform);
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.name = spec.item_id;
			bt.SetActive (true);

			bt.transform.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (spec.iconAddress);
			bt.transform.FindChild ("NameText").GetComponent<Text> ().text = spec.itemName;
			bt.transform.FindChild ("MessageText").GetComponent<Text> ().text = spec.itemInfo;

			if (spec.coinPrice > 0) {
				bt.transform.FindChild ("Button").FindChild ("CostCoinIcon").gameObject.SetActive (true);
				bt.transform.FindChild ("Button").FindChild ("CostText").GetComponent<Text> ().text = spec.coinPrice.ToString();
			} else {
				bt.transform.FindChild ("Button").FindChild ("CostGemIcon").gameObject.SetActive (true);
				bt.transform.FindChild ("Button").FindChild ("CostText").GetComponent<Text> ().text = spec.gemPrice.ToString();
			}

			if (spec.maxCount > 0) {
				bt.transform.FindChild ("OwnedCountText").GetComponent<Text> ().text = ownC.ToString () +" /" + spec.maxCount.ToString ();
			}
		
			renderCount++;
		}

		int t = 200 * renderCount + (renderCount+1)*10;
		ItemList.GetComponent<RectTransform>().sizeDelta = new Vector2(t, 300);
		ItemList.GetComponent<RectTransform> ().localPosition = new Vector3(Mathf.Max(0,t/2-420) ,0,0);
	}

	void creatTreasureItemList(Dictionary<string,TreasureData> dic){
		foreach (TreasureData data in dic.Values) {

			GameObject bt = Instantiate(ItemRender);
			bt.GetComponent<RectTransform>().SetParent(ItemList.transform);
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.name = data.name;
			bt.SetActive (true);

			bt.transform.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (data.TreasureIcon);
			bt.transform.FindChild ("NameText").GetComponent<Text> ().text = data.TreasureName;
			bt.transform.FindChild ("MessageText").GetComponent<Text> ().text = data.treasureMessage;
			bt.transform.FindChild ("Button").FindChild ("CostText").GetComponent<Text> ().text = data.moneyCount.ToString();
			bt.transform.FindChild ("OwnedCountText").GetComponent<Text> ().text = data.getCount.ToString ();

		
		}

		int t = 200 * dic.Values.Count + (dic.Values.Count+1)*10 ;
		ItemList.GetComponent<RectTransform>().sizeDelta = new Vector2(t, 300);
		ItemList.GetComponent<RectTransform> ().localPosition = new Vector3(Mathf.Max(0,t/2-420) ,0,0);
	}

	public void onClickTreasureItem(TreasureData data){
		
	}
	public void onClickBuyItem(GameObject render){
		Debug.Log (render.name);
		ItemSpec spec = SpecController.getItemById (render.name);
		if (spec.coinPrice > 0) {
			if (curPlayer.coin >=  spec.coinPrice) {
				toBuyItem (spec);
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughCoin"));
			}
		} else {
			if (curPlayer.gem >=  spec.gemPrice) {
				toBuyItem (spec);
			} else {
				DialogManager.showMessagePanel (LanController.getString ("noEnoughGem"));
			}
		}
	}

	void toBuyItem(ItemSpec spec){
		switch(spec.type){
		case "Garden":
			int maxId = 0;
			foreach (GardenData gdata in curPlayer.Gardens.Values) {
				maxId = Mathf.Max (maxId,  gdata.data_id);
			}
			maxId++;
			GardenData data = new GardenData ();
			data.creatGarden (maxId,spec.item_id, 0, 0);

			GameObject garden = (GameObject)Instantiate (GameManager.getPrefabByName ("Garden"), Vector3.zero, Quaternion.identity,FarmManager.Instance.cropLayer.transform);
			garden.GetComponent<Garden> ().intoBuyMode (data);
			break;
		case "TreeBoard":
			
			int maxTreeId = 0;
			foreach (TreeData treeD in curPlayer.Trees) {
				maxTreeId = Mathf.Max (maxTreeId, treeD.data_id);
			}
			maxTreeId++;
			TreeData newTreeData = new TreeData ();
			newTreeData.creatTreeBoard (maxTreeId,spec.item_id, 0, 0);

			GameObject treeEntity = (GameObject)Instantiate (GameManager.getPrefabByName ("TreeEntity"), Vector3.zero, Quaternion.identity,FarmManager.Instance.cropLayer.transform);
			treeEntity.GetComponent<Tree> ().intoBuyMode (newTreeData);
			break;
		}
		onClickOut ();
	}

	PlayerData curPlayer{
		get{ 
			return GameManager.MyPlayData;
		}
	}
	public void onClickOut(){
		Destroy (gameObject);
	}
}

