using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Texture2D CursorTexture;

    public CursorMode CursMode = CursorMode.Auto;

    Vector2 HotSpot = Vector2.zero;

    [SerializeField]
    Text NuovoGioco;

    [SerializeField]
    Text CaricaGioco;

    [SerializeField]
    Text EsciDalGioco;

    [SerializeField]
    Text Opzioni;

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
        NuovoGioco.text = "Nuovo Gioco";

        CaricaGioco.text = "Carica Gioco";

        EsciDalGioco.text = "Esci Dal Gioco";

        Opzioni.text = "Opzioni";
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}