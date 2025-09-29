using UnityEngine;

public class InteractionButton : MonoBehaviour
{
   

    [Header("Tipo de Parte del Cuerpo")]
    public BodyPart partType;

    // Esta función será llamada por el evento OnClick() del botón de la UI.
    public void OnPressed()
    {
        Debug.Log($"Se ha presionado el botón de: {partType}");
        // Le avisamos al RescueManager qué parte del cuerpo fue tocada.
        RescueManager.Instance.BodyPartInteracted(partType);
    }
}