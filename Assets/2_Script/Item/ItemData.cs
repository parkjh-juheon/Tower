using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public GameObject prefab; // 실제 씬에 등장할 아이템 프리팹
    public float attackDamageBonus;
    public float attackCooldownBonus;
    public float attackRangeBonus;
    public float moveSpeedBonus;
    public float jumpForceBonus;
    public float jumpcountBonus;
    public int maxHPBonus;
}
