using UnityEngine;

public class InteractionButton : MonoBehaviour
{
   

    [Header("Tipo de Parte del Cuerpo")]
    public BodyPart partType;

    // Esta funci�n ser� llamada por el evento OnClick() del bot�n de la UI.
    public void OnPressed()
    {
        Debug.Log($"Se ha presionado el bot�n de: {partType}");
        // Le avisamos al RescueManager qu� parte del cuerpo fue tocada.
        RescueManager.Instance.BodyPartInteracted(partType);
    }
}