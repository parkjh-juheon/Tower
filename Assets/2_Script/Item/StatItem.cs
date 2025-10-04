using UnityEngine;

public class StatItem : MonoBehaviour
{
    public ItemData data;
    public float interactDistance = 2.5f;
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
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactDistance && !isPlayerInRange)
        {
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);
        }
        else if (distance > interactDistance && isPlayerInRange)
        {
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.C))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (pickedUp) return;
        pickedUp = true;

        PlayerController controller = player.GetComponent<PlayerController>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (controller != null && controller.stats != null)
        {
            var stats = controller.stats;
            stats.moveSpeed += data.moveSpeedBonus;
            //stats.dashDistance += data.dashDistanceBonus;
            stats.jumpForce += data.jumpForceBonus;
            stats.maxJumpCount += data.maxJumpCountBonus;
            stats.attackDamage += data.attackDamageBonus;
            stats.attackCooldown = Mathf.Max(0.05f, stats.attackCooldown + data.attackCooldownBonus);

            if (data.maxHPBonus > 0 && health != null)
                health.UpdateMaxHP(health.maxHP + data.maxHPBonus);

            if (data.healAmount > 0 && health != null)
            {
                health.currentHP = Mathf.Min(health.currentHP + data.healAmount, health.maxHP);
                health.UpdateHealthBar();
            }
        }

        // ¿Ã∆Â∆Æ
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

        // ªÁøÓµÂ
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Debug.Log($"»πµÊ«— æ∆¿Ã≈€: {data.itemName} (»Ò±Õµµ: {data.rarity})");

        ItemUIManager.Instance?.HideItemInfo();
        Destroy(gameObject);
    }

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
