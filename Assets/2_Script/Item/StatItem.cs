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

            if (animator != null)
            {
                animator.SetBool("NearItem", true);
            }
        }
        // 범위 벗어남
        else if (distance > interactDistance && isPlayerInRange)
        {
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();

            if (animator != null)
            {
                animator.SetBool("NearItem", false);
            }
        }

        // 아이템 줍기
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
        if (controller != null && controller.stats != null)
        {
            var stats = controller.stats;
            stats.moveSpeed += data.moveSpeedBonus;
            stats.jumpForce += data.jumpForceBonus;
            stats.maxJumpCount += data.maxJumpCountBonus;
            stats.attackDamage += data.attackDamageBonus;
            stats.attackCooldown = Mathf.Max(0.05f, stats.attackCooldown + data.attackCooldownBonus);
        }

        if (ItemInventory.Instance != null)
            ItemInventory.Instance.AddItem(data);

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
