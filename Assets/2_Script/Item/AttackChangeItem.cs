using UnityEngine;
using System.Collections;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    [Header("아이템 데이터")]
    public ItemData data;

    [Header("공격 타입 변경 설정")]
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6; // Ranged 전환 시 지급할 탄약 수

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

        float distance = Vector2.Distance(transform.position, player.position);
        Animator animator = player.GetComponent<Animator>();

        // 접근 시 설명 표시
        if (distance <= interactDistance && !isPlayerInRange)
        {
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);

            if (animator != null)
                animator.SetBool("NearItem", true);
        }
        // 멀어지면 설명 숨기기
        else if (distance > interactDistance && isPlayerInRange)
        {
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();

            if (animator != null)
                animator.SetBool("NearItem", false);
        }

        // C키로 아이템 획득
        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("[AttackChangeItem] Pickup 호출됨");
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
        if (controller != null)
        {
            controller.SetAttackType(newAttackType);

            if (newAttackType == AttackType.Ranged)
            {
                controller.maxAmmo = giveAmmo;
                controller.SendMessage("Reload");
            }

            Debug.Log($"[AttackChangeItem] 공격 방식 변경 → {newAttackType}");
        }

        // 인벤토리에 추가
        if (ItemInventory.Instance != null && data != null)
            ItemInventory.Instance.AddItem(data);

        // 이펙트, 사운드 재생
        if (pickupEffectPrefab != null)
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioManager.Instance?.PlaySFX(pickupSound);

        // UI 정리
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
