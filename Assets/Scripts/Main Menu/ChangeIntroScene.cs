using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeIntroScene : MonoBehaviour 
{
	void Start () 
	{
		StartCoroutine(StartGame());
	}

	IEnumerator StartGame()
	{
		yield return new WaitForSeconds(1.0f);
	}
}