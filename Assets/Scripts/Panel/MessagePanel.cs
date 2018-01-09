using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class MessagePanel : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	System.Action confirmHandle;
	public void init(string message,System.Action _confirmHandle,float posy = 0.5f){
		confirmHandle = _confirmHandle;
		transform.FindChild ("Skin").FindChild ("Content").GetComponent<Text> ().text = message;

		if (confirmHandle != null) {
			transform.FindChild ("Skin").FindChild ("YesButton").gameObject.SetActive (true);
			transform.FindChild ("Skin").FindChild ("YesButton").FindChild ("Text").GetComponent<Text> ().text = LanController.getString ("yes").ToUpper();
			transform.FindChild ("Skin").FindChild ("NoButton").gameObject.SetActive (true);
			transform.FindChild ("Skin").FindChild ("NoButton").FindChild ("Text").GetComponent<Text> ().text = LanController.getString ("no").ToUpper();

		} else {
			transform.FindChild ("Skin").FindChild ("ConfirmButton").gameObject.SetActive (true);
			transform.FindChild ("Skin").FindChild ("ConfirmButton").FindChild ("Text").GetComponent<Text> ().text = LanController.getString ("confirm").ToUpper();

		}

		transform.FindChild ("Skin").position = new Vector3 (-Screen.width/2,Screen.height * posy, 0);
		transform.FindChild ("Skin").DOMove(new Vector3(0, Screen.height * posy, 0),0.5f);

		Invoke ("outPanel",2f);
	}

	public void onClickYes(){
		if(confirmHandle != null){
			confirmHandle ();
		}
		outPanel ();
	}
	public void onClickNo(){
		outPanel ();
	}
	public void onClickConfirm(){
		outPanel ();
	}

	void outPanel(){
		CancelInvoke ();
		Destroy (gameObject);
	}

}

