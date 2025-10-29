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
    public TextMeshProUGUI textoReloj; 

  

    void Start()
    {
        tiempoRestante = tiempoInicial;

        if (panelDerrota != null)
            panelDerrota.SetActive(false); 

        ActualizarTextoReloj(tiempoRestante); // Mostramos el tiempo inicial
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
            relojActivo = false; // Lo apagamos
            ActualizarTextoReloj(tiempoRestante);

            // ¡Perdiste!
            ActivarDerrota();
        }
    }

    /// <summary>
    /// Formatea el tiempo (float) a un string 00:00 y lo pone en el texto
    /// </summary>
    void ActualizarTextoReloj(float tiempo)
    {
        if (textoReloj == null) return; // No hacer nada si no hay texto asignado

        if (tiempo < 0) tiempo = 0;

        // Calculamos minutos y segundos
        float minutos = Mathf.FloorToInt(tiempo / 60);
        float segundos = Mathf.FloorToInt(tiempo % 60);

        // Actualizamos el texto con el formato 00:00
        textoReloj.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    /// <summary>
    /// Lógica que se ejecuta al perder
    /// </summary>
    void ActivarDerrota()
    {
        Debug.LogWarning("¡TIEMPO AGOTADO! Has perdido.");
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(true);
        }

        // Opcional: Pausar todo el juego
        // Time.timeScale = 0f; 
    }


    // --- MÉTODOS PÚBLICOS DE CONTROL ---

    /// <summary>
    /// Inicia o reanuda el conteo del reloj.
    /// (Llama a esto desde otro script)
    /// </summary>
    public void IniciarReloj()
    {
        relojActivo = true;
    }

    /// <summary>
    /// Pausa o detiene el conteo del reloj.
    /// (Llama a esto desde otro script)
    /// </summary>
    public void PausarReloj()
    {
        relojActivo = false;
    }
}