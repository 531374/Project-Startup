using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int inventorySize;
    public static Inventory instance { get; private set; }

    private void Awake ()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }

    public List<Item> items= new List<Item>();
    public delegate void OnItemChanged ();
    public OnItemChanged onItemChangedCallback;

    public bool Add (Item item)
    {
        if (items.Count >= inventorySize) return false;

        items.Add(item);

        if (onItemChangedCallback != null) onItemChangedCallback.Invoke();

        return true;
    }

    public void Remove (Item item)
    {
        items.Remove(item);
        if (onItemChangedCallback != null) onItemChangedCallback.Invoke(); 
    }
}
