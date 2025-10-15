using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("공격 범위")]
    public float meleeRange = 3f;        // 근접 공격 사거리
    public float rangedRange = 8f;       // 원거리 공격 사거리
    public float dashTriggerDistance = 10f; // 돌진 발동 거리

    [Header("쿨다운")]
    public float meleeCooldown = 2f;     // 근접 공격 간격
    public float rangedCooldown = 4f;    // 원거리 공격 쿨타임
    public float dashCooldown = 6f;      // 돌진 쿨타임

    private float nextRangedTime = 0f;
    private float nextDashTime = 0f;

    [Header("근접 공격")]
    public int attackDamage = 20;
    public float knockbackPower = 8f;
    public Transform hitboxCenter;
    public Vector2 hitboxSize = new Vector2(2f, 1f);
    public LayerMask targetLayer;

    [Header("원거리 공격")]
    public GameObject missilePrefab;
    public Transform firePoint;

    [Header("돌진 공격")]
    public float dashSpeed = 10f;
    public int dashDamage = 25;
    public float dashKnockback = 12f;
    public float dashPrepTime = 1.5f;
    public float dashStunDuration = 1.0f;
    public float stunTime = 0.5f;
    public LayerMask wallLayer;
    public float backOffDistance = 1f;

    [Header("카메라 흔들림")]
    public Cinemachine.CinemachineImpulseSource impulseSource;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform target;
    private BossChase bossChase;

    private bool isAttacking = false;
    private bool isDashing = false;
    private bool isStunned = false;
    private bool hasHitPlayer = false;
    private bool canDash = true;
    private Vector2 dashDir;

    [Header("사운드")]
    public AudioClip meleeSFX;
    public AudioClip rangedSFX;
    public AudioClip dashSFX;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossChase = GetComponent<BossChase>();

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (target == null)
            target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (isStunned || target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        //  Ranged 공격 - 쿨타임 돌았고 중거리면
        if (!isAttacking && Time.time >= nextRangedTime && distance > meleeRange && distance <= rangedRange)
        {
            StartCoroutine(DoRangedAttack());
            nextRangedTime = Time.time + rangedCooldown;
            return;
        }

        //  Dash 공격 - 쿨타임 돌았고 플레이어가 일정 거리 이상
        if (!isAttacking && canDash && Time.time >= nextDashTime && distance >= dashTriggerDistance)
        {
            DashPrep();
            nextDashTime = Time.time + dashCooldown;
            return;
        }

        //  근접 공격 - 가까우면 무조건 공격
        if (!isAttacking && distance <= meleeRange)
        {
            StartCoroutine(DoMeleeAttack());
            return;
        }

        //  기본적으로 계속 추격
        if (!isAttacking && bossChase != null)
            bossChase.StartChase(meleeRange);
    }

    // ======================= 근접 공격 =======================
    IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        bossChase.StopChase();
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(meleeCooldown);
        isAttacking = false;
        bossChase.ResumeChase();
    }

    public void PerformHit() // 애니메이션 이벤트용
    {
        AudioManager.Instance?.PlaySFX(meleeSFX);

        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter.position, hitboxSize, 0f, targetLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(attackDamage, transform.position, knockbackPower);
            }
        }
    }


    // ======================= 원거리 공격 =======================
    IEnumerator DoRangedAttack()
    {
        isAttacking = true;
        bossChase.StopChase();
        anim.SetTrigger("RangedAttack");

        AudioManager.Instance?.PlaySFX(rangedSFX);

        yield return new WaitForSeconds(rangedCooldown * 0.5f);

        isAttacking = false;
        bossChase.ResumeChase();
    }

    public void ShootMissile() // 애니메이션 이벤트용
    {
        if (missilePrefab != null && firePoint != null && target != null)
        {
            GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
            Missile m = missile.GetComponent<Missile>();
            if (m != null)
                m.SetTarget(target);
        }
    }

    // ======================= 돌진 공격 =======================
    public void DashPrep()
    {
        if (!canDash || isAttacking) return;
        StartCoroutine(DashPrepCoroutine());
    }

    IEnumerator DashPrepCoroutine()
    {
        isAttacking = true;
        bossChase.StopChase();
        anim.SetTrigger("DashPrep");

        dashDir = new Vector2(Mathf.Sign(transform.localScale.x), 0f);

        yield return new WaitForSeconds(dashPrepTime);
        StartDash();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.MovePosition(rb.position + dashDir * dashSpeed * Time.fixedDeltaTime);
            PerformDashHit();
        }
    }

    public void StartDash()
    {
        isDashing = true;
        hasHitPlayer = false;
        anim.SetTrigger("Dash");
    }

    public void PerformDashHit()
    {
        if (!isDashing || hasHitPlayer) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, 3f, targetLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(dashDamage, transform.position, dashKnockback, stunTime);
                hasHitPlayer = true;
            }
        }
    }

    private void StopDash()
    {
        if (!isDashing) return;

        isDashing = false;
        isAttacking = false;

        Debug.Log("Dash Ended");

        anim.ResetTrigger("Dash");
        anim.ResetTrigger("DashPrep");
        anim.SetTrigger("EndDash");

        if (impulseSource != null)
            impulseSource.GenerateImpulse();

        StartCoroutine(DashStunCoroutine());
    }

    private IEnumerator DashStunCoroutine()
    {
        isStunned = true;
        if (bossChase != null) bossChase.StopChase();

        yield return new WaitForSeconds(dashStunDuration);

        isStunned = false;
        isAttacking = false;
        if (bossChase != null) bossChase.ResumeChase();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;

        Debug.Log($"Dash Collision Detected with {collision.gameObject.name}, Layer: {collision.gameObject.layer}");

        if (((1 << collision.collider.gameObject.layer) & wallLayer) != 0)
        {

            AudioManager.Instance?.PlaySFX(dashSFX);

            Debug.Log("Dash hit wall! Stopping dash.");
            rb.MovePosition(rb.position - dashDir * backOffDistance);
            StopDash();
        }
    }


    void OnDrawGizmosSelected()
    {
        if (hitboxCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(hitboxCenter.position, hitboxSize);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
    }
}
