using UnityEngine;

[System.Serializable]
public class MissionSequenceStep
{
    [Tooltip("Nombre del parámetro Trigger en el Animator para esta animación")]
    public string animatorTriggerName;

    [Tooltip("Clip de audio que se reproduce junto con la animación")]
    public AudioClip audioClip;

    [Tooltip("Duración manual en segundos. Si es 0, usa la duración del AudioClip automáticamente.")]
    public float overrideDuration = 0f;

    public float GetDuration()
    {
        if (overrideDuration > 0f) return overrideDuration;
        if (audioClip != null) return audioClip.length;
        return 1f;
    }
}
