using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : SingletonPersistent<SceneChanger>
{

    public void CambiarEscena(string nombreDeLaEscena)
    {
        SceneManager.LoadScene(nombreDeLaEscena);
    }
}