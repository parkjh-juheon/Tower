using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Image healthBarFill;

    [Header("����Ʈ")]
    public GameObject hitEffect;
    public Transform effectSpawnPoint;

    [Header("ī�޶� ��鸲")]
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

        // �ǰ� ����Ʈ
        if (hitEffect != null && effectSpawnPoint != null)
        {
            Instantiate(hitEffect, effectSpawnPoint.position, Quaternion.identity);
        }

        // ī�޶� ��鸲
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
        Debug.Log("�÷��̾� ���");
        // �״� ó��
    }
}
