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
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.activeInHierarchy)
            {
                player = p.transform;
                break;
            }
        }
    }

    private void Update()
    {
        if (player == null || data == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Animator animator = player.GetComponent<Animator>();

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

        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (pickedUp) return;
        pickedUp = true;

        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Pickup");
            animator.SetBool("NearItem", false);
            StartCoroutine(ReturnToIdle(animator, 0.8f));
        }

        PlayerController controller = player.GetComponent<PlayerController>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (controller == null) controller = player.GetComponentInChildren<PlayerController>();
        if (health == null) health = player.GetComponentInChildren<PlayerHealth>();

        if (controller == null || controller.stats == null) return;
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
        stats.maxJumpCount += data.maxJumpCountBonus;
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
        //  변화 감지 & UI 출력
        // ---------------------
        void TryShowChange(string name, float newValue)
        {
            if (!before.ContainsKey(name)) return;
            float oldValue = before[name];
            if (!Mathf.Approximately(oldValue, newValue))
                PlayerStatsUI.Instance?.ShowStatChange(name, oldValue, newValue);
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
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        if (pickupSound != null)
            AudioManager.Instance?.PlaySFX(pickupSound);

        ItemUIManager.Instance?.HideItemInfo();
        ItemTooltip.Instance?.HideTooltip();

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
