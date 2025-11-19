using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : Singleton<PauseManager>
{
    [Header("Referencias del Jugador (Arrastrar)")]
   // [SerializeField] private PlayerMovement playerMovement;
   // [SerializeField] private MouseLook playerMouseLook;

    [Header("Panel de Pausa (Arrastrar)")]
    [SerializeField] private GameObject pauseMenuPanel;

    private bool isPaused = false;

    // --- ¡ARREGLO BUG 2! ---
    // Esta variable "recuerda" si el reloj estaba corriendo
    // antes de que pusiéramos pausa.
    private bool wasClockActiveBeforePause = false;

    protected override void Awake()
    {
        base.Awake();
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Se llama desde el PlayerInput (P o botón de Menú)
    /// </summary>
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }

    /// <summary>
    /// La lógica central para pausar o reanudar
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // --- PAUSAR EL JUEGO ---

            // --- ¡ARREGLO BUG 1! ---
            // Esta línea pausa TODOS los sonidos del juego.
            AudioListener.pause = true;

            // --- ¡ARREGLO BUG 2! ---
            // 1. Guardamos el estado del reloj ANTES de pausarlo.
            wasClockActiveBeforePause = GameManager.Instance.relojActivo;

            Time.timeScale = 0f; // Congela la física
            GameManager.Instance.PausarReloj(); // Pausa el cronómetro

            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
        }
        else
        {
            // --- REANUDAR EL JUEGO ---

            // --- ¡ARREGLO BUG 1! ---
            // Esta línea reanuda TODOS los sonidos del juego.
            AudioListener.pause = false;

            Time.timeScale = 1f; // Reanuda la física

            // --- ¡ARREGLO BUG 2! ---
            // 2. Solo reanudamos el reloj SI ESTABA CORRIENDO ANTES.
            if (wasClockActiveBeforePause)
            {
                GameManager.Instance.IniciarReloj();
            }


            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
        }
    }

    // --- Funciones para tus 3 Botones de UI (Sin cambios) ---

    public void Continuar()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false; // ¡Importante! Asegurarse de reanudar el audio

        // --- ¡ARREGLO DEL BUG DE VOZ PERSISTENTE! ---
        // Le decimos al AudioManager que pare todos los sonidos ANTES de recargar.
        // (Asegúrate de tener una función "StopAllSounds" en tu AudioManager.Instance)
        if (AudioManager.Instance != null)
        {
            // O como se llame tu función para parar los audios: StopVoices(), StopAll(), etc.
            AudioManager.Instance.StopAllSounds();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false; // ¡Importante! Asegurarse de reanudar el audio

        // --- ¡ARREGLO DEL BUG DE VOZ PERSISTENTE! ---
        // Le decimos al AudioManager que pare todos los sonidos ANTES de salir al menú.
        // (Asegúrate de tener una función "StopAllSounds" en tu AudioManager.Instance)
        if (AudioManager.Instance != null)
        {
            // O como se llame tu función para parar los audios: StopVoices(), StopAll(), etc.
            AudioManager.Instance.StopAllSounds();
        }

        SceneManager.LoadScene("Menu_"); // (Asegúrate de que tu escena se llame así)
    }


  
}

