using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class VRInventory : MonoBehaviour
{
    [Header("Canvas del inventario VR")]
    public GameObject vrInventoryCanvas;
    public CanvasGroup canvasGroup; // Agrega CanvasGroup al canvas para el fade

    [Header("VR - Main Camera del XR Origin")]
    public Transform xrCameraTransform;

    [Header("Distancia frente al jugador")]
    public float canvasDistance = 1.5f;

    [Header("Tiempo visible y fade")]
    public float visibleDuration = 5f;
    public float fadeDuration = 1f;

    private bool _isOpen = false;
    private bool _isVRMode = false;
    private bool _buttonPressedLastFrame = false;
    private float _lastControllerSearchTime = 0f;
    private const float _controllerSearchCooldown = 3f;

    private InputDevice _leftController;
    private Coroutine _autoCloseCoroutine;

    void Start()
    {
        if (vrInventoryCanvas != null)
            vrInventoryCanvas.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        StartCoroutine(InitVR());
    }

    IEnumerator InitVR()
    {
        float timeout = 10f;
        while (!XRSettings.isDeviceActive && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        _isVRMode = XRSettings.isDeviceActive;

        if (_isVRMode)
            TryGetLeftController();
    }

    void TryGetLeftController()
    {
        var devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count == 0)
            InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);

        if (devices.Count > 0)
            _leftController = devices[0];
    }

    void Update()
    {
        if (!_isVRMode) return;

        if (!_leftController.isValid)
        {
            if (Time.time - _lastControllerSearchTime > _controllerSearchCooldown)
            {
                _lastControllerSearchTime = Time.time;
                TryGetLeftController();
            }
            return;
        }

        // Botón X → abrir/cerrar
        _leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed);
        if (pressed && !_buttonPressedLastFrame)
            ToggleInventory();
        _buttonPressedLastFrame = pressed;

        // Sigue la cámara mientras está abierto
        if (_isOpen)
            PositionCanvas();
    }

    void ToggleInventory()
    {
        if (_isOpen)
        {
            // Cierra inmediatamente si se presiona X de nuevo
            if (_autoCloseCoroutine != null)
                StopCoroutine(_autoCloseCoroutine);
            StartCoroutine(FadeOut());
        }
        else
        {
            // Abre y arranca el timer
            if (_autoCloseCoroutine != null)
                StopCoroutine(_autoCloseCoroutine);

            _isOpen = true;
            PositionCanvas();
            vrInventoryCanvas.SetActive(true);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            _autoCloseCoroutine = StartCoroutine(AutoClose());
        }
    }

    // Espera 15 segundos y hace fade out
    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(visibleDuration);
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        _isOpen = false;

        if (canvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        vrInventoryCanvas.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f; // Resetea para la próxima vez
    }

    void PositionCanvas()
    {
        if (vrInventoryCanvas == null) return;

        if (xrCameraTransform == null)
            xrCameraTransform = Camera.main?.transform;

        if (xrCameraTransform == null) return;

        Vector3 forward = xrCameraTransform.forward;

        vrInventoryCanvas.transform.position =
            xrCameraTransform.position + forward * canvasDistance;

        vrInventoryCanvas.transform.rotation =
            Quaternion.LookRotation(forward);
    }
}