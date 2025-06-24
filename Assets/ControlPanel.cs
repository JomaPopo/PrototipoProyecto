using UnityEngine;

public class ControlPanel : MonoBehaviour
{

    public GameObject panelajuste;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPanel()
    {
     panelajuste.SetActive(true);
    }
    public void OffPanel()
    {
        panelajuste.SetActive(false);
    }
}
