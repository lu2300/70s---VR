using UnityEngine;
using UnityEngine.InputSystem;

public class PuertaInteractiva : MonoBehaviour
{
    [Header("Puerta")]
    public float anguloApertura = 90f;
    public float velocidad = 2f;

    [Header("Input VR")]
    public InputActionReference botonVR_Derecha;
    public InputActionReference botonVR_Izquierda;

    private bool abierta = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionFinal = Quaternion.Euler(transform.eulerAngles + new Vector3(0, anguloApertura, 0));

        if (botonVR_Derecha != null)  botonVR_Derecha.action.Enable();
        if (botonVR_Izquierda != null) botonVR_Izquierda.action.Enable();
    }

    void Update()
    {
        Quaternion objetivo = abierta ? rotacionFinal : rotacionInicial;
        transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * velocidad);

        // --- Input Mouse ---
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
                if (hit.transform == transform)
                    AlternarPuerta();
        }

        // --- Input VR ---
        if (BotonPresionado(botonVR_Derecha) || BotonPresionado(botonVR_Izquierda))
        {
            Ray rayVR = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(rayVR, out RaycastHit hitVR, 5f))
                if (hitVR.transform == transform)
                    AlternarPuerta();
        }
    }

    bool BotonPresionado(InputActionReference boton)
    {
        return boton != null && boton.action.WasPressedThisFrame();
    }

    void AlternarPuerta()
    {
        abierta = !abierta;
    }
}