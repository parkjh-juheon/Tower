using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Image healthBarFill;

    [Header("이펙트")]
    public GameObject hitEffect;
    public Transform effectSpawnPoint;

    [Header("카메라 흔들림")]
    public CameraShake cameraShake;

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();

        // 피격 이펙트
        if (hitEffect != null && effectSpawnPoint != null)
        {
            Instantiate(hitEffect, effectSpawnPoint.position, Quaternion.identity);
        }

        // 카메라 흔들림
        if (cameraShake != null)
        {
            cameraShake.Shake();
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
