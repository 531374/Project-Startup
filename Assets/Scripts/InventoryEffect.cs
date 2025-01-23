using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.RightArrow))
        {
            animator.SetBool ("RightOnce", true);
        }
    }
}
