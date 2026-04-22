using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

[System.Serializable]
public class AnimationStep
{
    public string stateName;
    public float duration;
}

public class NPCInteraction : MonoBehaviour
{
    [Header("Animación NPC")]
    public Animator npcAnimator;
    public List<AnimationStep> animationSequence = new List<AnimationStep>();

    private bool isInteracting = false;

    // Referencias jugador
    private FirstPersonController playerController;
    private DynamicMoveProvider moveProvider;
    private Rigidbody playerRigidbody;

    // Estado original
    private bool originalPlayerCanMove;
    private bool originalCameraCanMove;
    private bool originalHeadBob;
    private bool moveProviderWasEnabled;
    private Vector3 originalVelocity;
    private Vector3 originalAngularVelocity;

    void Start()
    {
        if (npcAnimator != null)
            npcAnimator.applyRootMotion = false;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteracting)
        {
            InitializePlayer(other.gameObject);
            StartCoroutine(InteractionSequence(other.gameObject));
        }
    }

    void InitializePlayer(GameObject player)
    {
        playerController = player.GetComponent<FirstPersonController>();
        moveProvider = player.GetComponent<DynamicMoveProvider>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        SaveState();
    }

    void SaveState()
    {
        if (playerController != null)
        {
            originalPlayerCanMove = playerController.playerCanMove;
            originalCameraCanMove = playerController.cameraCanMove;
            originalHeadBob = playerController.enableHeadBob;
        }

        if (moveProvider != null)
            moveProviderWasEnabled = moveProvider.enabled;

        if (playerRigidbody != null)
        {
            originalVelocity = playerRigidbody.linearVelocity;
            originalAngularVelocity = playerRigidbody.angularVelocity;
        }
    }

    IEnumerator InteractionSequence(GameObject player)
    {
        isInteracting = true;

        // 🔒 BLOQUEAR
        LockPlayer();

        // 🎭 ANIMACIONES
        if (npcAnimator != null)
        {
            foreach (AnimationStep step in animationSequence)
            {
                if (!string.IsNullOrEmpty(step.stateName))
                {
                    npcAnimator.Play(step.stateName, 0, 0f);
                    yield return new WaitForSeconds(step.duration);
                }
            }
        }


        // 🔓 DESBLOQUEAR
        RestorePlayer();
        isInteracting = false;
    }

    void LockPlayer()
    {
        if (playerController != null)
        {
            // ❌ NO bloquear la cámara
            // playerController.cameraCanMove = false;

            // ✅ Solo bloquear movimiento
            playerController.playerCanMove = false;

            // Opcional: quitar headbob si quieres
            playerController.enableHeadBob = false;
        }

        // ✅ Esto es lo importante en VR: desactiva locomotion
        if (moveProvider != null)
            moveProvider.enabled = false;

        // Opcional: parar física
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void RestorePlayer()
    {
        if (playerController != null)
        {
            playerController.playerCanMove = originalPlayerCanMove;

            // ❌ NO tocar la cámara
            // playerController.cameraCanMove = originalCameraCanMove;

            playerController.enableHeadBob = originalHeadBob;
        }

        if (moveProvider != null)
            moveProvider.enabled = moveProviderWasEnabled;

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = originalVelocity;
            playerRigidbody.angularVelocity = originalAngularVelocity;
        }
    }
}