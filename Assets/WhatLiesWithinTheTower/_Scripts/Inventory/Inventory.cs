using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private HashSet<string> items = new HashSet<string>();
    private Dictionary<string, int> itemCounts = new Dictionary<string, int>(); 

    public Transform inventorySlotsParent;
    public Sprite keySprite;
    public Sprite potionSprite;
    public Sprite noteSprite;

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
            itemCounts[itemName] = 1; 
        }
        else
        {
            itemCounts[itemName]++; 
        }

        AddItemToUI(itemName);
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            itemCounts[itemName]--;

            if (itemCounts[itemName] <= 0)
            {
                items.Remove(itemName);
                itemCounts.Remove(itemName);
            }

            RemoveItemFromUI(itemName);
        }
    }

    private void AddItemToUI(string itemName)
    {
        foreach (Transform slot in inventorySlotsParent)
        {
            Image slotImage = slot.GetComponentInChildren<Image>();
            TextMeshProUGUI countText = slot.GetComponentInChildren<TextMeshProUGUI>();

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
                else if (itemName == "2DNote")
                {
                    slotImage.sprite = noteSprite;
                    slotImage.enabled = true;
                    slotImage.color = Color.white;

                    if (countText != null)
                    {
                        countText.text = itemCounts[itemName].ToString();
                        countText.enabled = true;
                    }
                }
                return;
            }
            else if (slotImage != null && slotImage.sprite == noteSprite && itemName == "2DNote")
            {
                if (countText != null)
                {
                    countText.text = itemCounts[itemName].ToString();
                    countText.enabled = true;
                    if(itemCounts[itemName] == 4)
                    {
                        GameManager.Instance.SetLevelComplete(4);
                        Inventory.Instance.ResetInventory();
                        SceneManager.LoadScene("MainScene");
                    }
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
        else if (itemName == "2DNote")
        {
            itemSprite = noteSprite;
        }

        if (itemSprite == null) return;

        foreach (Transform slot in inventorySlotsParent)
        {
            Image slotImage = slot.GetComponentInChildren<Image>();
            Text countText = slot.GetComponentInChildren<Text>();

            if (slotImage != null && slotImage.sprite == itemSprite)
            {
                if (itemName == "2DNote")
                {
                    if (itemCounts.ContainsKey(itemName))
                    {
                        countText.text = itemCounts[itemName].ToString();
                        if (itemCounts[itemName] <= 0)
                        {
                            slotImage.sprite = null;
                            slotImage.enabled = false;
                            slotImage.color = Color.white;
                            countText.enabled = false;
                        }
                    }
                }
                else
                {
                    slotImage.sprite = null;
                    slotImage.enabled = false;
                    slotImage.color = Color.white;
                    if (countText != null)
                    {
                        countText.enabled = false;
                    }
                }
                return;
            }
        }
    }
    public List<string> GetItems()
    {
        return new List<string>(items);
    }

    public Dictionary<string, int> GetItemCounts()
    {
        return new Dictionary<string, int>(itemCounts);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void LoadInventory(InventoryData inventoryData)
    {
        items.Clear();
        itemCounts.Clear();

        foreach (var itemName in inventoryData.itemNames)
        {
            if (!items.Contains(itemName))
            {
                AddItemToUI(itemName);
                items.Add(itemName);
            }
        }

        foreach (var itemCount in inventoryData.itemCounts)
        {
            itemCounts[itemCount.Key] = itemCount.Value;
        }
    }

    public void ResetInventory()
    {
        items.Clear();
        itemCounts.Clear();

        foreach (Transform slot in inventorySlotsParent)
        {
            Image slotImage = slot.GetComponentInChildren<Image>();
            TextMeshProUGUI countText = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (slotImage != null)
            {
                slotImage.sprite = null;
                slotImage.enabled = false;
            }

            if (countText != null)
            {
                countText.text = "";
                countText.enabled = false;
            }
        }
    }

}