using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public static ItemInventory Instance;  // �̱���

    public List<ItemData> acquiredItems = new List<ItemData>();  // ȹ���� ������ ����Ʈ
    public ItemInventoryUI inventoryUI;    // UI ����

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        Debug.Log($"[ItemInventory] AddItem ȣ���: {item?.itemName}");
        acquiredItems.Add(item);
        if (inventoryUI == null)
            Debug.LogWarning("inventoryUI�� ������� �ʾҽ��ϴ�!");
        else
            inventoryUI.AddItemToUI(item);
    }

}
