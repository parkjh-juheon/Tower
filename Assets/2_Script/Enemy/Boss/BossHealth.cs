using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHP = 200;
    public int currentHP;

    [Header("UI")]
    public Slider bossHealthSlider;  // ȭ�� ��� ���� HP ��
    public GameObject bossUIRoot;    // ü�� �� ������ ���� ó����

    [Header("Hit Effect")]
    public SpriteRenderer spriteRenderer;  // ���� ��������Ʈ
    public Color hitColor = Color.red;     // �ǰ� �� ����
    public float flashDuration = 0.1f;     // ���� ���� �ð�

    private Color originalColor;

    private void Start()
    {
        currentHP = maxHP;

        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = maxHP;
            bossHealthSlider.value = currentHP;
        }

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        UpdateHealthBar();
        StartCoroutine(HitFlash());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHP;
        }
    }

    IEnumerator HitFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        Debug.Log("Boss Dead!");

        if (bossUIRoot != null)
            bossUIRoot.SetActive(false); // ü�� UI ����

        // ��� ����
        Destroy(gameObject, 2f);
    }
}
