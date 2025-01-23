using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int collectablesInventorySize;
    public int weaponInventorySize;
    public static Inventory instance { get; private set; }

    private void Awake ()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }

    public List<CollectableItem> collectableItems= new List<CollectableItem>();
    public List<Weapon> weapons= new List<Weapon>();
    public delegate void OnItemChanged ();
    public delegate void OnWeaponChanged ();
    public OnItemChanged onCollectableItemChangedCallback;
    public OnWeaponChanged onWeaponChangedCallback;

    public bool AddWeapon (Weapon weapon)
    {
        if (weapons.Count >= weaponInventorySize) return false;

        weapons.Add(weapon);

        if (onWeaponChangedCallback != null) onWeaponChangedCallback.Invoke();

        return true;
    }
    public void RemoveWeapon (Weapon weapon)
    {
        weapons.Remove(weapon);
        if (onWeaponChangedCallback != null) onWeaponChangedCallback.Invoke(); 
    }

    public bool AddCollectable (CollectableItem item)
    {
        if (collectableItems.Count >= collectablesInventorySize) return false;

        collectableItems.Add(item);

        if (onCollectableItemChangedCallback != null) onCollectableItemChangedCallback.Invoke();

        return true;
    }

    public void RemoveCollectable (CollectableItem item)
    {
        collectableItems.Remove(item);
        if (onCollectableItemChangedCallback != null) onCollectableItemChangedCallback.Invoke(); 
    }
}
