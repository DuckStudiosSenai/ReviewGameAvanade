using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Cosmetic Item")]
public class ShopCosmeticItem : ScriptableObject
{
    public int itemId;
    public RuntimeAnimatorController animator;
}
