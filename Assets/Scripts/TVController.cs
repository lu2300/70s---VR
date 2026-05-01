using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR;
using System.Collections.Generic;

public class TVController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioClip clickSound;
    public VideoClip[] videos;
    private int indiceActual = 0;
    public AudioSource audio3D;

    bool tvEncendida;

    private bool _isHovered = false;
    private bool _gripPressedLastFrame = false;
    private float _lastActionTime = 0f;
    private const float _cooldown = 0.4f;
    private List<InputDevice> _controllers = new List<InputDevice>();

    void Start()
    {
        Debug.Log("TVController activo");
        videoPlayer.Stop();

        if (audio3D != null)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audio3D);
        }

        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(_ => { _isHovered = true;  _gripPressedLastFrame = false; });
            interactable.hoverExited.AddListener(_  => { _isHovered = false; _gripPressedLastFrame = false; });
        }
    }

    void Update()
    {
        if (tvEncendida && Input.GetMouseButtonDown(1))
            CambiarVideo();

        if (_isHovered && tvEncendida)
            CheckVRGrip();
    }

    void CheckVRGrip()
    {
        if (Time.time - _lastActionTime < _cooldown) return;

        if (_controllers.Count == 0)
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller, _controllers);

        bool gripDetected = false;
        foreach (var device in _controllers)
        {
            device.TryGetFeatureValue(CommonUsages.gripButton, out bool grip);
            if (grip) gripDetected = true;
        }

        if (gripDetected && !_gripPressedLastFrame)
        {
            CambiarVideo();
            _lastActionTime = Time.time;
        }

        _gripPressedLastFrame = gripDetected;
    }

    public void ToggleTV()
    {
        tvEncendida = !tvEncendida;

        if (tvEncendida)
        {
            Debug.Log("TV ENCENDIDA");
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
            videoPlayer.clip = videos[indiceActual];
            videoPlayer.Play();
        }
        else
        {
            Debug.Log("TV APAGADA");
            videoPlayer.Stop();
        }
    }

    void CambiarVideo()
    {
        indiceActual++;
        if (indiceActual >= videos.Length)
            indiceActual = 0;

        videoPlayer.clip = videos[indiceActual];
        videoPlayer.Play();
    }
}