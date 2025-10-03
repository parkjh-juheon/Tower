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
    public float sizeInfluence = 1f;            // 사이즈 영향 계수 (인스펙터에서 조절 가능)

    private Rigidbody2D rb;
    private bool isKnockback = false;

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

    // 플레이어 넉백 파워 기반
    float scaledForce = knockbackPower; // 필요 시 추가 조정 가능

    rb.AddForce(direction * scaledForce, ForceMode2D.Impulse);

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

    void Die()
    {
        Missile missile = GetComponent<Missile>();
        if (missile != null)
        {
            missile.Explode(); // 폭발 연출 포함 파괴
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
