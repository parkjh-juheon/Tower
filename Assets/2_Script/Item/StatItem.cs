using UnityEngine;
using System.Collections;

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
        {
            player = playerObj.transform;
            Debug.Log($"[StatItem] Player ������Ʈ Ž�� ����: {player.name}");
        }
        else
        {
            Debug.LogWarning("[StatItem] Player ������Ʈ�� ã�� �� �����ϴ�!");
        }
    }

    private void Update()
    {
        if (player == null || data == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Animator animator = player.GetComponent<Animator>();

        // �Ÿ� �α�
        Debug.Log($"[StatItem] �Ÿ�: {distance:F2} / ��ȣ�ۿ� �Ÿ�: {interactDistance} / isPlayerInRange: {isPlayerInRange}");

        // ��ó ���� ��
        if (distance <= interactDistance && !isPlayerInRange)
        {
            Debug.Log($"[StatItem] >>> ��ó ���� ���� ({data.itemName})");
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);

            if (animator != null)
            {
                animator.SetBool("NearItem", true);
                Debug.Log($"[StatItem] NearItem �� TRUE (�Ÿ�: {distance:F2})");
            }
            else
            {
                Debug.LogWarning("[StatItem] Player�� Animator�� �����ϴ�!");
            }
        }
        // ���� ���
        else if (distance > interactDistance && isPlayerInRange)
        {
            Debug.Log($"[StatItem] <<< ���� ��� ���� ({data.itemName})");
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();

            if (animator != null)
            {
                animator.SetBool("NearItem", false);
                Debug.Log($"[StatItem] NearItem �� FALSE (�Ÿ�: {distance:F2})");
            }
        }

        // ������ �ݱ�
        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log($"[StatItem] [C] Ű �Է� ���� �� PickupItem() ȣ��");
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
            Debug.Log("[StatItem] ������ �ݱ� �ִϸ��̼� ����");
        }

        Debug.Log($"[StatItem] '{data.itemName}' ȹ�� ó�� ����");

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
            Debug.Log($"[StatItem] ���� ���� �Ϸ�: {data.itemName}");
        }
        else
        {
            Debug.LogWarning("[StatItem] PlayerController �Ǵ� stats�� �����ϴ�!");
        }

        if (ItemInventory.Instance != null)
            ItemInventory.Instance.AddItem(data);
        else
            Debug.LogWarning("[StatItem] ItemInventory.Instance�� null�Դϴ�!");

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
            Debug.Log("[StatItem] �÷��̾� �ִϸ��̼� Idle�� ����");
        }
    }
}
