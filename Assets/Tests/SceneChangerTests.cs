using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;


public class SceneChangerTests
{
    [Test]
    public void SceneChanger_SePuedeAnadirAUnGameObject()
    {
        var gameObject = new GameObject();

        var sceneChanger = gameObject.AddComponent<SceneChanger>();


        Assert.IsNotNull(sceneChanger, "No se pudo añadir el script SceneChanger al GameObject.");

        Assert.IsTrue(gameObject.activeInHierarchy, "El GameObject debería estar activo por defecto.");

        Assert.IsTrue(sceneChanger.enabled, "El componente SceneChanger debería estar habilitado por defecto.");
    }
}