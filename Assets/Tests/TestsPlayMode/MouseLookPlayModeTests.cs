using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MouseLookPlayModeTests
{
    [UnityTest]
    public IEnumerator AlMoverElMouseHorizontalmente_ElCuerpoDelJugadorRota()
    {
        var playerBody = new GameObject("PlayerBody").transform;
        var cameraObject = new GameObject("Camera");
        cameraObject.transform.SetParent(playerBody); 
        var mouseLook = cameraObject.AddComponent<MouseLook>();

        mouseLook.playerBody = playerBody;
        mouseLook.mouseSensitivity = 100f;
        Time.timeScale = 20; 

        float rotacionInicialY = playerBody.eulerAngles.y;

        var mouseInput = new Vector2(10, 0);

        mouseLook.ProcessLook(mouseInput);
        yield return null;

        // Assert 1: La rotación en Y ha cambiado
        Assert.AreNotEqual(rotacionInicialY, playerBody.eulerAngles.y);

        // Assert 2: La rotación es positiva (hacia la derecha)
        Assert.Greater(playerBody.eulerAngles.y, rotacionInicialY);

        // Assert 3: La rotación vertical de la cámara NO ha cambiado
        Assert.AreEqual(0, cameraObject.transform.localEulerAngles.x);

        Object.Destroy(playerBody.gameObject);
        Time.timeScale = 1; 
    }
}