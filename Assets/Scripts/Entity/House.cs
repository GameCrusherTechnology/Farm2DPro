using UnityEngine;
using System.Collections;

public class House : Decoration
{
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}





	override public string entityType{
		get{ 
			return DataManager.ENTITY_HOUSE;
		}
	}
}

