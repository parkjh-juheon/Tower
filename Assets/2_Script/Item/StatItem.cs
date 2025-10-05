using UnityEngine;

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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null || data == null) return;

        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        // 근처에 들어오면 설명창 표시
        if (distance <= interactDistance && !isPlayerInRange)
        {
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);
        }
        // 범위 밖으로 나가면 설명창 닫기
        else if (distance > interactDistance && isPlayerInRange)
        {
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();
        }

        // C키 입력 시 아이템 획득
        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (pickedUp) return;
        pickedUp = true;

        Debug.Log($"[StatItem] PickupItem 실행됨: {data.itemName}");

        PlayerController controller = player.GetComponent<PlayerController>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        // 스탯 반영
        if (controller != null && controller.stats != null)
        {
            var stats = controller.stats;
            stats.moveSpeed += data.moveSpeedBonus;
            stats.jumpForce += data.jumpForceBonus;
            stats.maxJumpCount += data.maxJumpCountBonus;
            stats.attackDamage += data.attackDamageBonus;
            stats.attackCooldown = Mathf.Max(0.05f, stats.attackCooldown + data.attackCooldownBonus);

            // HP 변화
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

        //  인벤토리 반영
        if (ItemInventory.Instance != null)
        {
            ItemInventory.Instance.AddItem(data);
        }
        else
        {
            Debug.LogWarning(" ItemInventory.Instance가 null입니다!");
        }

        //  이펙트 출력
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

        //  사운드 재생
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        //  설명창 닫기
        ItemUIManager.Instance?.HideItemInfo();
        ItemTooltip.Instance?.HideTooltip();

        //  아이템 제거
        Destroy(gameObject, 0.05f);
    }

    // 희귀도에 따른 색상
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
