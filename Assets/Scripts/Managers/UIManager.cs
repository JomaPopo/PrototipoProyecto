using UnityEngine;
using TMPro;
using System.Collections;
using System.Text; 

public class UIManager : Singleton<UIManager>
{
    [Header("Elementos de Feedback")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [Tooltip("La velocidad a la que aparecen las letras. Un valor m�s bajo es m�s r�pido.")]
    [SerializeField] private float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;

    // Optimizacion: Usaremos una instancia de StringBuilder para no generar basura con strings.
    private readonly StringBuilder stringBuilder = new StringBuilder();

    protected override void Awake()
    {
        base.Awake();
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }
    }

    public void ShowInstruction(string message)
    {
        if (instructionText != null)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(message));
        }
    }

    public void HideInstructions()
    {
        if (instructionText != null)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            instructionText.gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeText(string message)
    {
        // Optimizacion: Creamos la instrucci�n de espera UNA SOLA VEZ y la reutilizamos.
        var waitInstruction = new WaitForSeconds(typingSpeed);

        instructionText.gameObject.SetActive(true);

        // Limpiamos el StringBuilder antes de usarlo. Es m�s eficiente que crear uno nuevo.
        stringBuilder.Clear();

        // Optimizacion: Usamos un bucle 'for' est�ndar. Es m�s r�pido para strings.
        for (int i = 0; i < message.Length; i++)
        {
            // Optimizacion: A�adimos el caracter al StringBuilder.
            stringBuilder.Append(message[i]);

            // Actualizamos el texto en la UI.
            instructionText.text = stringBuilder.ToString();

            // Reutilizamos la misma instrucci�n de espera.
            yield return waitInstruction;
        }

        typingCoroutine = null;
    }
}