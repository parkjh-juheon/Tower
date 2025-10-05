using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public static ItemInventory Instance;  // 싱글톤

    public List<ItemData> acquiredItems = new List<ItemData>();  // 획득한 아이템 리스트
    public ItemInventoryUI inventoryUI;    // UI 참조

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        Debug.Log($"[ItemInventory] AddItem 호출됨: {item?.itemName}");
        acquiredItems.Add(item);
        if (inventoryUI == null)
            Debug.LogWarning("inventoryUI가 연결되지 않았습니다!");
        else
            inventoryUI.AddItemToUI(item);
    }

}
