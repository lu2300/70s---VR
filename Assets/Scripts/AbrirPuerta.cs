using UnityEngine;

public class AbrirCerrarPuerta : MonoBehaviour
{
    public float anguloApertura = 90f;
    public float velocidad = 2f;
    private bool abierta = false;

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionFinal = Quaternion.Euler(transform.eulerAngles + new Vector3(0, anguloApertura, 0));
    }

    void Update()
    {
        // Interpolamos hacia la rotación deseada según el estado "abierta"
        if (abierta)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionFinal, Time.deltaTime * velocidad);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionInicial, Time.deltaTime * velocidad);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            abierta = true; // Abrir al tocar
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            abierta = false; // Cerrar al alejarse
        }
    }
}