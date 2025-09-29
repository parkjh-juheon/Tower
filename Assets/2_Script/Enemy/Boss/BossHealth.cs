using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHP = 200;
    public int currentHP;

    [Header("UI")]
    public Slider bossHealthSlider;  // 화면 상단 고정 HP 바
    public GameObject bossUIRoot;    // 체력 다 닳으면 숨김 처리용

    [Header("Hit Effect")]
    public SpriteRenderer spriteRenderer;  // 보스 스프라이트
    public Color hitColor = Color.red;     // 피격 시 색상
    public float flashDuration = 0.1f;     // 색상 유지 시간

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
            bossUIRoot.SetActive(false); // 체력 UI 숨김

        // 사망 연출
        Destroy(gameObject, 2f);
    }
}
