using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Texture2D CursorTexture;

    public CursorMode CursMode = CursorMode.Auto;

    Vector2 HotSpot = Vector2.zero;

    void Awake()
    {
        Cursor.visible = true;
    }

    void Start()
    {
        Cursor.SetCursor(CursorTexture, HotSpot, CursMode);
    }

    public void OnNewGameClick()
    {
        SceneManager.LoadScene("Intro");
    }

    public void OnConfigurationClick()
    {

    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}