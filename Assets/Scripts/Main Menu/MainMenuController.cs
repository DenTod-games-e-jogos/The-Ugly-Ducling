using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] 
    Canvas mainMenu;
    
    [SerializeField] 
    Canvas optionsMenu;

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

        GoToMainMenu();
    }

    public void OnNewGameClick()
    {
        SceneManager.LoadScene("Intro");
    }

    public void OnOptionsClick()
    {
        /*
        NuovoGioco.text = "Nuovo Gioco";

        CaricaGioco.text = "Carica Gioco";

        EsciDalGioco.text = "Esci Dal Gioco";

        Opzioni.text = "Opzioni";

        if(Opzioni.enabled == true)
        {
            OpzioniDiLingua[0].gameObject.SetActive(true);

            OpzioniDiLingua[0].enabled = true;

            OpzioniDiLingua[0].text = "Italian";

            if (OpzioniDiLingua[0].text == "Italian")
            {
                OpzioniDiLingua[0].text = NuovoGioco.text;
            }
        }

        if(Opzioni.enabled == true)
        {
            OpzioniDiLingua[1].gameObject.SetActive(true);

            OpzioniDiLingua[1].enabled = true;

            OpzioniDiLingua[1].text = "English";

            if (OpzioniDiLingua[1].text == "English")
            {
                OpzioniDiLingua[1].text = NuovoGioco.text;
            }
        }
        */
        GoToOptionsMenu();
    }

    public void OnOptionsReturnsClick()
    {
        GoToMainMenu();
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    void GoToMainMenu()
    {
        mainMenu.enabled = true;

<<<<<<< Updated upstream
        optionsMenu.SetActive(true);
=======
        optionsMenu.enabled = false;
>>>>>>> Stashed changes
    }

    void GoToOptionsMenu()
    {
        optionsMenu.enabled = true;

<<<<<<< Updated upstream
        mainMenu.SetActive(true);
=======
        mainMenu.enabled = false;
>>>>>>> Stashed changes
    }
}