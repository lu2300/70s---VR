using UnityEngine;
using UnityEngine.XR;

using System.Collections.Generic;

public class RadiolaController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource musicaSource;
    public AudioSource sfxSource;
    public AudioClip clickSound;

    [Header("Canciones")]
    public AudioClip[] canciones;
    private int indiceActual = 0;

    private bool encendida = false;

    // ─── VR ───────────────────────────────────────────────────────────────────
    private bool _isHovered = false;
    private bool _gripPressedLastFrame = false;
    private bool _triggerPressedLastFrame = false;
    private float _lastActionTime = 0f;
    private const float _cooldown = 0.4f;
    private List<InputDevice> _controllers = new List<InputDevice>();

    void Start()
    {
        Debug.Log("Radiola lista");

        if (musicaSource != null) musicaSource.Stop();
        if (sfxSource != null)    sfxSource.Stop();

        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(_ =>
            {
                _isHovered = true;
                _gripPressedLastFrame = false;
                _triggerPressedLastFrame = false;
            });
            interactable.hoverExited.AddListener(_ =>
            {
                _isHovered = false;
                _gripPressedLastFrame = false;
                _triggerPressedLastFrame = false;
            });
        }
    }

    void Update()
    {
        // 👉 Desktop: clic derecho para cambiar canción
        if (encendida && Input.GetMouseButtonDown(1))
            CambiarCancion();

        // 👉 VR
        if (_isHovered)
            CheckVRInput();
    }

    // 👉 CLICK CON MOUSE (desktop)
    void OnMouseDown() => ToggleRadio();

    // 👉 ACTIVACIÓN POR TRIGGER (colisiones)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ToggleRadio();
    }

    // ─── VR Input ─────────────────────────────────────────────────────────────
    void CheckVRInput()
    {
        if (Time.time - _lastActionTime < _cooldown) return;

        if (_controllers.Count == 0)
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller, _controllers);

        bool gripDetected = false;
        bool triggerDetected = false;

        foreach (var device in _controllers)
        {
            device.TryGetFeatureValue(CommonUsages.gripButton, out bool grip);
            device.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);
            if (grip)    gripDetected = true;
            if (trigger) triggerDetected = true;
        }

        // Trigger → Encender/Apagar
        if (triggerDetected && !_triggerPressedLastFrame)
        {
            ToggleRadio();
            _lastActionTime = Time.time;
        }
        // Grip → Cambiar canción
        else if (gripDetected && !_gripPressedLastFrame && encendida)
        {
            CambiarCancion();
            _lastActionTime = Time.time;
        }

        _gripPressedLastFrame = gripDetected;
        _triggerPressedLastFrame = triggerDetected;
    }

    // ─── Acciones ─────────────────────────────────────────────────────────────

    public void ToggleRadio()
    {
        encendida = !encendida;

        if (encendida)
        {
            Debug.Log("RADIOLA ENCENDIDA");
            if (sfxSource != null && clickSound != null)
                sfxSource.PlayOneShot(clickSound);
            if (musicaSource != null && canciones.Length > 0)
            {
                musicaSource.clip = canciones[indiceActual];
                musicaSource.Play();
            }
        }
        else
        {
            Debug.Log("RADIOLA APAGADA");
            if (musicaSource != null) musicaSource.Stop();
        }
    }

    public void CambiarCancion()
    {
        if (!encendida || canciones.Length == 0) return;

        indiceActual++;
        if (indiceActual >= canciones.Length)
            indiceActual = 0;

        musicaSource.clip = canciones[indiceActual];
        musicaSource.Play();
    }
}