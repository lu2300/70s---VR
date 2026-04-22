using UnityEngine;
using UnityEngine.InputSystem; // Necesario para InputActionReference

public class PuertaInteractiva : MonoBehaviour
{
    [Header("Puerta")]
    public float anguloApertura = 90f;
    public float velocidad = 2f;

    [Header("Input VR")]
    public InputActionReference botonVR; // Botón de VR para abrir/cerrar

    private bool abierta = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionFinal = Quaternion.Euler(transform.eulerAngles + new Vector3(0, anguloApertura, 0));

        if (botonVR != null)
            botonVR.action.Enable();
    }

    void Update()
    {
        // Interpolamos hacia la rotación deseada
        Quaternion objetivo = abierta ? rotacionFinal : rotacionInicial;
        transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * velocidad);

        // --- Input Mouse ---
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (hit.transform == transform)
                {
                    AlternarPuerta();
                }
            }
        }

        // --- Input VR ---
        if (botonVR != null && botonVR.action.WasPressedThisFrame())
        {
            // Raycast desde la cámara del jugador para detectar la puerta
            Ray rayVR = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(rayVR, out RaycastHit hitVR, 5f))
            {
                if (hitVR.transform == transform)
                {
                    AlternarPuerta();
                }
            }
        }
    }

    void AlternarPuerta()
    {
        abierta = !abierta; // Alterna entre abrir y cerrar
    }
}