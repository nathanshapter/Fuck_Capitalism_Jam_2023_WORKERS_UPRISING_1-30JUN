using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InventoryItem", menuName = "Inventory Item Data", order = 53)]
public class InventoryItemData : ScriptableObject
{
    [SerializeField]
    private string itemName;
    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite inventoryIcon;

    [SerializeField]
    private bool unique;

    [SerializeField]
    private bool stackOrRecollectUnique;

    [SerializeField]
    private bool showInInventory;

    //Sprite icon for inventory
    public Sprite InventoryIcon { get { return inventoryIcon; } }

    //Is this unique item. Only one allowed in inventory even many can appear in world
    public bool Unique { get { return unique; } }

    //Can items be stacked? E.g. coins
    public bool StackOrRecollectUnique { get { return stackOrRecollectUnique; } }

    //Not all items are shown in inventory by default, some may be renderered elsewhere
    public bool ShowInInventory { get { return showInInventory; } }

    public string ItemName { get { return itemName; } }
    public string Description { get { return description; } }
}
