using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeIntroScene : MonoBehaviour 
{
	void Start () 
	{
		StartCoroutine(StartGame());
	}

	IEnumerator StartGame()
	{
		yield return new WaitForSeconds(1.0f);

		SceneManager.LoadScene("Game 1");
	}
}