using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 10;
    public int currentHP;

    [Header("UI")]
    public GameObject healthBarUI; // 체력바 UI를 담은 GameObject (Canvas)
    public Image healthFillImage;  // 체력 채우는 이미지

    private void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        UpdateHealthBar();

        if (currentHP <= 0)
        {
            Die();
        }
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
        // 죽음 처리
        Destroy(gameObject);
    }
}
