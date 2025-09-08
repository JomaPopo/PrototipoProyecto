using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Se encontró otra instancia de {typeof(T).Name}. Destruyendo este objeto.");
            Destroy(gameObject); // Destruye el GameObject completo, no solo el script.
        }
        else
        {
            Instance = (T)System.Convert.ChangeType(this, typeof(T));
        }
    }
}