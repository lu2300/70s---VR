using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TejoExplosion : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip explosionClip;

    [Header("Efectos (opcional)")]
    public ParticleSystem explosionEffect;

    [Header("Configuración")]
    public string mechaTag = "Mecha";

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(mechaTag))
        {
            Explotar(collision.GetContact(0).point);
        }
    }

    void Explotar(Vector3 punto)
    {
        if (explosionClip != null)
            audioSource.PlayOneShot(explosionClip);

        if (explosionEffect != null)
        {
            explosionEffect.transform.position = punto;
            explosionEffect.Play();
        }
    }
}