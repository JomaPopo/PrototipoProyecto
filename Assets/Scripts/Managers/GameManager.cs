using UnityEngine;
using TMPro; // Asegúrate de importar TextMeshPro si usas ese tipo de texto

public class GameManager : Singleton<GameManager>
{
    

    [Header("Configuración del Reloj")]
    public float tiempoInicial = 180f; 
    private float tiempoRestante;
    public bool relojActivo = false;

    [Header("Referencias de UI")]
    public GameObject panelDerrota;
    public TextMeshProUGUI textoPanelDerrota; // ¡AÑADE ESTO!
    public GameObject panelVictoria;          // ¡AÑADE ESTO TAMBIÉN!
    public TextMeshProUGUI textoReloj;



    void Start()
    {
        tiempoRestante = tiempoInicial;

        if (panelDerrota != null)
            panelDerrota.SetActive(false); 

        ActualizarTextoReloj(tiempoRestante);
        relojActivo = false;
        if (panelVictoria != null) panelVictoria.SetActive(false);

    }

    void Update()
    {
        // Si el reloj no está activo, no hacemos nada
        if (!relojActivo)
            return;

        // Si el reloj SÍ está activo:
        if (tiempoRestante > 0)
        {
            // Restamos el tiempo
            tiempoRestante -= Time.deltaTime;
            // Actualizamos la UI
            ActualizarTextoReloj(tiempoRestante);
        }
        else
        {
            // El tiempo llegó a 0
            tiempoRestante = 0;
            relojActivo = false; 
            ActualizarTextoReloj(tiempoRestante);

            // ¡Perdiste!
            TriggerDerrota("¡SE ACABÓ EL TIEMPO!");
        }
    }
    public void TriggerVictoria()
    {
        if (relojActivo == false) return; 

        Debug.Log("¡JUEGO GANADO!");
        PausarReloj();
        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        // Detenemos al jugador (PC o VR)
       // PauseManager.Instance.FreeCursorForUI();
    }
    
    void ActualizarTextoReloj(float tiempo)
    {
        if (textoReloj == null) return;
        textoReloj.text = GetFormattedTime();
    }

    
    public void TriggerDerrota(string motivo)
    {
        if (relojActivo == false) return; 

        Debug.LogWarning($"¡JUEGO PERDIDO! Motivo: {motivo}");
        PausarReloj();

        if (panelDerrota != null)
            panelDerrota.SetActive(true);

        if (textoPanelDerrota != null)
            textoPanelDerrota.text = motivo;

       // PauseManager.Instance.FreeCursorForUI();
    }

    public void IniciarReloj()
    {
        relojActivo = true;
    }

    public void PausarReloj()
    {
        relojActivo = false;
    }
    public string GetFormattedTime()
    {
        float tiempo = (tiempoRestante < 0) ? 0 : tiempoRestante;
        float minutos = Mathf.FloorToInt(tiempo / 60);
        float segundos = Mathf.FloorToInt(tiempo % 60);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}