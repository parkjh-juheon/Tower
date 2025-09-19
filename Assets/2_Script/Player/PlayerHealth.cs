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

    [Header("이펙트")]
    public ParticleSystem hitEffect;

    [Header("피격 후 무적 시간")]
    public float invincibleDuration = 0.5f;
    public bool isInvincible = false;

    [Header("넉백 설정")]
    public float knockbackDuration = 0.2f;  // 넉백 시간
    public float knockbackPower = 10f;      // 기본 넉백 세기
    private Rigidbody2D rb;
    private bool isKnockback = false;

    [Header("사망 처리")]
    [SerializeField] private float deathDeactivateDelay = 1.5f;

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

    void ApplyKnockback(Vector2 attackerPosition, float power)
    {
        if (isKnockback) return;

        // 방향 계산
        Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;

        // 너무 가까워서 (0,0) 되는 것 방지
        if (direction == Vector2.zero)
            direction = Vector2.left;

        direction.y = 0.3f;        // 살짝 위로 튕기게
        direction.Normalize();

        isKnockback = true;
        if (playerController != null)
            playerController.canControl = false;

        rb.linearVelocity = Vector2.zero; // 기존 속도 초기화
        rb.AddForce(direction * power, ForceMode2D.Impulse);

        StartCoroutine(EndKnockback());
    }

    IEnumerator EndKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
        if (playerController != null)
            playerController.canControl = true;
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    public void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHP / maxHP;
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
        if (animator != null)
            animator.SetTrigger("Die");

        if (playerController != null)
            playerController.canControl = false;

        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(deathDeactivateDelay);
        gameObject.SetActive(false);
    }
}
