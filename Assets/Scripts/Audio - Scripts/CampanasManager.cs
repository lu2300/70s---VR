using UnityEngine;

public class BellManager : MonoBehaviour
{
    [Header("--- AUDIO ---")]
    public AudioSource audioSource;
    public AudioClip sonidoCampana;

    [Header("--- TIEMPOS ---")]
    public float primeraSonada = 90f;    // 1:30 minutos
    public float intervalo = 600f;       // cada 10 minutos

    private float timer = 0f;
    private bool primeraVez = true;

    void Update()
    {
        timer += Time.deltaTime;

        if (primeraVez)
        {
            if (timer >= primeraSonada)
            {
                TocarCampana();
                primeraVez = false;
                timer = 0f; // reinicia para contar los 10 minutos
            }
        }
        else
        {
            if (timer >= intervalo)
            {
                TocarCampana();
                timer = 0f;
            }
        }
    }

    void TocarCampana()
    {
        if (audioSource && sonidoCampana)
        {
            audioSource.PlayOneShot(sonidoCampana);
            Debug.Log("🔔 Campanas sonando!");
        }
    }
}