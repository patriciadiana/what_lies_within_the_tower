using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private HashSet<string> items = new HashSet<string>();

    public Transform inventorySlotsParent;
    public Sprite keySprite;

    public void AddItem(string itemName)
    {
        if (!items.Contains(itemName))
        {
            items.Add(itemName);
            AddItemToUI(itemName);
        }
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            RemoveItemFromUI(itemName);
        }
    }

    private void AddItemToUI(string itemName)
    {
        foreach (Transform slot in inventorySlotsParent)
        {
            Image slotImage = slot.GetComponentInChildren<Image>();
            if (slotImage != null && slotImage.sprite == null) 
            {
                if (itemName == "TowerKey")
                {
                    slotImage.sprite = keySprite;
                    slotImage.enabled = true;
                    slotImage.color = Color.white;
                }
                return;
            }
        }
    }

    private void RemoveItemFromUI(string itemName)
    {
        foreach (Transform slot in inventorySlotsParent)
        {
            Image slotImage = slot.GetComponentInChildren<Image>();
            if (slotImage != null && slotImage.sprite == keySprite)
            {
                slotImage.sprite = null;
                slotImage.enabled = false;
                slotImage.color = Color.white;
                return; 
            }
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}
