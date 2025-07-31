using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Image healthBarFill;
    public CinemachineImpulseSource impulseSource;
    private Animator animator;

    [Header("이펙트")]
    public ParticleSystem hitEffect;


    void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();
        
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        if (hitEffect == null)
            hitEffect = GetComponentInChildren<ParticleSystem>();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();

        // 카메라 흔들림 효과
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // 피격 파티클 출력
        if (hitEffect != null)
        {
            hitEffect.Play();
        }

        if (currentHP <= 0)
        {
            Die();
        }
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
        // 죽는 처리
    }
}
