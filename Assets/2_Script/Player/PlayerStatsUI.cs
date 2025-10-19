using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance;

    [Header("�÷��̾� ����")]
    public PlayerController player;

    [Header("UI �������")]
    public Transform statsPanel;              // VerticalLayoutGroup�� �޸� �θ�
    public GameObject statRowPrefab;          // �� �� (HorizontalLayoutGroup)
    public TextMeshProUGUI changeTextPrefab;  // + / - ǥ�ÿ� ������

    // �� ���� �̸� �� Text ���� ����
    private Dictionary<string, TextMeshProUGUI> statTexts = new();

    private void Awake()
    {
        // ������ �̱��� ����: �̹� �����ϸ� �ߺ� �ı�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // �ʼ� ������Ұ� �������� �� �̻� �������� ����
        if (player == null || player.stats == null || statsPanel == null || statRowPrefab == null)
        {
            Debug.LogWarning("[PlayerStatsUI] �ʼ� UI �Ǵ� player�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        // �̹� ������ ���� ������ �ǳʶٱ� (�ߺ� ����)
        if (statTexts.Count == 0)
        {
            CreateStatRows();
        }

        UpdateStatsDisplay();
    }

    void Update()
    {
        // player�� ��ȿ���� ������ �������� ����
        if (player == null || player.stats == null) return;
        UpdateStatsDisplay();
    }

    // ���� �� ����
    void CreateStatRows()
    {
        string[] statNames = {
            "moveSpeed", "jumpForce", "attackDamage", "attackCooldown",
            "meleeRange", "knockbackPower",
            "bulletSpeed", "bulletLifeTime", "bulletSize",
            "maxHP", "currentHP"
        };

        foreach (string name in statNames)
        {
            if (statRowPrefab == null) break;

            GameObject row = Instantiate(statRowPrefab, statsPanel);
            row.name = $"StatRow_{name}";

            TextMeshProUGUI text = row.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                statTexts[name] = text;
            else
                Debug.LogWarning($"[PlayerStatsUI] StatRow prefab�� TextMeshProUGUI�� �����ϴ�: {name}");
        }
    }

    // ���� �� ������Ʈ
    void UpdateStatsDisplay()
    {
        var s = player.stats;
        if (s == null) return;

        // ���� ����
        SetStatText("moveSpeed", s.moveSpeed);
        SetStatText("jumpForce", s.jumpForce);
        SetStatText("attackDamage", s.attackDamage);
        SetStatText("attackCooldown", s.attackCooldown);

        // Ÿ�Ժ� ����
        bool isMelee = player.attackType == PlayerController.AttackType.Melee;
        bool isRanged = !isMelee;

        SetRowActive("meleeRange", isMelee);
        SetRowActive("knockbackPower", isMelee);
        SetRowActive("bulletSpeed", isRanged);
        SetRowActive("bulletLifeTime", isRanged);
        SetRowActive("bulletSize", isRanged);

        if (isMelee)
        {
            SetStatText("meleeRange", s.meleeRange);
            SetStatText("knockbackPower", s.knockbackPower);
        }
        else
        {
            SetStatText("bulletSpeed", s.bulletSpeed);
            SetStatText("bulletLifeTime", s.bulletLifeTime);
            SetStatText("bulletSize", s.bulletSize);
        }

        // HP
        var health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            SetStatText("maxHP", health.maxHP);
            SetStatText("currentHP", health.currentHP);
        }
    }

    // �� Ȱ��ȭ/��Ȱ��ȭ ���� �Լ�
    void SetRowActive(string statName, bool active)
    {
        if (statTexts.TryGetValue(statName, out var text))
        {
            if (text != null && text.transform != null && text.transform.parent != null)
                text.transform.parent.gameObject.SetActive(active);
        }
    }

    void SetStatText(string statName, float value)
    {
        if (statTexts.TryGetValue(statName, out var text) && text != null)
        {
            text.text = $"{statName}: {value:F2}";
        }
    }

    // ��ȭ�� �ؽ�Ʈ ǥ��
    public void ShowStatChange(string statName, float oldValue, float newValue)
    {
        if (!statTexts.ContainsKey(statName)) return;
        if (changeTextPrefab == null) return;

        float change = newValue - oldValue;
        if (Mathf.Approximately(change, 0f)) return;

        string color = change > 0 ? "#00FF00" : "#FF0000";
        string text = $"{(change > 0 ? "+" : "")}{change:F1}";

        Transform statRow = statTexts[statName]?.transform?.parent;
        if (statRow == null) return;

        TextMeshProUGUI tmp = Instantiate(changeTextPrefab, statRow);
        if (tmp == null) return;

        tmp.text = $"<color={color}>{text}</color>";

        if (isActiveAndEnabled)
        {
            StartCoroutine(HideChangeTextSafe(tmp, 1.5f));
        }
        else
        {
            // ��Ȱ�� ���̸� Destroy�� �����ϰ� �ڷ�ƾ ����
            Destroy(tmp.gameObject, 1.5f);
        }
    }

    private IEnumerator HideChangeTextSafe(TextMeshProUGUI tmp, float delay)
    {
        yield return new WaitForSeconds(delay);
        // �ڷ�ƾ�� ���ƿ� �� ��ü�� �̹� �ı��Ǿ����� ���� �˻�
        if (tmp != null && tmp.gameObject != null)
        {
            Destroy(tmp.gameObject);
        }
    }
}
