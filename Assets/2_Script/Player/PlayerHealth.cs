using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

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

    private PlayerController playerController;

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();

        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        hitEffect ??= GetComponentInChildren<ParticleSystem>();

        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int damage)
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
            StartCoroutine(InvincibleCoroutine());
        }
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        if (playerController != null)
            playerController.canControl = false;

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
        // 죽는 처리
    }
}
