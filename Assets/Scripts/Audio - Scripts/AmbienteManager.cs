using UnityEngine;

public class AmbientManager : MonoBehaviour
{
    [Header("--- AUDIO ---")]
    public AudioSource sonidoAmbiente;

    [Header("--- CONFIG ---")]
    public float volumenAfuera = 1f;
    public float volumenAdentro = 0f;
    public float velocidadTransicion = 2f;

    private float volumenObjetivo;

    void Start()
    {
        volumenObjetivo = volumenAfuera;
    }

    void Update()
    {
        // Transición suave de volumen
        sonidoAmbiente.volume = Mathf.Lerp(
            sonidoAmbiente.volume,
            volumenObjetivo,
            Time.deltaTime * velocidadTransicion
        );
    }

    public void EntrarCasa()
    {
        volumenObjetivo = volumenAdentro;
        Debug.Log("🏠 Dentro de la casa");
    }

    public void SalirCasa()
    {
        volumenObjetivo = volumenAfuera;
        Debug.Log("🌿 Fuera de la casa");
    }
}