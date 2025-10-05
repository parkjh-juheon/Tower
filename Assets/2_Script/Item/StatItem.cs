using UnityEngine;

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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null || data == null) return;

        // �÷��̾���� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, player.position);

        // ��ó�� ������ ����â ǥ��
        if (distance <= interactDistance && !isPlayerInRange)
        {
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);
        }
        // ���� ������ ������ ����â �ݱ�
        else if (distance > interactDistance && isPlayerInRange)
        {
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();
        }

        // CŰ �Է� �� ������ ȹ��
        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (pickedUp) return;
        pickedUp = true;

        Debug.Log($"[StatItem] PickupItem �����: {data.itemName}");

        PlayerController controller = player.GetComponent<PlayerController>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        // ���� �ݿ�
        if (controller != null && controller.stats != null)
        {
            var stats = controller.stats;
            stats.moveSpeed += data.moveSpeedBonus;
            stats.jumpForce += data.jumpForceBonus;
            stats.maxJumpCount += data.maxJumpCountBonus;
            stats.attackDamage += data.attackDamageBonus;
            stats.attackCooldown = Mathf.Max(0.05f, stats.attackCooldown + data.attackCooldownBonus);

            // HP ��ȭ
            if (health != null)
            {
                if (data.maxHPBonus > 0)
                    health.UpdateMaxHP(health.maxHP + data.maxHPBonus);

                if (data.healAmount > 0)
                {
                    health.currentHP = Mathf.Min(health.currentHP + data.healAmount, health.maxHP);
                    health.UpdateHealthBar();
                }
            }
        }

        //  �κ��丮 �ݿ�
        if (ItemInventory.Instance != null)
        {
            ItemInventory.Instance.AddItem(data);
        }
        else
        {
            Debug.LogWarning(" ItemInventory.Instance�� null�Դϴ�!");
        }

        //  ����Ʈ ���
        if (pickupEffectPrefab != null)
        {
            GameObject effect = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            foreach (var ps in effect.GetComponentsInChildren<ParticleSystem>())
            {
                var main = ps.main;
                main.startColor = GetColorByRarity(data.rarity);
            }
            Destroy(effect, 2f);
        }

        //  ���� ���
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        //  ����â �ݱ�
        ItemUIManager.Instance?.HideItemInfo();
        ItemTooltip.Instance?.HideTooltip();

        //  ������ ����
        Destroy(gameObject, 0.05f);
    }

    // ��͵��� ���� ����
    private Color GetColorByRarity(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return Color.white;
            case ItemRarity.Rare: return new Color(0.6f, 2.5f, 1f);
            case ItemRarity.Epic: return new Color(0.85f, 0.55f, 1f);
            case ItemRarity.Legendary: return new Color(1f, 0.8f, 0.2f);
            default: return Color.white;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
