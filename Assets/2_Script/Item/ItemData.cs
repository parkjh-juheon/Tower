using UnityEngine;

public enum ItemRarity { Common, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string itemName;
    [TextArea] public string description;
    public ItemRarity rarity;    // ��͵�
    public GameObject prefab;

    // --- ���� ���ʽ� ---
    public float moveSpeedBonus;
    //public float dashDistanceBonus;
    public float jumpForceBonus;
    public int maxJumpCountBonus;
    public float attackDamageBonus;
    public float attackCooldownBonus;
    public int maxHPBonus;
    public int healAmount;

    // ���� ����
    public float knockbackPowerBonus;
    public float meleeRangeBonus;

    // ���Ÿ� ����
    public float bulletSizeBonus;
    public float bulletLifeTimeBonus;
    public float bulletSpeedBonus;
}
