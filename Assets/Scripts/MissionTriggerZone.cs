using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MissionTriggerZone : MonoBehaviour
{
    [Tooltip("El MissionNPCController del NPC al que pertenece esta zona")]
    public MissionNPCController npcController;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && npcController != null)
            npcController.TriggerMissionSequence();
    }
}