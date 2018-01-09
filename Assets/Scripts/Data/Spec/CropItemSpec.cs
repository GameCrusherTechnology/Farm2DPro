using UnityEngine;
using System.Collections;
public class CropItemSpec : ItemSpec
{
	public int growStepTime;
	public int productCount;
	public string productId;
	public int harvestExp;

	public ItemSpec productSpec{
		get{ 
			return SpecController.getItemById (productId);
		}
	}
}

