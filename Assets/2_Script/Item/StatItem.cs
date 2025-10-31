using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatItem : MonoBehaviour
{
    [Header("������ ������")]
    public ItemData data;

    [Header("��ȣ�ۿ� ����")]
    public float interactDistance = 2.5f;

    [Header("���� ����")]
    public GameObject pickupEffectPrefab;
    public AudioClip pickupSound;

    private Transform player;
    private bool isPlayerInRange = false;
    private bool pickedUp = false;

    private void Start()
    {
        // �÷��̾� ���� Ȯ��
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.activeInHierarchy)
            {
                player = p.transform;
                break;
            }
        }

        if (player == null)
            Debug.LogWarning("[StatItem] Player ��ü�� ã�� ���߽��ϴ�.");
    }

    private void Update()
    {
        if (player == null || data == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Animator animator = player.GetComponent<Animator>();

        // ����/��Ż ����
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

        // ȹ�� �Է�
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
            Debug.LogWarning("[StatItem] Player�� �����ϴ�. �������� ������ �� �����ϴ�.");
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
            Debug.LogWarning("[StatItem] PlayerController �Ǵ� stats�� �����ϴ�. ������ ���� ����.");
            return;
        }

        var stats = controller.stats;

        // ---------------------
        //  ������ ����
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
        // ���� ����
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
        //  ��ȭ ���� & UI ��� (�����ϰ�)
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
                    Debug.LogWarning($"[StatItem] ShowStatChange ����: {e.Message}");
                }
            }
        }

        // ���� ����
        TryShowChange("moveSpeed", stats.moveSpeed);
        TryShowChange("jumpForce", stats.jumpForce);
        TryShowChange("maxJumpCount", stats.maxJumpCount);
        TryShowChange("attackDamage", stats.attackDamage);
        TryShowChange("attackCooldown", stats.attackCooldown);

        // Ÿ�Ժ�
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

        // ü�� ����
        if (health != null)
        {
            TryShowChange("maxHP", health.maxHP);
            TryShowChange("currentHP", health.currentHP);
        }

        // ---------------------
        //  ����Ʈ & ���� & UI ����
        // ---------------------
        ItemInventory.Instance?.AddItem(data);

        if (pickupEffectPrefab != null)
        {
            GameObject effect = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

            // ��޿� ���� ���� ����
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

        // �ణ�� ������ �� ���� (�����)
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
