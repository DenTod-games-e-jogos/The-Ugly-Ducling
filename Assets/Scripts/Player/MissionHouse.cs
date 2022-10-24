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
		ShowMissionInsideHouse.enabled = false;
		
		yield return new WaitForSeconds(6.0f);

		ShowMissionInsideHouse.enabled = true;
		
		yield return new WaitForSeconds(6.0f);

		ShowMissionInsideHouse.enabled = false;
	}
}