using UnityEngine;

public class RecommendationsSteps : MonoBehaviour
{
    [Header("Contenedores de pasos")]
    public GameObject[] pasos;

    private int indiceActual = 0;

    private void Start()
    {
        MostrarPaso(indiceActual);
    }

    public void Siguiente()
    {
        indiceActual = (indiceActual + 1) % pasos.Length;
        MostrarPaso(indiceActual);
    }

    public void Anterior()
    {
        indiceActual = (indiceActual - 1 + pasos.Length) % pasos.Length;
        MostrarPaso(indiceActual);
    }

    private void MostrarPaso(int indice)
    {
        for (int i = 0; i < pasos.Length; i++)
        {
            pasos[i].SetActive(i == indice);
        }
    }
}
