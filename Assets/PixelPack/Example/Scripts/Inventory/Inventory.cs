using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class Inventory : Singleton<Inventory>
{
    public BooleanNotifier InventoryChanged = new BooleanNotifier(false);    
    public ReactiveProperty<InventoryItemData> InventoryItemUsed = new ReactiveProperty<InventoryItemData>();
    public Dictionary<InventoryItemData, int> Items = new Dictionary<InventoryItemData, int>();
    public void AddItem(InventoryItemData item)
    {
        if (Items.ContainsKey(item))
            if (item.Unique)
            {
                Debug.Log("This is unique item.");
                return; //<-- already has one
            }
            else
            {
                Items[item]++; //<-- stackable item
                Debug.Log("You now have " + Items[item] + " " + item.ItemName + "(s)");
            }
        else
        {
            Debug.Log("Added item: " + item.ItemName);
            Items.Add(item, 1); //<-- add one
        }

        InventoryChanged.SwitchValue();
    }

    public void RemoveItem(InventoryItemData _item)
    {
        if (Items[_item] > 1)
            Items[_item]--; //<-- use one stackable item
        else
            Items.Remove(_item);

        InventoryChanged.SwitchValue();
    }

    public void UseItem(InventoryItemData _item)
    {
        RemoveItem(_item);
        InventoryItemUsed.Value = _item;
        //TODO: kinda hackish, but felt better than event:
        InventoryItemUsed.Value = null;
    }
}
