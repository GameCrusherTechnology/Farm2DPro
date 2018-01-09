using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using DG.Tweening;
public class HarvestEffect : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void init(ItemSpec spec,Vector3 targetPos){

		transform.GetComponent<Image> ().sprite = GameManager.getSpByName (spec.iconAddress);


		transform.DOMoveX (targetPos.x, 1f).OnComplete(delegate {
			Destroy (gameObject);
		});
		transform.DOMoveY (targetPos.y, 1f).SetEase (Ease.InOutQuart);


	}
	private IEnumerator showEffect()
	{
		yield return new WaitForSeconds(5);
		ShowB();
	}

	private void ShowB()
	{
		Destroy (gameObject);
	}
}

