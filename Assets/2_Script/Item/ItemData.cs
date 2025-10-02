using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public GameObject prefab; // ���� ���� ������ ������ ������
    public float attackDamageBonus;
    public float attackCooldownBonus;
    public float attackRangeBonus;
    public float moveSpeedBonus;
    public float jumpForceBonus;
    public float jumpcountBonus;
    public int maxHPBonus;
}
