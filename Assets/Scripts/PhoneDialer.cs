using UnityEngine;
using TMPro;

public class PhoneDialer : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public GameObject telefonoUI;
    public MoverUIConBoton moverUI;
    public GameObject objetoAActivar;

    [Header("Sistema de pasos")]
    public RecommendationsSteps pasosManager;

    [Header("Configuración")]
    public string numeroCorrecto = "116";
    public float destinoFinalY = -914f;

    [Header("Cámara")]
    public Camera camaraObjetivo; // ? Nueva referencia a la cámara

    // Valores de posición y rotación deseados
    private Vector3 nuevaPosicion = new Vector3(-82.34f, 1.059f, -2.532f);
    private Quaternion nuevaRotacion = new Quaternion(0.1886f, 0.6815f, -0.1886f, 0.6815f);

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
                objetoAActivar.SetActive(false);

            if (camaraObjetivo != null)
            {
                camaraObjetivo.transform.position = nuevaPosicion;
                camaraObjetivo.transform.rotation = nuevaRotacion;
            }
        }
        else
        {
            inputField.text = "";
        }
    }
}
