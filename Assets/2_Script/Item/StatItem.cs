using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatItem : MonoBehaviour
{
    [Header("아이템 데이터")]
    public ItemData data;

    [Header("상호작용 설정")]
    public float interactDistance = 2.5f;

    [Header("연출 설정")]
    public GameObject pickupEffectPrefab;
    public AudioClip pickupSound;

    private Transform player;
    private bool isPlayerInRange = false;
    private bool pickedUp = false;

    private void Start()
    {
        // 플레이어 참조 확보
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.activeInHierarchy)
            {
                player = p.transform;
                break;
            }
        }

        if (player == null)
            Debug.LogWarning("[StatItem] Player 객체를 찾지 못했습니다.");
    }

    private void Update()
    {
        if (player == null || data == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Animator animator = player.GetComponent<Animator>();

        // 접근/이탈 감지
        if (distance <= interactDistance && !isPlayerInRange)
        {
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);
            if (animator != null) animator.SetBool("NearItem", true);
        }
        else if (distance > interactDistance && isPlayerInRange)
        {
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();
            if (animator != null) animator.SetBool("NearItem", false);
        }

        // 획득 입력
        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (pickedUp) return;
        pickedUp = true;

        if (player == null)
        {
            Debug.LogWarning("[StatItem] Player가 없습니다. 아이템을 적용할 수 없습니다.");
            return;
        }

        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Pickup");
            animator.SetBool("NearItem", false);
            StartCoroutine(ReturnToIdle(animator, 0.8f));
        }

        PlayerController controller = player.GetComponent<PlayerController>() ??
                                      player.GetComponentInChildren<PlayerController>();
        PlayerHealth health = player.GetComponent<PlayerHealth>() ??
                              player.GetComponentInChildren<PlayerHealth>();

        if (controller == null || controller.stats == null)
        {
            Debug.LogWarning("[StatItem] PlayerController 또는 stats가 없습니다. 아이템 적용 실패.");
            return;
        }

        var stats = controller.stats;

        // ---------------------
        //  기존값 저장
        // ---------------------
        Dictionary<string, float> before = new()
        {
            { "moveSpeed", stats.moveSpeed },
            { "jumpForce", stats.jumpForce },
            { "maxJumpCount", stats.maxJumpCount },
            { "attackDamage", stats.attackDamage },
            { "attackCooldown", stats.attackCooldown },
            { "meleeRange", stats.meleeRange },
            { "knockbackPower", stats.knockbackPower },
            { "bulletSpeed", stats.bulletSpeed },
            { "bulletLifeTime", stats.bulletLifeTime },
            { "bulletSize", stats.bulletSize },
            { "maxHP", health != null ? health.maxHP : 0 },
            { "currentHP", health != null ? health.currentHP : 0 }
        };

        // ---------------------
        // 스탯 적용
        // ---------------------
        stats.moveSpeed += data.moveSpeedBonus;
        stats.jumpForce += data.jumpForceBonus;
        controller.baseMaxJumpCount += data.maxJumpCountBonus;
        stats.attackDamage += data.attackDamageBonus;
        stats.attackCooldown = Mathf.Max(0.05f, stats.attackCooldown + data.attackCooldownBonus);

        if (controller.attackType == PlayerController.AttackType.Melee)
        {
            stats.knockbackPower += data.knockbackPowerBonus;
            stats.meleeRange += data.meleeRangeBonus;
        }
        else
        {
            stats.bulletSize += data.bulletSizeBonus;
            stats.bulletLifeTime += data.bulletLifeTimeBonus;
            stats.bulletSpeed += data.bulletSpeedBonus;
        }

        if (health != null)
        {
            if (data.maxHPBonus != 0)
                health.UpdateMaxHP(health.maxHP + data.maxHPBonus);

            if (data.healAmount > 0)
            {
                health.currentHP = Mathf.Min(health.currentHP + data.healAmount, health.maxHP);
                health.UpdateHealthBar();
            }
        }

        // ---------------------
        //  변화 감지 & UI 출력 (안전하게)
        // ---------------------
        void TryShowChange(string name, float newValue)
        {
            if (!before.ContainsKey(name)) return;
            float oldValue = before[name];
            if (Mathf.Approximately(oldValue, newValue)) return;

            if (PlayerStatsUI.Instance != null && PlayerStatsUI.Instance.gameObject != null)
            {
                try
                {
                    PlayerStatsUI.Instance.ShowStatChange(name, oldValue, newValue);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[StatItem] ShowStatChange 오류: {e.Message}");
                }
            }
        }

        // 공통 스탯
        TryShowChange("moveSpeed", stats.moveSpeed);
        TryShowChange("jumpForce", stats.jumpForce);
        TryShowChange("maxJumpCount", stats.maxJumpCount);
        TryShowChange("attackDamage", stats.attackDamage);
        TryShowChange("attackCooldown", stats.attackCooldown);

        // 타입별
        if (controller.attackType == PlayerController.AttackType.Melee)
        {
            TryShowChange("meleeRange", stats.meleeRange);
            TryShowChange("knockbackPower", stats.knockbackPower);
        }
        else
        {
            TryShowChange("bulletSpeed", stats.bulletSpeed);
            TryShowChange("bulletLifeTime", stats.bulletLifeTime);
            TryShowChange("bulletSize", stats.bulletSize);
        }

        // 체력 관련
        if (health != null)
        {
            TryShowChange("maxHP", health.maxHP);
            TryShowChange("currentHP", health.currentHP);
        }

        // ---------------------
        //  이펙트 & 사운드 & UI 정리
        // ---------------------
        ItemInventory.Instance?.AddItem(data);

        if (pickupEffectPrefab != null)
        {
            GameObject effect = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

            // 등급에 따른 색상 지정
            Color effectColor = Color.white;
            switch (data.rarity)
            {
                case ItemRarity.Rare:
                    effectColor = new Color(0.6f, 1.5f, 0.6f);
                    break;
                case ItemRarity.Epic:
                    effectColor = new Color(0.8f, 0.4f, 1.0f);
                    break;
                case ItemRarity.Legendary:
                    effectColor = new Color(1.2f, 1.0f, 0.3f);
                    break;
            }

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = effectColor;
            }
        }

        if (pickupSound != null)
            AudioManager.Instance?.PlaySFX(pickupSound);

        ItemUIManager.Instance?.HideItemInfo();
        ItemTooltip.Instance?.HideTooltip();

        MiniMapController miniMap = FindAnyObjectByType<MiniMapController>();
        if (miniMap != null)
        {
            miniMap.UnregisterItem(transform);
        }

        // 약간의 딜레이 후 제거 (연출용)
        Destroy(gameObject, 0.05f);
    }

    private IEnumerator ReturnToIdle(Animator animator, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
        {
            animator.ResetTrigger("Pickup");
            animator.Play("Idle");
        }
    }
}
