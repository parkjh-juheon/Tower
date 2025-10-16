using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 10;
    public int currentHP;

    [Header("UI")]
    public GameObject healthBarUI;
    public Image healthFillImage;

    [Header("넉백 설정")]
    public float knockbackForce = 5f;           // 넉백 기본 세기
    public float knockbackDuration = 0.2f;      // 넉백 시간
    public float sizeInfluence = 1f;            // 사이즈 영향 계수 (인스펙터에서 조절 가능

    [Header("사운드 설정")]
    public AudioClip hitSound;

    private Rigidbody2D rb;
    private bool isKnockback = false;

    public event Action onDie;

    private void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition, float attackerKnockbackPower)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        UpdateHealthBar();

        if (AudioManager.Instance != null && hitSound != null)
            AudioManager.Instance.PlaySFX(hitSound);

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(attackerPosition, attackerKnockbackPower);
        }
    }

    void ApplyKnockback(Vector2 attackerPosition, float knockbackPower)
    {
        if (isKnockback) return;
        isKnockback = true;

        Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;

        direction.y = Mathf.Clamp(direction.y, -0.01f, 0.01f);
        direction = direction.normalized;

        Debug.Log($"[Knockback] Enemy '{name}' ← 방향({direction.x:F2}, {direction.y:F2}) 힘({knockbackPower:F2})");

        rb.AddForce(direction * knockbackPower, ForceMode2D.Impulse);

        Invoke(nameof(EndKnockback), knockbackDuration);
    }


    void EndKnockback()
    {
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }

    void UpdateHealthBar()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)currentHP / maxHP;
        }
    }

    public void Die()
    {
        onDie?.Invoke();
        Missile missile = GetComponent<Missile>();
        if (missile != null)
        {
            missile.Explode(); // 폭발 연출 포함 파괴
        }
        else
        {
            Destroy(gameObject);
        }

        BattleZoneTrigger zone = FindAnyObjectByType<BattleZoneTrigger>();
        if (zone != null)
        {
            zone.ReportEnemyDeath(transform.position);
        }

        Destroy(gameObject);
    }

}
