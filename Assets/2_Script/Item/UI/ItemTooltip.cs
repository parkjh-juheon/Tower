using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;
    public TextMeshProUGUI toolText;

    private RectTransform rect;
    private Canvas canvas;
    private bool isTooltipVisible = false;

    private void Awake()
    {
        Instance = this;
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isTooltipVisible) return;

        // 마우스 따라다니게 (조금 오른쪽 아래)
        Vector2 pos = Input.mousePosition + new Vector3(-360f, -360f);
        rect.position = pos;

        // 화면 밖으로 나가지 않게 Clamp
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        Vector2 anchoredPos = rect.anchoredPosition;
        Vector2 size = rect.sizeDelta;

        anchoredPos.x = Mathf.Clamp(anchoredPos.x, -canvasSize.x / 2 + size.x / 2, canvasSize.x / 2 - size.x / 2);
        anchoredPos.y = Mathf.Clamp(anchoredPos.y, -canvasSize.y / 2 + size.y / 2, canvasSize.y / 2 - size.y / 2);
        rect.anchoredPosition = anchoredPos;
    }

    public void ShowTooltip(ItemData item)
    {
        if (item == null) return;
        if (isTooltipVisible) return; // 이미 켜져 있으면 무시

        toolText.text = $"{item.itemName}\n<size=70%>{item.description}</size>";
        gameObject.SetActive(true);
        isTooltipVisible = true;
    }

    public void HideTooltip()
    {
        if (!isTooltipVisible) return;

        gameObject.SetActive(false);
        isTooltipVisible = false;
    }
}
