using UnityEngine;

[CreateAssetMenu (fileName = "New Itme", menuName = "Scriptables/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
}
