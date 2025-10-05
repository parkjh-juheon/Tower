using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
    [Header("UI ����")]
    public Transform itemListParent;   // �����ܵ��� ǥ�õ� �θ� ������Ʈ (ItemIconList)
    public GameObject itemIconPrefab;  // ������ ������ (ItemIconSlot)

    public void AddItemToUI(ItemData item)
    {
        if (item == null) return;

        // ������ ���� ����
        GameObject iconObj = Instantiate(itemIconPrefab, itemListParent);

        // ������ ���� �ʱ�ȭ
        var iconSlot = iconObj.GetComponent<ItemIconSlot>();
        if (iconSlot != null)
        {
            iconSlot.Setup(item);  // ���� ǥ�ÿ� ������ ����
        }
    }
}
