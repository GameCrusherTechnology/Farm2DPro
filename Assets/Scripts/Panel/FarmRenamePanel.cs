using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FarmRenamePanel : MonoBehaviour
{

	// Use this for initialization
	PlayerData myPlayerData;
	bool isNewPlayer = false;
	InputField inputText;
	int renameCost = 10;
	void Start ()
	{
		transform.FindChild ("Title").FindChild ("Text").GetComponent<Text> ().text = LanController.getString ("welcome").ToUpper();
		transform.FindChild ("Message").GetComponent<Text> ().text = LanController.getString("warningName");
		inputText = transform.FindChild ("NamePart").FindChild ("InputField").GetComponent<InputField> ();
		myPlayerData = GameManager.MyPlayData;

	
		Transform grid = transform.FindChild ("CharactorView").FindChild ("Viewport").FindChild ("Content");

		/*string[] charators = DataManager.CharactorList;
		foreach(string name in charators){
			GameObject bt = Instantiate(render);
			bt.GetComponent<RectTransform>().SetParent(grid.GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.name = name;
			bt.SetActive (true);
			bt.transform.FindChild ("Icon").GetComponent<Image> ().sprite = GameManager.getSpByName ("");
		}
		int t = charators.Length;
		float w = t * 80 +(t+1)*20;
		transform.FindChild ("CharactorView").GetComponent<RectTransform>().sizeDelta = new Vector2(w, 128);
		grid.transform.Translate (new Vector3(-w/2,0,0));
*/

		if (myPlayerData.name == null) {
			isNewPlayer = true;
			transform.FindChild ("Button").gameObject.SetActive (true);
			transform.FindChild ("BuyButton").gameObject.SetActive (false);
			transform.FindChild ("Message").GetComponent<Text> ().text = LanController.getString("warningName");
			transform.FindChild ("Button").FindChild ("Text").GetComponent<Text> ().text = LanController.getString ("confirm");
			transform.FindChild ("CancelButton").gameObject.SetActive (false);

		} else {
			transform.FindChild ("Button").gameObject.SetActive (false);
			transform.FindChild ("BuyButton").gameObject.SetActive (true);
			transform.FindChild ("BuyButton").FindChild ("Text").GetComponent<Text> ().text = "×" + renameCost;
			transform.FindChild ("Message").GetComponent<Text> ().text = LanController.getString ("warningReName") ;

		}

		string character = "Girl";
		if(myPlayerData.charactor != null){
			character = myPlayerData.charactor;
		}
		Transform t = grid.FindChild (character);
		if(t != null){
			t.GetComponent<Button> ().Select ();
			t.FindChild ("SelectedIcon").gameObject.SetActive (true);
			lastRender = t;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	Transform lastRender;
	public void onCharacterSelected(GameObject characterRender){
		if(lastRender != characterRender.transform){
			lastRender.FindChild ("SelectedIcon").gameObject.SetActive (false);
			lastRender = characterRender.transform;
			lastRender.FindChild ("SelectedIcon").gameObject.SetActive (true);
		}
	}
	public void onConfirmClick(){
		//保存
		if (inputText.text != "") {
			GameManager.MyPlayData.name = inputText.text;
			GameManager.MyPlayData.charactor = lastRender.name;
			outPanel ();
			FarmManager.Instance.intoFarm ();
		} else {
			DialogManager.showMessagePanel (LanController.getString("warningNoName"));
		}

	
	}

	public void onBuyClick(){
		//购买

	}

	public void outPanel(){
		Destroy (gameObject);
	}



}

