using UnityEngine;

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake(); // Ejecuta la lógica del Singleton normal primero

        // Si este es el Singleton válido, lo hacemos persistente.
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}