using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public void CambiarEscena(string nombreDeLaEscena)
    {
        
      SceneManager.LoadScene(nombreDeLaEscena);
                
    }
}
