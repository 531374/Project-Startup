using UnityEngine;

public class ItemPickup : InteractableItem
{

    protected override void Interact()
    {
        base.Interact();
        PickUp();
    }

    private void PickUp ()
    {
        bool wasItemPickedUp = false;
        if (item is CollectableItem)
        {
            wasItemPickedUp = Inventory.instance.AddCollectable((CollectableItem)item);
        }
        else if (item is Weapon)
        {
            wasItemPickedUp = Inventory.instance.AddWeapon ((Weapon)item);
        }

        if (wasItemPickedUp) Destroy (gameObject);
    }
}
