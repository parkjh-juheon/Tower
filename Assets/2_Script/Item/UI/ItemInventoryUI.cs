using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Transform itemListParent;   // 아이콘들이 표시될 부모 오브젝트 (ItemIconList)
    public GameObject itemIconPrefab;  // 아이콘 프리팹 (ItemIconSlot)

    public void AddItemToUI(ItemData item)
    {
        if (item == null) return;

        // 아이콘 슬롯 생성
        GameObject iconObj = Instantiate(itemIconPrefab, itemListParent);

        // 아이콘 슬롯 초기화
        var iconSlot = iconObj.GetComponent<ItemIconSlot>();
        if (iconSlot != null)
        {
            iconSlot.Setup(item);  // 툴팁 표시용 데이터 세팅
        }
    }
}
