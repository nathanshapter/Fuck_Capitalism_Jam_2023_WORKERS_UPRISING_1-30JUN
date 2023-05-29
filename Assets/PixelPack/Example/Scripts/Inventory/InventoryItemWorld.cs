using UnityEngine;

public class InventoryItemWorld : PlatformerObject
{
    public InventoryItemData ItemData;

    public void Collect()
    {
        if (!Inventory.Instance.Items.ContainsKey(ItemData) || ItemData.StackOrRecollectUnique)
        {
            gameObject.SetActive(false);
            Inventory.Instance.AddItem(ItemData);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerCharacter>() != null)
            Collect();
    }
}
