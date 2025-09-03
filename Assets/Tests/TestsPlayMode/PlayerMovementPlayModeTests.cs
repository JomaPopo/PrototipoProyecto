using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class PlayerMovementPlayModeTests
{
    [UnityTest]
    public IEnumerator AlLlamarOnJump_ElPersonajeSalta_ConMultiplesAsserts()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        var playerGameObject = new GameObject("Player");
        var rb = playerGameObject.AddComponent<Rigidbody>();
        var playerMovement = playerGameObject.AddComponent<PlayerMovement>();

        playerMovement.jumpForce = 7f;
        playerMovement.groundMask = LayerMask.GetMask("Default");

        var groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(playerGameObject.transform);
        groundCheck.transform.localPosition = new Vector3(0, -0.5f, 0);
        playerMovement.groundCheck = groundCheck.transform;

        playerGameObject.transform.position = new Vector3(0, 0.6f, 0);
        float posicionInicialY = rb.position.y;

        yield return null; 

        playerMovement.JumpForTest();

        yield return new WaitForFixedUpdate();

        // 3. ASSERT (Verificar)
        Assert.Greater(rb.linearVelocity.y, 0, "La velocidad vertical debería ser positiva.");
        Assert.Greater(rb.position.y, posicionInicialY, "La posición Y debería ser mayor que la inicial.");

        yield return new WaitForSeconds(0.1f);

        var isGroundedField = typeof(PlayerMovement).GetField("isGrounded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool isGroundedActual = (bool)isGroundedField.GetValue(playerMovement);
        Assert.IsFalse(isGroundedActual, "El personaje no debería estar en el suelo después de saltar.");

        Object.Destroy(playerGameObject);
        Object.Destroy(ground);
    }
}