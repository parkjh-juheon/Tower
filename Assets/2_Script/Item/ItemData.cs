using UnityEngine;

public enum ItemRarity { Common, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    [TextArea] public string description;
    public ItemRarity rarity;    // 희귀도
    public GameObject prefab;

    // --- 스탯 보너스 ---
    public float moveSpeedBonus;
    //public float dashDistanceBonus;
    public float jumpForceBonus;
    public int maxJumpCountBonus;
    public float attackDamageBonus;
    public float attackCooldownBonus;
    public int maxHPBonus;
    public int healAmount;

    // 근접 전용
    public float knockbackPowerBonus;
    public float meleeRangeBonus;

    // 원거리 전용
    public float bulletSizeBonus;
    public float bulletLifeTimeBonus;
    public float bulletSpeedBonus;
}
