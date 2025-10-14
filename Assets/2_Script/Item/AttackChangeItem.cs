using UnityEngine;
using System.Collections;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    [Header("������ ������")]
    public ItemData data;

    [Header("���� Ÿ�� ���� ����")]
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6; // Ranged ��ȯ �� ������ ź�� ��

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

        float distance = Vector2.Distance(transform.position, player.position);
        Animator animator = player.GetComponent<Animator>();

        // ���� �� ���� ǥ��
        if (distance <= interactDistance && !isPlayerInRange)
        {
            isPlayerInRange = true;
            ItemUIManager.Instance?.ShowItemInfo(data);

            if (animator != null)
                animator.SetBool("NearItem", true);
        }
        // �־����� ���� �����
        else if (distance > interactDistance && isPlayerInRange)
        {
            isPlayerInRange = false;
            ItemUIManager.Instance?.HideItemInfo();

            if (animator != null)
                animator.SetBool("NearItem", false);
        }

        // CŰ�� ������ ȹ��
        if (isPlayerInRange && !pickedUp && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("[AttackChangeItem] Pickup ȣ���");
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

            Debug.Log($"[AttackChangeItem] ���� ��� ���� �� {newAttackType}");
        }

        // �κ��丮�� �߰�
        if (ItemInventory.Instance != null && data != null)
            ItemInventory.Instance.AddItem(data);

        // ����Ʈ, ���� ���
        if (pickupEffectPrefab != null)
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioManager.Instance?.PlaySFX(pickupSound);

        // UI ����
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
