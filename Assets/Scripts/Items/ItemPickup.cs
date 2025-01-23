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
        bool wasItemPickedUp = Inventory.instance.Add(item);

        if (wasItemPickedUp) Destroy (gameObject);
    }
}
