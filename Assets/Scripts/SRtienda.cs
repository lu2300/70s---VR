using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SRtienda : MonoBehaviour
{
    [Header("Referencia al dinero")]
    public GameObject itemDinero;

    [Header("Animador del NPC")]
    public Animator npcAnimator;
    public string triggerCompra = "Comprar"; // nombre del trigger

    [Header("Punto de interacción (opcional)")]
    public Transform interactionPoint;

    private bool isInteracting = false;

    // Player refs
    private FirstPersonController playerController;
    private DynamicMoveProvider moveProvider;
    private Rigidbody playerRigidbody;

    // Estado original
    private bool originalPlayerCanMove;
    private bool originalCameraCanMove;
    private bool originalHeadBob;
    private bool moveProviderWasEnabled;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteracting)
        {
            InitPlayer(other.gameObject);
            StartCoroutine(Interaccion());
        }
    }

    void InitPlayer(GameObject player)
    {
        playerController = player.GetComponent<FirstPersonController>();
        moveProvider = player.GetComponent<DynamicMoveProvider>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        if (playerController != null)
        {
            originalPlayerCanMove = playerController.playerCanMove;
            originalCameraCanMove = playerController.cameraCanMove;
            originalHeadBob = playerController.enableHeadBob;
        }

        if (moveProvider != null)
            moveProviderWasEnabled = moveProvider.enabled;
    }

    IEnumerator Interaccion()
    {
        isInteracting = true;

        // 🔒 BLOQUEAR
        LockPlayer();

        // 🎯 MIRAR AL NPC (opcional)
        if (interactionPoint != null)
        {
            Vector3 dir = interactionPoint.position - playerController.transform.position;
            dir.y = 0;

            if (dir != Vector3.zero)
                playerController.transform.rotation = Quaternion.LookRotation(dir);
        }

        yield return new WaitForSeconds(0.2f);

        // 💰 VERIFICAR DINERO
        if (itemDinero != null && itemDinero.activeSelf)
        {
            Debug.Log("🛒 Gracias por su compra");

            // 🎭 ACTIVAR ANIMACIÓN POR TRIGGER
            if (npcAnimator != null && !string.IsNullOrEmpty(triggerCompra))
            {
                npcAnimator.SetTrigger(triggerCompra);
            }
        }
        else
        {
            Debug.Log("❌ No tienes dinero");
        }

        yield return new WaitForSeconds(2f);

        // 🔓 DESBLOQUEAR
        RestorePlayer();

        isInteracting = false;
    }

    void LockPlayer()
    {
        if (playerController != null)
        {
            playerController.playerCanMove = false;
            playerController.cameraCanMove = false;
            playerController.enableHeadBob = false;
        }

        if (moveProvider != null)
            moveProvider.enabled = false;

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector3.zero;
    }

    void RestorePlayer()
    {
        if (playerController != null)
        {
            playerController.playerCanMove = originalPlayerCanMove;
            playerController.cameraCanMove = originalCameraCanMove;
            playerController.enableHeadBob = originalHeadBob;
        }

        if (moveProvider != null)
            moveProvider.enabled = moveProviderWasEnabled;
    }
}