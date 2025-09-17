using UnityEngine;

public class MoverUIConBoton : MonoBehaviour
{
    public float duracionMovimiento = 2f; // Tiempo en segundos
    private float destinoY;

    private RectTransform rectTransform;
    private Vector2 posicionInicial;
    private Vector2 posicionDestino;

    private bool enMovimiento = false;
    private float tiempoTranscurrido = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (enMovimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / duracionMovimiento);

            Vector2 nuevaPos = Vector2.Lerp(posicionInicial, posicionDestino, t);
            rectTransform.anchoredPosition = nuevaPos;

            if (t >= 1f)
            {
                enMovimiento = false;
            }
        }
        HandleInteractionInput();
    }

    public void ActivarMovimiento(float nuevoDestinoY)
    {
        rectTransform = GetComponent<RectTransform>();
        posicionInicial = rectTransform.anchoredPosition;
        destinoY = nuevoDestinoY;
        posicionDestino = new Vector2(posicionInicial.x, destinoY);
        tiempoTranscurrido = 0f;
        enMovimiento = true;
    }
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivarMovimiento(2);
        }
    }
}
