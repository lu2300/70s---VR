using UnityEngine;

public class NPCproximidad : MonoBehaviour
{
    [Header("Players")]
    public Transform playerVR;
    public Transform playerPC;

    private Transform playerActual;

    public float distanciaActivacion = 3f;
    public Animator animator;

    [Header("Animación")]
    public string triggerNombre = "Saludar";

    private bool yaActivado = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 🔄 Elegir automáticamente cuál usar
        if (playerVR != null && playerVR.gameObject.activeInHierarchy)
        {
            playerActual = playerVR;
        }
        else if (playerPC != null && playerPC.gameObject.activeInHierarchy)
        {
            playerActual = playerPC;
        }

        if (playerActual == null || animator == null) return;

        float distancia = Vector3.Distance(transform.position, playerActual.position);

        if (distancia <= distanciaActivacion && !yaActivado)
        {
            animator.SetTrigger(triggerNombre);
            yaActivado = true;
        }

        if (distancia > distanciaActivacion)
        {
            yaActivado = false;
        }
    }
}