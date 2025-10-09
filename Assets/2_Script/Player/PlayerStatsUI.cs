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

    private void Awake() => Instance = this;

    void Start()
    {
        if (player == null || player.stats == null) return;
        CreateStatRows();
        UpdateStatsDisplay();
    }

    void Update()
    {
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
            GameObject row = Instantiate(statRowPrefab, statsPanel);
            row.name = $"StatRow_{name}";

            TextMeshProUGUI text = row.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                statTexts[name] = text;
        }
    }

    // ���� �� ������Ʈ
    void UpdateStatsDisplay()
    {
        var s = player.stats;

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
            text.transform.parent.gameObject.SetActive(active);
    }


    void SetStatText(string statName, float value)
    {
        if (statTexts.TryGetValue(statName, out var text))
        {
            text.text = $"{statName}: {value:F2}";
        }
    }

    //  ��ȭ�� �ؽ�Ʈ ǥ��
    public void ShowStatChange(string statName, float oldValue, float newValue)
    {
        if (!statTexts.ContainsKey(statName)) return;
        float change = newValue - oldValue;
        if (Mathf.Approximately(change, 0f)) return;

        string color = change > 0 ? "#00FF00" : "#FF0000";
        string text = $"{(change > 0 ? "+" : "")}{change:F1}";

        // �ش� ������ Row�� ���̱�
        Transform statRow = statTexts[statName].transform.parent;
        TextMeshProUGUI tmp = Instantiate(changeTextPrefab, statRow);
        tmp.text = $"<color={color}>{text}</color>";

        StartCoroutine(HideChangeText(tmp, 1.5f));
    }

    private IEnumerator HideChangeText(TextMeshProUGUI tmp, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(tmp.gameObject);
    }
}
