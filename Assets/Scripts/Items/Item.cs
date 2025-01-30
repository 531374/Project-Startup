using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    new public string name = "New Item";
    public string description = "";
    public Sprite icon = null;
    public Sprite selectedIcon = null;
    public Sprite itemDiscription;

}
