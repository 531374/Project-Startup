using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CollectablesInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemsParent;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    Inventory inventory;
    InventorySlot[] slots;
    private InventorySlot displayingSlot = null;

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reinitialize the UI references
        InitializeUI();
    }

    void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        inventory = Inventory.instance;
        inventory.onCollectableItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        foreach (var slot in slots)
        {
            slot.GetComponent<Button>().onClick.AddListener(() => DisplayInformation(slot));
        }

        // Force an update of the UI when the scene loads
        UpdateUI();
    }

    private void DisplayInformation(InventorySlot slot)
    {
        if (slot.item == null)
        {
            itemName.gameObject.SetActive(false);
            itemDescription.gameObject.SetActive(false);
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

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.collectableItems.Count)
            {
                slots[i].AddItem(inventory.collectableItems[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}