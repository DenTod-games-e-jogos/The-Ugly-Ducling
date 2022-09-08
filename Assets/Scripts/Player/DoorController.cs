using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour 
{
	KeyCode X = KeyCode.X;

	void Update()
	{
		if(Input.GetKeyDown(X))
		{
			SceneManager.LoadScene("Game 2");
		}
	}
}