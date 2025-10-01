using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Elementos de Feedback")]
    [SerializeField] private TextMeshProUGUI instructionText; 

    protected override void Awake()
    {
        base.Awake();
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }
    }

    public void ShowInstruction(string message)
    {
        if (instructionText != null)
        {
            instructionText.text = message;
            instructionText.gameObject.SetActive(true);
        }
    }

    public void HideInstructions()
    {
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }
    }
}