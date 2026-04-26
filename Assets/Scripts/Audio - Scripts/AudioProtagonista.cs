using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class AudioProtagonista : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip clipPrimeraVez;

    private bool yaReprodujo = false;
    private AudioSource audioSource;

    void Start()
    {
        // Busca AudioSource en la escena o crea uno
        audioSource = FindObjectOfType<AudioSource>();
        if (audioSource == null)
        {
            GameObject go = new GameObject("AudioManager");
            audioSource = go.AddComponent<AudioSource>();
        }

        var interactable = GetComponent<XRGrabInteractable>();
        interactable.selectEntered.AddListener(OnAgarrar);
    }

    void OnAgarrar(SelectEnterEventArgs args)
    {
        if (yaReprodujo || clipPrimeraVez == null) return;

        yaReprodujo = true;
        audioSource.PlayOneShot(clipPrimeraVez);
    }

    void OnDestroy()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnAgarrar);
    }
}