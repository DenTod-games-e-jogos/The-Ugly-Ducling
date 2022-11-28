using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionOutside : MonoBehaviour 
{
	[SerializeField]
	Text OutsideMission;

	void Start() 
	{
		StartCoroutine(ShowMission());
	}

	IEnumerator ShowMission()
	{
		OutsideMission.enabled = false;
		
		yield return new WaitForSeconds(6.0f);

		OutsideMission.enabled = true;
		
		yield return new WaitForSeconds(6.0f);

		OutsideMission.enabled = false;
	}
}