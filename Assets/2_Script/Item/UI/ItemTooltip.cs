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

        // ���콺 ����ٴϰ� (���� ������ �Ʒ�)
        Vector2 pos = Input.mousePosition + new Vector3(-360f, -360f);
        rect.position = pos;

        // ȭ�� ������ ������ �ʰ� Clamp
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
        if (isTooltipVisible) return; // �̹� ���� ������ ����

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
