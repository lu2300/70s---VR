using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNPCController : MonoBehaviour
{
    [Header("Secuencia de Misión")]
    public List<MissionSequenceStep> sequenceSteps = new List<MissionSequenceStep>();

    [Header("Componentes del NPC")]
    public Animator npcAnimator;
    public AudioSource audioSource;

    [Header("Sistema de Diálogos")]
    [Tooltip("Arrastra aquí el componente de diálogo del hijo Capsule")]
    public MonoBehaviour dialogueComponent;

    [Header("Retorno al Idle")]
    [Tooltip("Nombre del Trigger de idle en el Animator. Dejar vacío si el Animator lo maneja solo.")]
    public string returnToIdleTrigger = "";

    private bool _sequenceCompleted = false;
    private bool _sequencePlaying = false;

    public void TriggerMissionSequence()
    {
        if (_sequenceCompleted || _sequencePlaying) return;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        _sequencePlaying = true;

        if (dialogueComponent != null)
            dialogueComponent.enabled = false;

        foreach (var step in sequenceSteps)
        {
            if (!string.IsNullOrEmpty(step.animatorTriggerName) && npcAnimator != null)
                npcAnimator.SetTrigger(step.animatorTriggerName);

            if (step.audioClip != null && audioSource != null)
            {
                audioSource.clip = step.audioClip;
                audioSource.Play();
            }

            yield return new WaitForSeconds(step.GetDuration());
        }

        if (!string.IsNullOrEmpty(returnToIdleTrigger) && npcAnimator != null)
            npcAnimator.SetTrigger(returnToIdleTrigger);

        if (dialogueComponent != null)
            dialogueComponent.enabled = true;

        _sequenceCompleted = true;
        _sequencePlaying = false;
    }
}