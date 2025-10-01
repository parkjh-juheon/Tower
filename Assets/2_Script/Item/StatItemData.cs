using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Items/StatItemData")]
public class StatItemData : ScriptableObject
{
    [Header("����")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("���� ���� ��ȭ��")]
    public float attackDamageBonus;
    public float attackCooldownBonus;
    public float moveSpeedBonus;
    public float jumpForceBonus;
    public int maxHPBonus;
    public int healAmount;
    public int maxJumpCountBonus;

    [Header("���� ����")]
    public float meleeAttackRangeBonus;
    public float knockbackBonus;

    [Header("���Ÿ� ����")]
    public float bulletSpeedBonus;
    public float bulletSizeBonus;
    public float bulletLifeTimeBonus;
}
