using UnityEngine;
using TMPro;

public class PhoneDialer : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public GameObject telefonoUI;
    public MoverUIConBoton moverUI; // Referencia al script que mueve
    public GameObject objetoAActivar; // ?? Objeto que se activará al ejecutar ActivarPaso

    [Header("Sistema de pasos")]
    public RecommendationsSteps pasosManager;

    [Header("Configuración")]
    public string numeroCorrecto = "116";
    public float destinoFinalY = -914f;

    public void AgregarNumero(string numero)
    {
        inputField.text += numero;
    }

    public void IntentarLlamar()
    {
        if (inputField.text == numeroCorrecto)
        {
            pasosManager.Siguiente();
            moverUI.ActivarMovimiento(destinoFinalY);
            if (objetoAActivar != null)
                objetoAActivar.SetActive(false); // ?? Activar el objeto oculto
        }
        else
        {
            inputField.text = "";
        }
    }
}
