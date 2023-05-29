/**
 *  In-game UI. Inventory, coins and time.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IngameUiController : InventoryUIBase, IUiBase
{
    public RectTransform ItemParentContainer;
    public UIInventoryItem InventorySlotObject;
    public Text TextCoins;
    public Text TextTime;
    public UIInventoryItem CurrentlySelectedItem { get; private set; }

    private List<UIInventoryItem> InventoryItems = new List<UIInventoryItem>();    
    private int currentlySelectedItemIndex;

    /**
    * Update the inventory UI. Clear old icons and render new item icons.
    * @param _items Dictionary of InventoryItemData objects.
    */
    public override void UpdateInventory(Dictionary<InventoryItemData, int> _items)
    {
        ClearOldIcons();

        if (_items.Count > 0)
            RenderNewIcons(_items);
    }

    /**
    * Update the amount of coins to Text component text
    * @param currentCoins Current amount of coins        
    */
    public void UpdateTextCoins(int currentCoins)
    {
        TextCoins.text = "x" + currentCoins.ToString("00");
    }

    /**
    * Update the amount of time to Text component text
    * @param timeLeft Time left to pass the level (not further implemented)
    */
    public void UpdateTextTime(int timeLeft)
    {
        TextTime.text = timeLeft.ToString("000");
    }

    /**
    * Select item to use with d-pad or e/r keys
    * @param leftRight Select item left (-1) or right (1)
    */

    public void SelectItem(int leftRight)
    {
        if (!InventoryItems.Any())
            return;

        //Disable all items
        InventoryItems.ForEach(item => item.SetEnabled(false));

        //Scroll index left or right
        currentlySelectedItemIndex += leftRight;

        if (currentlySelectedItemIndex >= InventoryItems.Count)
            currentlySelectedItemIndex = 0;
        else if (currentlySelectedItemIndex < 0)
            currentlySelectedItemIndex = InventoryItems.Count - 1;

        //Enable new current item
        CurrentlySelectedItem = InventoryItems[currentlySelectedItemIndex];
        CurrentlySelectedItem.SetEnabled(true);
    }

    /**
    * Use CurrentlySelected item from Inventory
    */
    public void UseItem()
    {
        if (CurrentlySelectedItem != null)
        {
            Inventory.Instance.UseItem(CurrentlySelectedItem.ItemData);
        }
    }


    /**
    * Implement IUIbase interface to show/hide in-game UI
    * @param show Show or hide in-game UI.
    */
    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }


    private void ClearOldIcons()
    {
        //Clear list of items
        InventoryItems = new List<UIInventoryItem>();
        //Remove visual elements
        ItemParentContainer.Cast<Transform>().ToList().ForEach(child =>
        {
            if (child.GetComponent<UIInventoryItem>() != null) Destroy(child.gameObject);
        });
    }

    /**
    * Render inventory item icons.
    * @param _items Dictionary of InventoryItemData objects.
    */
    private void RenderNewIcons(Dictionary<InventoryItemData, int> _items)
    {
        _items.ToList().Where(x => x.Key.ShowInInventory).ToList().ForEach(item => InstantiateNewInventorySlot(item.Key, item.Value));

        if (currentlySelectedItemIndex > InventoryItems.Count - 1)
            currentlySelectedItemIndex = InventoryItems.Count - 1;

        CurrentlySelectedItem = InventoryItems[currentlySelectedItemIndex];
        CurrentlySelectedItem.SetEnabled(true);
    }

    private void InstantiateNewInventorySlot(InventoryItemData _itemData, int _stackedAmount)
    {
        UIInventoryItem itemSlot = Instantiate<UIInventoryItem>(InventorySlotObject, ItemParentContainer);
        itemSlot.gameObject.SetActive(true);
        itemSlot.gameObject.transform.localPosition = Vector3.zero;
        itemSlot.gameObject.transform.localScale = Vector3.one;
        itemSlot.SetItemInfo(_itemData, _stackedAmount);
        //Add new UIInventoryItem to InventoryItems list
        InventoryItems.Add(itemSlot);
    }
}
