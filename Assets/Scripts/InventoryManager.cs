using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public const int SlotCount = 3;

    private Sprite[] collectedItems = new Sprite[SlotCount];
    private int nextSlot = 0;

    public event Action<int, Sprite> OnItemCollected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CollectItem(Sprite itemSprite)
    {
        if (nextSlot >= SlotCount) return;

        collectedItems[nextSlot] = itemSprite;
        OnItemCollected?.Invoke(nextSlot, itemSprite);
        nextSlot++;
    }

    public Sprite GetItem(int index)
    {
        if (index < 0 || index >= collectedItems.Length) return null;
        return collectedItems[index];
    }
}