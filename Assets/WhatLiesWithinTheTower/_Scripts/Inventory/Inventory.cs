using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private HashSet<string> items = new HashSet<string>();

    public Transform inventorySlotsParent;
    public Sprite keySprite;
    public Sprite potionSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
                else if (itemName == "Potion")
                {
                    slotImage.sprite = potionSprite;
                    slotImage.enabled = true;
                    slotImage.color = Color.white;
                }
                return;
            }
        }
    }

    private void RemoveItemFromUI(string itemName)
    {
        Sprite itemSprite = null;

        if (itemName == "TowerKey")
        {
            itemSprite = keySprite;
        }
        else if (itemName == "Potion")
        {
            itemSprite = potionSprite;
        }

        if (itemSprite == null) return;

        foreach (Transform slot in inventorySlotsParent)
        {
            Image slotImage = slot.GetComponentInChildren<Image>();
            if (slotImage != null && slotImage.sprite == itemSprite)
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
