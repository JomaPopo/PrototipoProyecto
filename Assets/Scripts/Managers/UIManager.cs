using UnityEngine;
using TMPro;
using System.Collections;
using System.Text; 

public class UIManager : Singleton<UIManager>
{
    [Header("Elementos de Feedback")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [Tooltip("La velocidad a la que aparecen las letras. Un valor más bajo es más rápido.")]
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
        // Optimizacion: Creamos la instrucción de espera UNA SOLA VEZ y la reutilizamos.
        var waitInstruction = new WaitForSeconds(typingSpeed);

        instructionText.gameObject.SetActive(true);

        // Limpiamos el StringBuilder antes de usarlo. Es más eficiente que crear uno nuevo.
        stringBuilder.Clear();

        // Optimizacion: Usamos un bucle 'for' estándar. Es más rápido para strings.
        for (int i = 0; i < message.Length; i++)
        {
            // Optimizacion: Añadimos el caracter al StringBuilder.
            stringBuilder.Append(message[i]);

            // Actualizamos el texto en la UI.
            instructionText.text = stringBuilder.ToString();

            // Reutilizamos la misma instrucción de espera.
            yield return waitInstruction;
        }

        typingCoroutine = null;
    }
}