using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MarkInventory : MonoBehaviour 
{
	public Image KeyInventory;

	[SerializeField]
	GameObject Key;

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

	public IEnumerator ShowKey()
	{
		yield return new WaitForSeconds(10.0f);

		Key.SetActive(false);

		yield return new WaitForSeconds(5.0f);

		KeyInventory.enabled = true;

		KeyInventory.gameObject.SetActive(true);

		yield return new WaitForSeconds(5.0f);

		KeyInventory.enabled = false;

		KeyInventory.gameObject.SetActive(false);

		yield return new WaitForSeconds(7.0f);

		SceneManager.LoadScene("Game 2");
	}
}