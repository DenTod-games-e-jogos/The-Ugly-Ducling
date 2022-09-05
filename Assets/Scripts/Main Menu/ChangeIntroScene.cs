using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeIntroScene : MonoBehaviour 
{
	void Awake()
	{
		Cursor.visible = false;
	}

	void Start () 
	{
		StartCoroutine(StartGame());
	}

	IEnumerator StartGame()
	{
		yield return new WaitForSeconds(2.0f);

		SceneManager.LoadScene("Game 1");
	}
}