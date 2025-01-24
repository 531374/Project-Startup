using UnityEngine;

public class InventoryEffect : MonoBehaviour
{
    private Animator animator;

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
            }
            if (Input.GetKeyDown (KeyCode.E))
            {
                animator.SetTrigger ("Swap Left");
            }
        }
    }
}