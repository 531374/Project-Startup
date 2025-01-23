using UnityEngine;

public class InteractableItem : Interactable
{
    public Item item;

    protected override void Interact()
    {
        base.Interact();

        if (interacted)
        {
            if (item is CollectableItem)
            {
                Inventory.instance.AddCollectable ((CollectableItem)item);
            }
            else if (item is Weapon)
            {
                Inventory.instance.AddWeapon ((Weapon)item);
            }
            Destroy (gameObject);
        }
    }
}
