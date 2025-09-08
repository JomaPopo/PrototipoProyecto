using UnityEngine;

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake(); // Ejecuta la l�gica del Singleton normal primero

        // Si este es el Singleton v�lido, lo hacemos persistente.
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}