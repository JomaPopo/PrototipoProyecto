using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PickPocketManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform redBarUI;
    [SerializeField] private RectTransform greenBarUI;
    [SerializeField] private RectTransform indicatorUI;
    [SerializeField] private TMP_Text scoreText;

    [Header("Settings")]
    [SerializeField] private float indicatorTravelTime = 1f;
    [SerializeField] private float minSuccessSize = 5f;
    [SerializeField] private float maxSuccessSize = 20f;

    [Header("Animación de objeto")]
    [SerializeField] private Transform objetoAAnimar;
    [SerializeField] private float desplazamientoY = 0.196f;
    [SerializeField] private float duracionMovimiento = 1f;

    [Header("Cámara")]
    [SerializeField] private Camera camara;
    private Vector3 nuevaPosicion = new Vector3(-82.34f, 1.059f, -2.532f);
    private Quaternion nuevaRotacion = new Quaternion(0.1886f, 0.6815f, -0.1886f, 0.6815f);

    [Header("Contenedor del minijuego")]
    [SerializeField] private GameObject objetoContenedor;

    [Header("Objeto adicional a ocultar")]
    [SerializeField] private GameObject objetoExtraAOcultar;

    [Header("Sistema de pasos")]
    [SerializeField] private RecommendationsSteps pasosManager;

    [Header("Control de rotación de cámara")]
    [SerializeField] private RaycastDetector raycastDetector;

    private float moveSpeed;
    private float currentIndicatorPosition;
    private int currentDirection = 1;

    private float successZoneCenter;
    private float successZoneHalfWidth;

    private float LeftRedBound;
    private float RightRedBound;

    private bool isPlaying = false;
    private bool isGameEnded = false;

    private int score = 0;

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!isPlaying || isGameEnded) return;

        MoveIndicator();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }

    private void StartGame()
    {
        LeftRedBound = -redBarUI.rect.width / 2f;
        RightRedBound = redBarUI.rect.width / 2f;

        currentIndicatorPosition = LeftRedBound;

        float distance = RightRedBound - LeftRedBound;
        moveSpeed = distance / indicatorTravelTime;

        UpdateIndicatorPosition();
        SetupSuccessZone();

        isPlaying = true;
        isGameEnded = false;

        scoreText.text = "Pulsaciones: " + score;
    }

    private void UpdateIndicatorPosition()
    {
        indicatorUI.anchoredPosition = new Vector2(currentIndicatorPosition, redBarUI.anchoredPosition.y);
    }

    private void SetupSuccessZone()
    {
        float randomSuccessZoneWidth = redBarUI.rect.width * Random.Range(minSuccessSize, maxSuccessSize) / 100f;

        float maxCenter = redBarUI.rect.width / 2f;
        float minCenter = -redBarUI.rect.width / 2f;

        successZoneHalfWidth = randomSuccessZoneWidth / 2f;
        successZoneCenter = Random.Range(minCenter + successZoneHalfWidth, maxCenter - successZoneHalfWidth);

        greenBarUI.sizeDelta = new Vector2(randomSuccessZoneWidth, redBarUI.rect.height);
        greenBarUI.anchoredPosition = new Vector2(successZoneCenter, redBarUI.anchoredPosition.y);
    }

    private void MoveIndicator()
    {
        currentIndicatorPosition += moveSpeed * currentDirection * Time.deltaTime;

        if (currentIndicatorPosition > RightRedBound)
        {
            currentIndicatorPosition = RightRedBound;
            currentDirection = -1;
        }
        else if (currentIndicatorPosition < LeftRedBound)
        {
            currentIndicatorPosition = LeftRedBound;
            currentDirection = 1;
        }

        UpdateIndicatorPosition();
    }

    private void CheckSuccess()
    {
        if (currentIndicatorPosition >= (successZoneCenter - successZoneHalfWidth) &&
            currentIndicatorPosition <= (successZoneCenter + successZoneHalfWidth))
        {
            Debug.Log("Win");
            score++;
            scoreText.text = "Pulsaciones: " + score;

            StartCoroutine(BajarYSubirObjeto());

            if (score >= 30)
            {
                FinalizarJuego();
                return;
            }
        }
        else
        {
            Debug.Log("Lost");
        }

        isGameEnded = true;
        isPlaying = false;

        Invoke(nameof(RestartGame), 1.5f);
    }

    private void RestartGame()
    {
        StartGame();
    }

    private IEnumerator BajarYSubirObjeto()
    {
        Vector3 posicionInicial = objetoAAnimar.position;
        Vector3 posicionFinal = posicionInicial + new Vector3(0f, -desplazamientoY, 0f);

        float tiempoMitad = duracionMovimiento / 2f;
        float t = 0f;

        while (t < tiempoMitad)
        {
            objetoAAnimar.position = Vector3.Lerp(posicionInicial, posicionFinal, t / tiempoMitad);
            t += Time.deltaTime;
            yield return null;
        }

        objetoAAnimar.position = posicionFinal;

        t = 0f;
        while (t < tiempoMitad)
        {
            objetoAAnimar.position = Vector3.Lerp(posicionFinal, posicionInicial, t / tiempoMitad);
            t += Time.deltaTime;
            yield return null;
        }

        objetoAAnimar.position = posicionInicial;
    }

    private void FinalizarJuego()
    {
        Debug.Log("Fin del juego");

        // Ocultar el minijuego
        if (objetoContenedor != null)
            objetoContenedor.SetActive(false);
        else
            this.gameObject.SetActive(false);

        // Ocultar objeto adicional
        if (objetoExtraAOcultar != null)
            objetoExtraAOcultar.SetActive(false);

        // Mover la cámara
        if (camara != null)
        {
            camara.transform.position = nuevaPosicion;
            camara.transform.rotation = nuevaRotacion;
        }

        // Reactivar movimiento de cámara si fue desactivado
        if (raycastDetector != null)
            raycastDetector.activarRotacion = true;

        // Avanzar al siguiente paso
        if (pasosManager != null)
            pasosManager.Siguiente();
    }
}
