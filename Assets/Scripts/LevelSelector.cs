using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [Header("Contenedores de niveles")]
    public GameObject[] niveles;

    private int indiceActual = 0;

    private void Start()
    {
        MostrarNivel(indiceActual);
    }

    public void Siguiente()
    {
        indiceActual = (indiceActual + 1) % niveles.Length;
        MostrarNivel(indiceActual);
    }

    public void Anterior()
    {
        indiceActual = (indiceActual - 1 + niveles.Length) % niveles.Length;
        MostrarNivel(indiceActual);
    }

    private void MostrarNivel(int indice)
    {
        for (int i = 0; i < niveles.Length; i++)
        {
            niveles[i].SetActive(i == indice);
        }
    }
}
