using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class UIManager : MonoBehaviour
{
    [Header("--- ESCRITORIO ---")]
    public GameObject startScreen;
    public GameObject menuPrincipal;
    public GameObject configuraciones;
    public GameObject tutorial;

    [Header("--- VR ---")]
    public GameObject startScreen_VR;
    public GameObject menuPrincipal_VR;
    public GameObject configuraciones_VR;
    public GameObject tutorial_VR;

    [Header("--- Canvas raíz ---")]
    public GameObject canvasDesktop;
    public GameObject canvasVR;

    [Header("--- Managers ---")]
    public FadeManager fadeManager;

    [Header("--- TESTING ---")]
    public bool forzarModoVR = false;

    private bool esVR;

    void Start()
    {
        Invoke(nameof(CheckUI), 0.5f);
    }

    void CheckUI()
    {
        esVR = XRSettings.isDeviceActive || forzarModoVR;

        Debug.Log("¿Es VR? " + esVR);

        canvasDesktop.SetActive(!esVR);
        canvasVR.SetActive(esVR);

        if (esVR)
            MostrarSolo_VR(startScreen_VR);
        else
            MostrarSolo(startScreen);
    }

    void Update()
    {
        // Escritorio → Espacio o Enter
        if (!esVR && startScreen.activeSelf &&
        (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            StartCoroutine(CambiarPantalla(ShowMenu));
        }

        // VR → Botón A del control derecho
        if (esVR && startScreen_VR.activeSelf)
        {
            var rightHandDevices = new System.Collections.Generic.List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
                rightHandDevices
            );

            if (rightHandDevices.Count > 0)
            {
                rightHandDevices[0].TryGetFeatureValue(CommonUsages.primaryButton, out bool botonA);
                if (botonA)
                {
                    StartCoroutine(CambiarPantalla(ShowMenu_VR));
                }
            }
        }
    }

    // ─── ESCRITORIO ───────────────────────────────
    void MostrarSolo(GameObject pantalla)
    {
        startScreen.SetActive(pantalla == startScreen);
        menuPrincipal.SetActive(pantalla == menuPrincipal);
        configuraciones.SetActive(pantalla == configuraciones);
        tutorial.SetActive(pantalla == tutorial);
    }

    public void ShowComienzo()          => MostrarSolo(startScreen);
    public void ShowMenu()              => MostrarSolo(menuPrincipal);
    public void ShowConfiguraciones()   => MostrarSolo(configuraciones);
    public void ShowTutorial()          => MostrarSolo(tutorial);

    // ─── VR ───────────────────────────────────────
    void MostrarSolo_VR(GameObject pantalla)
    {
        startScreen_VR.SetActive(pantalla == startScreen_VR);
        menuPrincipal_VR.SetActive(pantalla == menuPrincipal_VR);
        configuraciones_VR.SetActive(pantalla == configuraciones_VR);
        tutorial_VR.SetActive(pantalla == tutorial_VR);
    }

    public void ShowComienzo_VR()           => MostrarSolo_VR(startScreen_VR);
    public void ShowMenu_VR()               => MostrarSolo_VR(menuPrincipal_VR);
    public void ShowConfiguraciones_VR()    => MostrarSolo_VR(configuraciones_VR);
    public void ShowTutorial_VR()           => MostrarSolo_VR(tutorial_VR);

    // ─── NAVEGACIÓN CON FADE ──────────────────────
    IEnumerator CambiarPantalla(System.Action accion)
    {
        yield return StartCoroutine(fadeManager.FadeOut());
        accion.Invoke();
        yield return StartCoroutine(fadeManager.FadeIn());
    }

    // Botones Escritorio
    public void IrMenu()            => StartCoroutine(CambiarPantalla(ShowMenu));
    public void IrConfiguraciones() => StartCoroutine(CambiarPantalla(ShowConfiguraciones));
    public void IrTutorial()        => StartCoroutine(CambiarPantalla(ShowTutorial));
    public void IrComienzo()        => StartCoroutine(CambiarPantalla(ShowComienzo));

    // Botones VR
    public void IrMenu_VR()             => StartCoroutine(CambiarPantalla(ShowMenu_VR));
    public void IrConfiguraciones_VR()  => StartCoroutine(CambiarPantalla(ShowConfiguraciones_VR));
    public void IrTutorial_VR()         => StartCoroutine(CambiarPantalla(ShowTutorial_VR));
    public void IrComienzo_VR()         => StartCoroutine(CambiarPantalla(ShowComienzo_VR));
}