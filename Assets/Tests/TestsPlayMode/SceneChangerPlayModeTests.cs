using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneChangerPlayModeTests
{
    [UnityTest]
    public IEnumerator AlLlamarCambiarEscena_SeCargaLaNuevaEscena()
    {
        var sceneChangerObject = new GameObject();
        var sceneChanger = sceneChangerObject.AddComponent<SceneChanger>();
        string nombreEscenaACargar = "EscenaDePrueba"; // La escena que creamos

        string escenaOriginal = SceneManager.GetActiveScene().name;

        sceneChanger.CambiarEscena(nombreEscenaACargar);

        yield return null;

        Assert.AreEqual(nombreEscenaACargar, SceneManager.GetActiveScene().name, "La nueva escena activa no es la esperada.");

        Scene nuevaEscena = SceneManager.GetSceneByName(nombreEscenaACargar);
        Assert.IsTrue(nuevaEscena.IsValid(), "La escena cargada debería ser válida.");

        Assert.AreNotEqual(escenaOriginal, SceneManager.GetActiveScene().name, "La escena no debería ser la misma que la original.");

        Object.Destroy(sceneChangerObject);
    }
}