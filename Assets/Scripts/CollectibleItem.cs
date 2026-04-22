using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private Sprite inventorySprite;

    private XRGrabInteractable grabInteractable;
    private bool collected = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (collected) return;
        collected = true;

        InventoryManager.Instance.CollectItem(inventorySprite);
        gameObject.SetActive(false);
    }
}
