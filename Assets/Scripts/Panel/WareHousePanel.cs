using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class WareHousePanel : MonoBehaviour
{
	public const string ItemTab = "Item";


	public GameObject ItemRender;
	public GameObject ItemList;
	public GameObject Tab;
	public Transform playerPart;


	List<OwnedItem> itemList = new List<OwnedItem>();

	string tabName;
	// Use this for initialization
	void Start ()
	{
		transform.FindChild ("TitleText").GetComponent<Text> ().text = LanController.getString ("warehouse").ToUpper();

		Dictionary<string, OwnedItem> itemDic = GameManager.MyPlayData.ownedItems;

		foreach(OwnedItem i in itemDic.Values){
			itemList.Add (i);
		}


		refreshPlayer ();
		onClickTab (Tab.transform.GetChild(0).gameObject);
	}

	// Update is called once per frame
	void Update ()
	{

	}
	void refreshPlayer(){
		playerPart.FindChild("CoinText").GetComponent<Text>().text = GameManager.MyPlayData.coin.ToString();
		playerPart.FindChild("GemText").GetComponent<Text>().text = GameManager.MyPlayData.gem.ToString();

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
		case ItemTab:
			creatItemList(itemList);
			break;
		

		}
	}

	void creatItemList(List<OwnedItem> dic){
		int renderCount = 0;
		foreach (OwnedItem item in dic) {
			if(item.count <=0){
				continue;
			}

			ItemSpec spec = SpecController.getItemById (item.item_id);


			GameObject bt = Instantiate(ItemRender);
			bt.GetComponent<RectTransform>().SetParent(ItemList.transform);
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.name = item.item_id;
			bt.SetActive (true);

			bt.transform.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName (spec.iconAddress);
			bt.transform.FindChild ("NameText").GetComponent<Text> ().text = spec.itemName;
			bt.transform.FindChild ("OwnedCountText").GetComponent<Text> ().text = "×" + item.count.ToString () ;

			renderCount++;
		}
		int c = (renderCount % 2 == 0) ? renderCount / 2 : (renderCount / 2 + 1);
		int t = Mathf.Max(800, 150 * c + (c+1)*10);

		ItemList.GetComponent<RectTransform>().sizeDelta = new Vector2(t, 330);
		ItemList.GetComponent<RectTransform> ().localPosition = new Vector3(Mathf.Max(0,t/2-420) ,0,0);
	}





	public void onClickOut(){
		Destroy (gameObject);
	}
}

