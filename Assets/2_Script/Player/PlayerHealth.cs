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
    private Rigidbody2D rb;
    private bool isKnockback = false;

    [Header("사망 처리")]
    [SerializeField] private float deathDeactivateDelay = 1.5f; // 사망 후 비활성화까지 대기 시간(초, 인스펙터에서 조절)

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
    /// 적에게 공격받을 때 호출
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

        // 방향 계산 (적 → 플레이어 반대 방향)
        Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;

        // 힘 적용
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
        Debug.Log("플레이어 사망");
        if (animator != null)
            animator.SetTrigger("Die"); // "Die" 트리거 애니메이션 실행

        if (playerController != null)
            playerController.canControl = false; // 조작 불가

        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(deathDeactivateDelay);
        gameObject.SetActive(false);
    }
}