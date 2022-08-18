using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionHouse : MonoBehaviour 
{
	public Text ShowMissionInsideHouse;

	void Start() 
	{
		StartCoroutine(ShowMission());
	}

	IEnumerator ShowMission()
	{
		ShowMissionInsideHouse.text = "Find a key to open the door!";

		yield return new WaitForSeconds(6.0f);

		ShowMissionInsideHouse.enabled = false;
	}
}