using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    [HideInInspector] public Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
        if (icon != null) // Check if the icon reference is valid
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
    }

    public void ClearSlot()
    {
        item = null;

        if (icon != null) // Check if the icon reference is valid
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}