using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Image healthBarFill;
    public CinemachineImpulseSource impulseSource;
    private Animator animator;

    [Header("����Ʈ")]
    public ParticleSystem hitEffect;

    [Header("�ǰ� �� ���� �ð�")]
    public float invincibleDuration = 0.5f;
    public bool isInvincible = false;

    [Header("�˹� ����")]
    public float knockbackDuration = 0.2f;  // �˹� �ð�
    private Rigidbody2D rb;
    private bool isKnockback = false;

    [Header("��� ó��")]
    [SerializeField] private float deathDeactivateDelay = 1.5f; // ��� �� ��Ȱ��ȭ���� ��� �ð�(��, �ν����Ϳ��� ����)

    private PlayerController playerController;

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();

        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        hitEffect ??= GetComponentInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody2D>();

        playerController = GetComponent<PlayerController>();
    }

    public void UpdateMaxHP(int newMaxHP)
    {
        maxHP = newMaxHP;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();
    }

    /// <summary>
    /// ������ ���ݹ��� �� ȣ��
    /// </summary>
    public void TakeDamage(int damage, Vector2 attackerPosition, float attackerKnockbackPower)
    {
        if (isInvincible) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();

        if (impulseSource != null)
            impulseSource.GenerateImpulse();

        if (animator != null)
            animator.SetTrigger("Hit");

        if (hitEffect != null)
            hitEffect.Play();

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(attackerPosition, attackerKnockbackPower);
            StartCoroutine(InvincibleCoroutine());
        }
    }

    void ApplyKnockback(Vector2 attackerPosition, float knockbackPower)
    {
        if (isKnockback) return;
        isKnockback = true;

        // ���� ��� (�� �� �÷��̾� �ݴ� ����)
        Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;

        // �� ����
        rb.AddForce(direction * knockbackPower, ForceMode2D.Impulse);

        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    void EndKnockback()
    {
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        if (playerController != null)

        yield return new WaitForSeconds(invincibleDuration);

        if (playerController != null)
            playerController.canControl = true;
        isInvincible = false;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHP / maxHP;
        }
    }

    void Die()
    {
        Debug.Log("�÷��̾� ���");
        if (animator != null)
            animator.SetTrigger("Die"); // "Die" Ʈ���� �ִϸ��̼� ����

        if (playerController != null)
            playerController.canControl = false; // ���� �Ұ�

        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(deathDeactivateDelay);
        gameObject.SetActive(false);
    }
}