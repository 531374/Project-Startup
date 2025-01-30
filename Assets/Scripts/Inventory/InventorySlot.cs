using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    [HideInInspector] public Item item;
    private bool isSelected;

    void Start ()
    {
        this.GetComponent<Button> ().onClick.AddListener (Select);
    }

    void Select ()
    {
        if (!isSelected)
        {
            isSelected = true;
            this.icon.sprite = item.selectedIcon;
        }

    }
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