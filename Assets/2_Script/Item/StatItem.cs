using UnityEngine;
using System.Collections;

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
        {
            player = playerObj.transform;
            Debug.Log($"[StatItem] Player 오브젝트 탐색 성공: {player.name}");
        }
        else
        {
            Debug.LogWarning("[StatItem] Player 오브젝트를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (player == null || data == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Animator animator = player.GetComponent<Animator>();

        // 거리 로그
        Debug.Log($"[StatItem] 거리: {distance:F2} / 상호작용 거리: {interactDistance} / isPlayerInRange: {isPlayerInRange}");

        // 근처 접근 시
        if (distance <= interactDistance && !isPlayerInRange)
        {
            Debug.Log($"[StatItem] >>> 근처 진입 감지 ({data.itemName})");
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);

            if (animator != null)
            {
                animator.SetBool("NearItem", true);
                Debug.Log($"[StatItem] NearItem → TRUE (거리: {distance:F2})");
            }
            else
            {
                Debug.LogWarning("[StatItem] Player에 Animator가 없습니다!");
            }
        }
        // 범위 벗어남
        else if (distance > interactDistance && isPlayerInRange)
        {
            Debug.Log($"[StatItem] <<< 범위 벗어남 감지 ({data.itemName})");
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();

            if (animator != null)
            {
                animator.SetBool("NearItem", false);
                Debug.Log($"[StatItem] NearItem → FALSE (거리: {distance:F2})");
            }
        }

        // 아이템 줍기
        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log($"[StatItem] [C] 키 입력 감지 → PickupItem() 호출");
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
            Debug.Log("[StatItem] 아이템 줍기 애니메이션 실행");
        }

        Debug.Log($"[StatItem] '{data.itemName}' 획득 처리 시작");

        PlayerController controller = player.GetComponent<PlayerController>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (controller != null && controller.stats != null)
        {
            var stats = controller.stats;
            stats.moveSpeed += data.moveSpeedBonus;
            stats.jumpForce += data.jumpForceBonus;
            stats.maxJumpCount += data.maxJumpCountBonus;
            stats.attackDamage += data.attackDamageBonus;
            stats.attackCooldown = Mathf.Max(0.05f, stats.attackCooldown + data.attackCooldownBonus);
            Debug.Log($"[StatItem] 스탯 적용 완료: {data.itemName}");
        }
        else
        {
            Debug.LogWarning("[StatItem] PlayerController 또는 stats가 없습니다!");
        }

        if (ItemInventory.Instance != null)
            ItemInventory.Instance.AddItem(data);
        else
            Debug.LogWarning("[StatItem] ItemInventory.Instance가 null입니다!");

        if (pickupEffectPrefab != null)
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

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
            Debug.Log("[StatItem] 플레이어 애니메이션 Idle로 복귀");
        }
    }
}
