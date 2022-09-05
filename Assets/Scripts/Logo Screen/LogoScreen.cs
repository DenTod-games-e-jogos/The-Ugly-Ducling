using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScreen : MonoBehaviour
{
    void Awake()
    {
        Cursor.visible = false;
    }
    
    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);

        ChangeScene();    
    }
    void ChangeScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
