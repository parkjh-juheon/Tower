using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIconSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemData itemData;

    public void Setup(ItemData data)
    {
        itemData = data;
        GetComponent<Image>().sprite = data.prefab.GetComponent<SpriteRenderer>()?.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.ShowTooltip(itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.HideTooltip();
    }
}
