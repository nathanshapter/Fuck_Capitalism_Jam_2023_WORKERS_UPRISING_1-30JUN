/**
 *  Derived classes can draw different kind of inventories by inheriting from this base class.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public abstract class InventoryUIBase : MonoBehaviour
{
    private Inventory inventory;
    CompositeDisposable disposables = new CompositeDisposable();

    private void OnEnable()
    {
        inventory = Inventory.Instance;
        inventory.InventoryChanged.Subscribe(b => UpdateInventory(inventory.Items)).AddTo(disposables);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        disposables.Dispose();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        disposables.Dispose();
    }

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        if (inventory != null)
            UpdateInventory(inventory.Items);
    }

    /**
    * Override this in different UI classes to update inventory visuals
    * @param _items Dictionary of inventory items.
    */
    public abstract void UpdateInventory(Dictionary<InventoryItemData, int> _items);
}
