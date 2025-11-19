using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelDeAjustes;
    [SerializeField] private GameObject panelDeControles;
    
    [Header("Scene To Load")]
    public string levelsSceneName = "Levels_";

    void Start()
    {
        MostrarPanelPrincipal();
    }
    public void HandleComenzarButton()
    {
        // 1. Llama al Singleton de Audio (si existe)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        // 2. Llama al Singleton de Escena (si existe)
        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.CambiarEscena(levelsSceneName);
        }
        else
        {
            Debug.LogError("¡SceneChanger no encontrado!");
        }
    }
    public void MostrarPanelDeAjustes()
    {
        AudioManager.Instance.PlayButtonClickSound();

        panelPrincipal.SetActive(false);
        panelDeAjustes.SetActive(true);
    }

    public void MostrarPanelPrincipal()
    {
        AudioManager.Instance.PlayButtonClickSound();

        panelDeAjustes.SetActive(false);
        panelPrincipal.SetActive(true);
        panelDeControles.SetActive(false);

    }
    public void MostrarPanelControles()
    {

        AudioManager.Instance.PlayButtonClickSound();

        panelPrincipal.SetActive(false);

        panelDeControles.SetActive(true);
    }
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego"); 
    }
}