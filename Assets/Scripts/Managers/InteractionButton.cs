using UnityEngine;

public class InteractionButton : MonoBehaviour
{
   

    [Header("Tipo de Parte del Cuerpo")]
    public BodyPart partType;

    public void OnPressed()
    {
        Debug.Log($"Se ha presionado el bot�n de: {partType}");
        RescueManager.Instance.BodyPartInteracted(partType);
    }
}