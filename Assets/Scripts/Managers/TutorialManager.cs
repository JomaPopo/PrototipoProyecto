using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header("Paneles del Tutorial")]
    [Tooltip("Arrastra aquí todos los paneles de tu tutorial en el orden que quieras.")]
    [SerializeField] private GameObject[] tutorialPanels;

    void Start()
    {
        //HideAllTutorialPanels();
    }

    public void ShowTutorialPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= tutorialPanels.Length)
        {
            Debug.LogError($"El índice de panel {panelIndex} es inválido. Solo hay {tutorialPanels.Length} paneles asignados.");
            return;
        }

        HideAllTutorialPanels();

        Debug.Log($"Mostrando panel de tutorial con índice: {panelIndex}");
        tutorialPanels[panelIndex].SetActive(true);
    }

    public void HideAllTutorialPanels()
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            GameObject panel = tutorialPanels[i];

            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}