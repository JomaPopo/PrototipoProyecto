using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : SingletonPersistent<SceneChanger>
{
    // 2. ¡Listo! No necesita más código. El molde hace todo el trabajo.

    public void CambiarEscena(string nombreDeLaEscena)
    {
        SceneManager.LoadScene(nombreDeLaEscena);
    }
}