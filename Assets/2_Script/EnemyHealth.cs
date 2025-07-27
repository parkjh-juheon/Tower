using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 10;
    public int currentHP;

    [Header("UI")]
    public GameObject healthBarUI; // ü�¹� UI�� ���� GameObject (Canvas)
    public Image healthFillImage;  // ü�� ä��� �̹���

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
        // ���� ó��
        Destroy(gameObject);
    }
}
