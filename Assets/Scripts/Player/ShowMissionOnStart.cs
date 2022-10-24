using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowMissionOnStart : MonoBehaviour
{
    [SerializeField]
    private Text ShowMission;
    IEnumerator Start()
    {
        ShowMission.enabled = false;

        yield return new WaitForSeconds(6.0f);

        ShowMission.enabled = true;

        yield return new WaitForSeconds(6.0f);

        ShowMission.enabled = false;
    }
}
