using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class StructureAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float animationSpeed;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        anim.SetFloat ("PlaySpeed", animationSpeed);
    }

    void Update ()
    {

    }

    public void PlayAnimation ()
    {
        anim.SetTrigger("Trigger");
    }
}
