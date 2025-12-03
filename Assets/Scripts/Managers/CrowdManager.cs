using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastra aquí el objeto padre que contiene a todos los NPCs mirones")]
    [SerializeField] private GameObject crowdParent;

    public static CrowdManager Instance;

    void Awake()
    {
        Instance = this;
        if (crowdParent != null)
            crowdParent.SetActive(false); 
    }

    /// <summary>
    /// Activa la multitud y hace que miren al centro.
    /// </summary>
    public void ActivateCrowd()
    {
        if (crowdParent != null)
        {
            crowdParent.SetActive(true);

        }
    }

    /// <summary>
    /// Desactiva la multitud (por si quieres que se vayan al final).
    /// </summary>
    public void DeactivateCrowd()
    {
        if (crowdParent != null)
            crowdParent.SetActive(false);
    }
}