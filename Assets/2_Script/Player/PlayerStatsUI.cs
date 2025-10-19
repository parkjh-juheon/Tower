using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance;

    [Header("플레이어 스텟")]
    public PlayerController player;

    [Header("UI 구성요소")]
    public Transform statsPanel;              // VerticalLayoutGroup이 달린 부모
    public GameObject statRowPrefab;          // 한 줄 (HorizontalLayoutGroup)
    public TextMeshProUGUI changeTextPrefab;  // + / - 표시용 프리팹

    // 각 스탯 이름 → Text 참조 저장
    private Dictionary<string, TextMeshProUGUI> statTexts = new();

    private void Awake()
    {
        // 안전한 싱글턴 패턴: 이미 존재하면 중복 파괴
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
        // 필수 구성요소가 빠졌으면 더 이상 진행하지 않음
        if (player == null || player.stats == null || statsPanel == null || statRowPrefab == null)
        {
            Debug.LogWarning("[PlayerStatsUI] 필수 UI 또는 player가 할당되지 않았습니다.");
            return;
        }

        // 이미 생성된 행이 있으면 건너뛰기 (중복 방지)
        if (statTexts.Count == 0)
        {
            CreateStatRows();
        }

        UpdateStatsDisplay();
    }

    void Update()
    {
        // player가 유효하지 않으면 갱신하지 않음
        if (player == null || player.stats == null) return;
        UpdateStatsDisplay();
    }

    // 스탯 행 생성
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
                Debug.LogWarning($"[PlayerStatsUI] StatRow prefab에 TextMeshProUGUI가 없습니다: {name}");
        }
    }

    // 스탯 값 업데이트
    void UpdateStatsDisplay()
    {
        var s = player.stats;
        if (s == null) return;

        // 공통 스탯
        SetStatText("moveSpeed", s.moveSpeed);
        SetStatText("jumpForce", s.jumpForce);
        SetStatText("attackDamage", s.attackDamage);
        SetStatText("attackCooldown", s.attackCooldown);

        // 타입별 스탯
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

    // 줄 활성화/비활성화 제어 함수
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

    // 변화량 텍스트 표시
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
            // 비활성 중이면 Destroy만 실행하고 코루틴 생략
            Destroy(tmp.gameObject, 1.5f);
        }
    }

    private IEnumerator HideChangeTextSafe(TextMeshProUGUI tmp, float delay)
    {
        yield return new WaitForSeconds(delay);
        // 코루틴이 돌아올 때 객체가 이미 파괴되었는지 안전 검사
        if (tmp != null && tmp.gameObject != null)
        {
            Destroy(tmp.gameObject);
        }
    }
}
