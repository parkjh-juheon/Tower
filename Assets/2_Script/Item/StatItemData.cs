using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Items/StatItemData")]
public class StatItemData : ScriptableObject
{
    [Header("정보")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("공통 스탯 변화량")]
    public float attackDamageBonus;
    public float attackCooldownBonus;
    public float moveSpeedBonus;
    public float jumpForceBonus;
    public int maxHPBonus;
    public int healAmount;
    public int maxJumpCountBonus;

    [Header("근접 전용")]
    public float meleeAttackRangeBonus;
    public float knockbackBonus;

    [Header("원거리 전용")]
    public float bulletSpeedBonus;
    public float bulletSizeBonus;
    public float bulletLifeTimeBonus;
}
