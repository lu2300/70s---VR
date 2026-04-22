using UnityEngine;

/// <summary>
/// Attach this component to every NPC that should have dialogue.
///
/// Requirements:
///  - A Collider (set as Trigger) on the NPC or a child object defines the interaction radius.
///  - The Player GameObject must be tagged "Player".
/// </summary>
public class NPCDialogueTrigger : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────────────────────────
    [Header("Dialogue Content")]
    [Tooltip("ScriptableObject with this NPC's lines.")]
    public DialogueData dialogueData;

    [Header("Animator")]
    public Animator npcAnimator;

    [Header("Trigger Settings")]
    [Tooltip("Auto-creates a sphere trigger if no Collider is found on this object.")]
    public float autoTriggerRadius = 2.5f;

    [Header("Visual Hint (optional)")]
    [Tooltip("GameObject shown above NPC when player is in range (e.g. '!' icon).")]
    public GameObject interactionHint;

    // ─── Unity ────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (GetComponent<Collider>() == null)
        {
            var sc = gameObject.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = autoTriggerRadius;
        }

        if (interactionHint != null)
            interactionHint.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactionHint != null)
            interactionHint.SetActive(true);

        if (dialogueData != null && !DialogueManager.Instance.IsOpen)
        {
            DialogueManager.Instance.StartDialogue(dialogueData, transform, npcAnimator);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactionHint != null)
            interactionHint.SetActive(false);

        if (DialogueManager.Instance.IsOpen)
            DialogueManager.Instance.EndDialogue();
    }
}