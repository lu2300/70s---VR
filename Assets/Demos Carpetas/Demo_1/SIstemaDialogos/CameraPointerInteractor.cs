using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach to the Player Camera (or VR camera rig).
///
/// This script does TWO things:
///   1. Mouse / Crosshair mode  – shoots a Physics ray from the screen center
///      and simulates a pointer-click on UI Buttons in World Space.
///   2. VR mode                 – relies on Unity's XR Interaction Toolkit or
///      a custom VR ray; see the VRPointerForwarder helper below.
///
/// Setup checklist:
///   ✔ Main Camera must have a PhysicsRaycaster component (for 3-D colliders)
///     AND the World-Space Canvas needs a GraphicRaycaster.
///   ✔ The scene must have an EventSystem (auto-created by Unity UI).
///   ✔ For Gaze/Crosshair: assign gazeInputModule if you use GazeInputModule,
///     otherwise this script handles it manually.
/// </summary>
public class CameraPointerInteractor : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────────────────────────
    [Header("Crosshair / Gaze Settings")]
    [Tooltip("Maximum distance to detect World Space UI.")]
    public float rayDistance = 10f;

    [Tooltip("Layer mask for the dialogue panel collider.")]
    public LayerMask uiLayerMask = ~0;

    [Tooltip("Show a visual indicator when hovering a button.")]
    public GameObject hoverIndicator;

    [Header("Input")]
    [Tooltip("Key / button that confirms selection (keyboard, gamepad, or VR trigger).")]
    public KeyCode    confirmKey         = KeyCode.Mouse0;
    public string     confirmButtonName  = "Fire1"; // also accepts gamepad

    // ─── Private ──────────────────────────────────────────────────────────────
    private Camera   _cam;
    private Button   _hoveredButton;
    private Button   _lastHovered;

    // ─── Unity ────────────────────────────────────────────────────────────────
    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null) _cam = Camera.main;

        if (hoverIndicator != null)
            hoverIndicator.SetActive(false);
    }

    private void Update()
    {
        DetectHover();
        HandleInput();
    }

    // ─── Private ──────────────────────────────────────────────────────────────

    private void DetectHover()
    {
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, uiLayerMask))
        {
            // Try to find a Button component on the hit object or its parents
            _hoveredButton = hit.collider.GetComponentInParent<Button>();
        }
        else
        {
            _hoveredButton = null;
        }

        // Visual feedback
        if (_hoveredButton != _lastHovered)
        {
            if (_lastHovered != null)
                _lastHovered.OnPointerExit(null);

            if (_hoveredButton != null)
            {
                _hoveredButton.OnPointerEnter(null);
                if (hoverIndicator != null) hoverIndicator.SetActive(true);
            }
            else
            {
                if (hoverIndicator != null) hoverIndicator.SetActive(false);
            }

            _lastHovered = _hoveredButton;
        }
    }

    private void HandleInput()
    {
        bool fired = Input.GetKeyDown(confirmKey);

        // Also support Unity Input Manager button (gamepad / VR)
        try { fired |= Input.GetButtonDown(confirmButtonName); }
        catch { /* button not configured – ignore */ }

        if (fired && _hoveredButton != null && _hoveredButton.interactable)
        {
            _hoveredButton.onClick.Invoke();
        }
    }

    // ─── Public (called from VR forwarding script) ────────────────────────────

    /// <summary>
    /// Call this from a VR raycaster script when the controller trigger fires.
    /// Pass the Button the VR ray is pointing at.
    /// </summary>
    public void OnVRSelect(Button button)
    {
        if (button != null && button.interactable)
            button.onClick.Invoke();
    }
}
