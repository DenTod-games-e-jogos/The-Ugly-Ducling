using UnityEngine;

public class DoorController : MonoBehaviour 
{
	KeyCode e = KeyCode.E;

	[SerializeField]
	MarkInventory inv;

	void Update()
	{
		if(Input.GetKeyDown(e))
		{
			inv.ShowKey();
		}
	}
}