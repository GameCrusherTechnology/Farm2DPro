using UnityEngine;
using System.Collections;

public class FarmUIController : MonoBehaviour
{
	public GameObject farmToolTip;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void showFarmToolTip(GameObject target,string entityType){
		farmToolTip.GetComponent<FarmToolTip> ().init (target, entityType);
		farmToolTip.SetActive (true);
	}
		


}

