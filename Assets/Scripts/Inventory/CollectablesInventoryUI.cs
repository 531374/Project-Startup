using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectablesInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemsParent;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    Inventory inventory;
    InventorySlot[] slots;
    private InventorySlot displayingSlot = null;
    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onCollectableItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        foreach (var slot in slots)
        {
            slot.GetComponent<Button> ().onClick.AddListener (() => DisplayInformation (slot));
        }
    }

    private void DisplayInformation (InventorySlot slot)
    {
        if (slot.item == null)
        {
            itemName.gameObject.SetActive (false);
            itemDescription.gameObject.SetActive (false);
            itemName.text = "";
            itemDescription.text = "";
            displayingSlot = null;
            return;
        }
        
        if (displayingSlot != slot)
        {
            itemName.gameObject.SetActive(true);
            itemDescription.gameObject.SetActive(true);
            itemName.text = slot.item.name;
            itemDescription.text = slot.item.description;
            slot.GetComponent<Button>().Select();
            displayingSlot = slot;
        }
        else
        {
            itemName.gameObject.SetActive(false);
            itemDescription.gameObject.SetActive(false);
            itemName.text = "";
            itemDescription.text = "";

            // Deselect the button
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            displayingSlot = null;
        }

    }

    void UpdateUI ()
    {
        for (int i =0; i < slots.Length; i++)
        {
            if (i < inventory.collectableItems.Count)
            {
                slots[i].AddItem(inventory.collectableItems[i]);
            } else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
