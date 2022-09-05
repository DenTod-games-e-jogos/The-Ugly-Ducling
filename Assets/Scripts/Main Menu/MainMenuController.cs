using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Texture2D CursorTexture;

    public CursorMode cursorMode = CursorMode.Auto;

    Vector2 HotSpot = Vector2.zero;

    void Awake()
    {
        Cursor.visible = true;
    }

    void Start()
    {
        Cursor.SetCursor(CursorTexture, HotSpot, cursorMode);
    }

    public void OnNewGameClick()
    {
        SceneManager.LoadScene("Intro");
    }
}