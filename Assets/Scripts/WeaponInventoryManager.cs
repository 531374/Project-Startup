using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryManager : MonoBehaviour
{
    [SerializeField] private Transform inventoryParent;
    [SerializeField] private Transform panel;
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private TMP_Text weaponDmg;
    [SerializeField] private TMP_Text weaponDurability;
    private InventoryEffect effct;
    private Transform midSlot;
    // Start is called before the first frame update
    void Start()
    {
        effct = GetComponent<InventoryEffect> ();
        UpdateMidChild ();
        effct.inventorySwaped = true;
    }

    // Update is called once per frame
    void Update()
    {
        SetInv ();

        if (inventoryParent.gameObject.activeSelf)
        {
            UpdateMidChild ();
        }
    }

    private void SetInv ()
    {
        if (Input.GetKey (KeyCode.Tab))
        {
            inventoryParent.gameObject.SetActive (true);
            panel.gameObject.SetActive (true);

            weaponName.gameObject.SetActive (true);
            weaponDmg.gameObject.SetActive (true);
            weaponDurability.gameObject.SetActive (true);
            Time.timeScale = 0.3f;
        } else if (Input.GetKeyUp (KeyCode.Tab))
        {
            inventoryParent.gameObject.SetActive (false);
            panel.gameObject.SetActive (false);
            weaponName.gameObject.SetActive (false);
            weaponDmg.gameObject.SetActive (false);
            weaponDurability.gameObject.SetActive (false);
            Time.timeScale = 1f;
        }
    }

    private void UpdateMidChild ()
    {
        weaponName.text = "";
        weaponDmg.text = "";
        weaponDurability.text = "";


        if (effct.inventorySwaped == false) return;


        foreach (Transform child in inventoryParent)
        {
            if (child.localPosition == new Vector3 (0, 0 ,0))
            {
                midSlot = child;
                GameObject boarder = child.GetChild (0).gameObject;
                boarder.GetComponent<Button> ().Select ();
                Color color = boarder.GetComponent<Image> ().color;
                color.a = 255;
                child.GetChild (0).GetComponent <Image> ().color = color;

                InventorySlot inventorySlot = boarder.GetComponent<InventorySlot>();
                if (inventorySlot.item == null) return;
                Weapon item = (Weapon)inventorySlot.item;

                weaponName.text = item.name;
                weaponDmg.text = "Damage:" + item.attack;
                weaponDurability.text = "Durability:" + item.durability;
            }
            else
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                Color color = child.GetChild (0).GetComponent<Image> ().color;
                color.a = 170;
                child.GetChild (0).GetComponent <Image> ().color = color;

            }
        }
    }
}
