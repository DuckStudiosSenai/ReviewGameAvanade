using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemShop", menuName = "Scriptable Objects/ItemShop")]
public class ItemShop : ScriptableObject
{
    public int itemID;
    public Sprite itemIcon;
    public float itemIconScale = 1f;
    public string itemName;
    public int itemPrice;
    public string itemDescription;
}
