using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelDeAjustes;
    // Si en el futuro tienes más paneles (créditos, selección de nivel), los añades aquí.

    void Start()
    {
        // Al iniciar la escena del menú, nos aseguramos de que solo se vea el panel principal.
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
        // Esta función la usará tu botón de "Regresar".
        panelDeAjustes.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    // Una función extra para el botón de salir del juego.
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para probar en el editor
        Application.Quit();
    }
}