using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHP = 200;
    public int currentHP;

    [Header("UI")]
    public Slider bossHealthSlider;  
    public GameObject bossUIRoot;    

    [Header("Hit Effect")]
    public SpriteRenderer spriteRenderer;  
    public Color hitColor = Color.red;     
    public float flashDuration = 0.1f;    

    [Header("Next Door")]
    public GameObject nextDoor;

    private Color originalColor;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;
    private BossChase bossChase;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        bossChase = GetComponent<BossChase>();
    }

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
        if (isDead) return;
        isDead = true;

        Debug.Log("Boss Dead!");

        if (bossUIRoot != null)
            bossUIRoot.SetActive(false);

        if (anim != null)
            anim.SetTrigger("Die");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (bossChase != null)
            bossChase.enabled = false; // BossChase 컴포넌트 비활성화

        if (col != null)
            col.enabled = false;

        if (nextDoor != null)
            nextDoor.SetActive(true);
    }
}
