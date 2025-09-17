using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelDeAjustes;

    void Start()
    {
        MostrarPanelPrincipal();
    }

    public void MostrarPanelDeAjustes()
    {
        panelPrincipal.SetActive(false);
        panelDeAjustes.SetActive(true);
    }

    public void MostrarPanelPrincipal()
    {
        panelDeAjustes.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego"); 
    }
}