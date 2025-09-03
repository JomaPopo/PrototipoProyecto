using NUnit.Framework;
using UnityEngine;

public class PlayerConfiguracionTests
{
    private GameObject personajePrefab;
    [SetUp]
    public void CargarElPrefabDelPersonaje()
    {
        personajePrefab = Resources.Load<GameObject>("Player");

    }
    [Test]
    public void PersonajeConfiguraciones()
    {

        var playerMovementScript = personajePrefab.GetComponent<PlayerMovement>();

        Assert.IsNotNull(playerMovementScript, "Al prefab del personaje le falta el script PlayerMovement.");

        var rigidbody = personajePrefab.GetComponent<Rigidbody>();
        Assert.IsNotNull(rigidbody, "Al prefab del personaje le falta el componente Rigidbody.");

        var collider = personajePrefab.GetComponent<Collider>();
        Assert.IsNotNull(collider, "Al prefab del personaje le falta un componente Collider.");

    }

}