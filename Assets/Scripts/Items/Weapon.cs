using System.Buffers.Text;
using UnityEngine;

[CreateAssetMenu (fileName = "New Weapon", menuName = "Scriptables/Weapon")]
public class Weapon : Item
{
    new public string name = "Weapon";
    public int attack = 2;
    public int durability = 1;
}
