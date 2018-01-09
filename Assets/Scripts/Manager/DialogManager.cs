using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class DialogManager
{
	static GameObject friendPanel;
	public static void showFriendPanel(){
		if(friendPanel == null){
			friendPanel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("FriendPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
		}
		friendPanel.SetActive (true);
		friendPanel.GetComponent<RectTransform>().SetParent (FarmManager.Instance.DialogCanvasTrans);
		friendPanel.transform.localScale = new Vector3 (1,1,1);
		friendPanel.transform.position = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		friendPanel.GetComponent<FriendPanel> ().init ();
	}

	// showPanel

	public static void showRenamePanel(){
		GameObject panel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("FarmRenamePanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
	}

	public static void showMessagePanel(string mes,System.Action okHandle = null,float posy = 0.5f){
		GameObject panel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("MessagePanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
		panel.GetComponent<MessagePanel> ().init (mes, okHandle, posy);
	}

	public static void showShopPanel(){
		GameObject panel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("ShopPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
		panel.GetComponent<RectTransform>().SetParent (FarmManager.Instance.DialogCanvasTrans);
		panel.transform.localScale = new Vector3 (1,1,1);
	}



	public static void showWareHousePanel(){
		GameObject panel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("WareHousePanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
	}

	public static void showGardenInfoPanel(Garden garden){
		GameObject panel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("GardenInfoPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
		panel.GetComponent<GardenInfoPanel> ().init (garden);
	}

	public static void showHouseInfoPanel(House house){
		GameObject panel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("HouseInfoPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
		panel.GetComponent<HouseInfoPanel> ().init (house);
	}

	static GameObject buyItemPanel;
	public static void showBuyItemPanel(List<ItemSpec> itemList,GameObject target = null,string buyType = "itemType"){
		if(buyItemPanel == null){
			buyItemPanel = (GameObject)MonoBehaviour.Instantiate (GameManager.getPrefabByName ("BuyItemPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity);
		}
		buyItemPanel.transform.GetComponent<BuyItemPanel> ().init (itemList,target,buyType);
		buyItemPanel.transform.SetParent (FarmManager.Instance.DialogCanvasTrans);
		buyItemPanel.SetActive (true);
	}

}

