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

    [Header("����Ʈ")]
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

        // ī�޶� ��鸲 ȿ��
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // �ǰ� ��ƼŬ ���
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
        Debug.Log("�÷��̾� ���");
        // �״� ó��
    }
}
