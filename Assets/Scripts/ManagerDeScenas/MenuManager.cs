using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Men�")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelDeAjustes;
    // Si en el futuro tienes m�s paneles (cr�ditos, selecci�n de nivel), los a�ades aqu�.

    void Start()
    {
        // Al iniciar la escena del men�, nos aseguramos de que solo se vea el panel principal.
        MostrarPanelPrincipal();
    }

    public void MostrarPanelDeAjustes()
    {
        // Apagamos el panel principal y encendemos el de ajustes.
        panelPrincipal.SetActive(false);
        panelDeAjustes.SetActive(true);
    }

    public void MostrarPanelPrincipal()
    {
        // Apagamos el panel de ajustes y encendemos el principal.
        // Esta funci�n la usar� tu bot�n de "Regresar".
        panelDeAjustes.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    // Una funci�n extra para el bot�n de salir del juego.
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para probar en el editor
        Application.Quit();
    }
}