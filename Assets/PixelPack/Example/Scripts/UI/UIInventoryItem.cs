/**
 *  Inventory item UI element's controller
 */

using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
    public Image FrameImage;
    public Image ItemImage;
    public Text AmountText;

    public Sprite FrameEnabledSprite;
    public Sprite FrameDisabledSprite;
    public bool IsEnabled;

    public InventoryItemData ItemData { get; private set; }

    public int Amount { get; private set; } = 0;
    /**
    * Enable or disable item (change frame sprite)
    * @param _enabled Enables or disables this item.
    */
    public void SetEnabled(bool _enabled)
    {
        IsEnabled = _enabled;
        FrameImage.sprite = IsEnabled ? FrameEnabledSprite : FrameDisabledSprite;
    }

    public void SetItemInfo(InventoryItemData _itemData, int _amount)
    {
        Amount = _amount;
        AmountText.text = Amount.ToString();
        ItemImage.sprite = _itemData.InventoryIcon;
        ItemData = _itemData;
    }
}
