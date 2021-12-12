using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScreen : MonoBehaviour
{
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
