using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUIManager : MonoBehaviour
{
    public static ItemUIManager Instance;

    [Header("UI ±∏º∫ø‰º“")]
    public GameObject itemInfoPanel;
    public TextMeshProUGUI itemNameText;
    public Image rarityImage;         // »Ò±Õµµ æ∆¿Ãƒ‹
    public TextMeshProUGUI descriptionText;

    [Header("»Ò±Õµµ æ∆¿Ãƒ‹")]
    public Sprite commonSprite;
    public Sprite rareSprite;
    public Sprite epicSprite;
    public Sprite legendarySprite;

    private void Awake()
    {
        Instance = this;
        itemInfoPanel.SetActive(false);
    }

    public void ShowItemInfo(ItemData data)
    {
        if (data == null) return;

        itemNameText.text = data.itemName;
        descriptionText.text = data.description;

        // »Ò±Õµµø° µ˚∏• æ∆¿Ãƒ‹ ¡ˆ¡§
        rarityImage.sprite = GetRaritySprite(data.rarity);

        itemInfoPanel.SetActive(true);
    }

    public void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }

    private Sprite GetRaritySprite(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return commonSprite;
            case ItemRarity.Rare: return rareSprite;
            case ItemRarity.Epic: return epicSprite;
            case ItemRarity.Legendary: return legendarySprite;
            default: return null;
        }
    }
}
