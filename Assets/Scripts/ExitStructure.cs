using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitStructure : InteractableObject
{
    protected override void Update()
    {
        base.Update();
        if (interacted)
        {
            interacted = false;

            
        }
    }
}
