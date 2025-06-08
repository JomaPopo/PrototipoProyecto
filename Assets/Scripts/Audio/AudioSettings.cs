using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Audio/Settings")]
public class AudioSettings : ScriptableObject
{
     public float masterVolume = 1f;
     public float musicVolume = 1f;
     public float sfxVolume = 1f;
}
