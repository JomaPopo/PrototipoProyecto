using UnityEngine;

public class TestRespuestaControl_QA : MonoBehaviour
{
    // Arrastra el Transform de tu personaje-cápsula aquí desde el Inspector
    public Transform personajeTransform;

    private float tiempoDeEntrada;
    private Vector3 posicionInicial;
    private bool esperandoRespuestaMovimiento = false;

    void Update()
    {
        // Detecta cuando se presiona la tecla de movimiento
        if (Input.GetKeyDown(KeyCode.W))
        {
            // El test solo debe empezar si el personaje está completamente quieto
            // (Para este método, asumimos que está quieto al presionar)

            tiempoDeEntrada = Time.realtimeSinceStartup;
            // Guardamos la posición exacta en el momento de presionar la tecla
            posicionInicial = personajeTransform.position;

            esperandoRespuestaMovimiento = true;
            Debug.Log($"[TEST] Tecla W presionada en el tiempo: {tiempoDeEntrada} segundos. Posición inicial: {posicionInicial}");
        }

        // Si estamos esperando una respuesta, revisamos si el personaje ya se movió
        if (esperandoRespuestaMovimiento)
        {
            // Comprobamos si la distancia desde la posición inicial es mayor a un valor pequeño
            if (Vector3.Distance(personajeTransform.position, posicionInicial) > 0.001f)
            {
                float tiempoDeRespuesta = Time.realtimeSinceStartup;
                float latencia = (tiempoDeRespuesta - tiempoDeEntrada) * 1000; // Convertir a milisegundos

                Debug.LogWarning($"[TEST] Personaje se movió en el tiempo: {tiempoDeRespuesta} segundos.");
                Debug.LogWarning($"[RESULTADO] Latencia interna del control: {latencia.ToString("F2")} ms.");

                // Detenemos el test hasta la próxima vez que se presione la tecla
                esperandoRespuestaMovimiento = false;
            }
        }
    }
}