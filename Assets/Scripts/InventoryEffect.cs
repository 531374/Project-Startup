using UnityEngine;

public class InventoryEffect : MonoBehaviour
{
    private Animator animator;
    [HideInInspector] public bool inventorySwaped;


    private void Start ()
    {
        animator = GetComponent<Animator>();
    }

    void Update ()
    {
        if (Input.GetKey (KeyCode.Tab))
        {
             if (Input.GetKeyDown (KeyCode.Q))
            {
                animator.SetTrigger ("Swap Right");
                inventorySwaped = true;
            }
            if (Input.GetKeyDown (KeyCode.E))
            {
                animator.SetTrigger ("Swap Left");
                inventorySwaped = true;
            }
        }
    }
}