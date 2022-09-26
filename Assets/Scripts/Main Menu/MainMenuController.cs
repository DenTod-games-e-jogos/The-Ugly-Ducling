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

    [SerializeField]
    Text[] OpzioniDiLingua;

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

        if(Opzioni.enabled == true)
        {
            OpzioniDiLingua[0].enabled = true;

            OpzioniDiLingua[0].text = "Italian";

            if (OpzioniDiLingua[0].text == "Italian")
            {
                OpzioniDiLingua[0].text = NuovoGioco.text;
            }
        }

        else if(Opzioni.enabled == true)
        {
            OpzioniDiLingua[1].enabled = true;

            OpzioniDiLingua[1].text = "English";

            if (OpzioniDiLingua[1].text == "English")
            {
                OpzioniDiLingua[1].text = NuovoGioco.text;
            }
        }
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}