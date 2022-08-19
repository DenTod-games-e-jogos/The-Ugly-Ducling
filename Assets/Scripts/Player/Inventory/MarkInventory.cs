using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MarkInventory : MonoBehaviour 
{
	public Image KeyInventory;

	KeyCode e = KeyCode.E;

	void Update()
	{
		if(Input.GetKeyDown(e))
		{
			StartCoroutine(ShowKey());
		}

		else if(Input.GetKeyUp(e))
		{
			StopCoroutine(ShowKey());
		}
	}

	IEnumerator ShowKey()
	{
		yield return new WaitForSeconds(5.0f);

		KeyInventory.enabled = true;

		KeyInventory.gameObject.SetActive(true);

		yield return new WaitForSeconds(5.0f);

		KeyInventory.enabled = false;

		KeyInventory.gameObject.SetActive(false);
	}
}