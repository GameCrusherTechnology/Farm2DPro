using UnityEngine;
using System.Collections;

public class DecorationItemSpec : ItemSpec
{
	public int expand_x;
	public int expand_y;

	public string surfaceName;
	public string boardName;
	public string floorId;
	public string fenceId;

	override public string iconAddress
	{
		get{ 
			return surfaceName;
		}
	}
}

