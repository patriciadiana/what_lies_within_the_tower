using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public List<string> itemNames;
    public Dictionary<string, int> itemCounts;

    public InventoryData(Inventory inventory)
    {
        itemNames = new List<string>(inventory.GetItems());
        itemCounts = new Dictionary<string, int>(inventory.GetItemCounts());
    }
}

