using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class InventoryHUDView : MonoBehaviour
{
    [Header("Slots del inventario")]
    [SerializeField] private Image[] slots = new Image[3];

    [Header("VR - Canvas raíz del inventario")]
    public GameObject vrInventoryCanvas;

    [Header("VR - Main Camera del XR Origin")]
    public Transform xrCameraTransform;

    [Header("VR - Configuración")]
    public float canvasDistance = 1.5f;
    public float visibleDuration = 5f;
    public float fadeDuration = 1f;

    private bool _isVRMode = false;
    private bool _isVisible = false;
    private bool _buttonPressedLastFrame = false;
    private float _lastControllerSearchTime = 0f;
    private const float _controllerSearchCooldown = 3f;
    private InputDevice _leftController;
    private Coroutine _autoHideCoroutine;
    private CanvasGroup _canvasGroup;

    // ─── Init ─────────────────────────────────────────────────────────────────

    private void Awake()
    {
        Debug.Log("[Inventory] Awake");

        // Suscripción segura con delay por si InventoryManager aún no existe
        StartCoroutine(SubscribeToInventory());
    }

    IEnumerator SubscribeToInventory()
    {
        // Espera hasta que InventoryManager exista
        float timeout = 5f;
        while (InventoryManager.Instance == null && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnItemCollected += OnItemCollected;
            Debug.Log("[Inventory] Suscrito a InventoryManager");
        }
        else
        {
            Debug.LogWarning("[Inventory] InventoryManager no encontrado");
        }
    }

    private void Start()
    {
        Debug.Log("[Inventory] Start");

        // Oculta el canvas al inicio
        if (vrInventoryCanvas != null)
        {
            vrInventoryCanvas.SetActive(false);

            _canvasGroup = vrInventoryCanvas.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = vrInventoryCanvas.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 1f;

            Debug.Log($"[Inventory] vrInventoryCanvas asignado: {vrInventoryCanvas.name}");
        }
        else
        {
            Debug.LogWarning("[Inventory] vrInventoryCanvas NO está asignado en el Inspector");
        }

        // Slots invisibles al inicio
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.sprite = null;
                var c = slot.color;
                c.a = 0f;
                slot.color = c;
            }
        }

        StartCoroutine(InitVR());
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemCollected -= OnItemCollected;
    }

    // ─── VR Init ──────────────────────────────────────────────────────────────

    IEnumerator InitVR()
    {
        Debug.Log("[Inventory] Esperando XR...");

        float timeout = 10f;
        while (!XRSettings.isDeviceActive && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        _isVRMode = XRSettings.isDeviceActive;
        Debug.Log($"[Inventory] _isVRMode: {_isVRMode}");

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
        {
            _leftController = devices[0];
            Debug.Log($"[Inventory] Controlador encontrado: {_leftController.name}");
        }
        else
        {
            Debug.LogWarning("[Inventory] Controlador no encontrado");
        }
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    private void Update()
    {
        if (!_isVRMode) return;

        // Sigue la cámara siempre que esté visible
        if (_isVisible)
            PositionCanvas();

        if (!_leftController.isValid)
        {
            if (Time.time - _lastControllerSearchTime > _controllerSearchCooldown)
            {
                _lastControllerSearchTime = Time.time;
                TryGetLeftController();
            }
            return;
        }

        _leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed);
        if (pressed && !_buttonPressedLastFrame)
            ToggleVisibility();
        _buttonPressedLastFrame = pressed;
    }

    // ─── Visibilidad ──────────────────────────────────────────────────────────

    void ToggleVisibility()
    {
        if (_isVisible) HideInventory();
        else ShowInventory();
    }

    void ShowInventory()
    {
        Debug.Log("[Inventory] ShowInventory");

        if (_autoHideCoroutine != null)
            StopCoroutine(_autoHideCoroutine);

        if (vrInventoryCanvas == null)
        {
            Debug.LogWarning("[Inventory] vrInventoryCanvas es null en ShowInventory");
            return;
        }

        _isVisible = true;
        PositionCanvas();
        vrInventoryCanvas.SetActive(true);

        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;

        _autoHideCoroutine = StartCoroutine(AutoHide());
    }

    void HideInventory()
    {
        if (_autoHideCoroutine != null)
            StopCoroutine(_autoHideCoroutine);

        // Asegura que empiece desde alpha 1
        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;

        _autoHideCoroutine = StartCoroutine(FadeOutAndHide());
    }


    void OnItemCollected(int slotIndex, Sprite sprite)
    {
        Debug.Log($"[Inventory] Item recogido en slot {slotIndex}");
        FillSlot(slotIndex, sprite);

        if (_isVRMode)
            ShowInventory();
    }

    // ─── Corrutinas ───────────────────────────────────────────────────────────

    IEnumerator AutoHide()
    {
        Debug.Log($"[Inventory] AutoHide: esperando {visibleDuration} segundos");
        yield return new WaitForSeconds(visibleDuration);
        yield return StartCoroutine(FadeOutAndHide());
    }

    IEnumerator FadeOutAndHide()
    {
        _isVisible = false;

        if (_canvasGroup != null)
        {
            float elapsed = 0f;
            float startAlpha = _canvasGroup.alpha; // Toma el alpha actual
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;
        }

        if (vrInventoryCanvas != null)
            vrInventoryCanvas.SetActive(false);
    }

    // ─── Posicionamiento ──────────────────────────────────────────────────────

    void PositionCanvas()
    {
        if (vrInventoryCanvas == null) return;

        if (xrCameraTransform == null)
            xrCameraTransform = Camera.main?.transform;

        if (xrCameraTransform == null) return;

        Vector3 forward = xrCameraTransform.forward;

        // Posición base frente al jugador
        Vector3 targetPosition = xrCameraTransform.position + forward * canvasDistance;

        // Baja el canvas para que quede en la parte inferior del campo visual
        targetPosition += Vector3.down * 0.2f;

        vrInventoryCanvas.transform.position = targetPosition;
        vrInventoryCanvas.transform.rotation = Quaternion.LookRotation(forward);
        vrInventoryCanvas.transform.localScale = new Vector3(0.006f, 0.006f, 0.006f);
    }

    // ─── Slots ────────────────────────────────────────────────────────────────

    private void FillSlot(int slotIndex, Sprite sprite)
    {
        if (slotIndex >= slots.Length || slots[slotIndex] == null) return;
        slots[slotIndex].sprite = sprite;
        var c = slots[slotIndex].color;
        c.a = 1f;
        slots[slotIndex].color = c;
    }

    private void ClearSlot(int slotIndex)
    {
        if (slotIndex >= slots.Length || slots[slotIndex] == null) return;
        slots[slotIndex].sprite = null;
        var c = slots[slotIndex].color;
        c.a = 0f;
        slots[slotIndex].color = c;
    }
}