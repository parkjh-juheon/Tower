using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private enum BossPattern { None, Melee, Ranged, Dash }
    private BossPattern nextPattern = BossPattern.None;

    [Header("공격 범위")]
    public float meleeRange = 3f;
    public float rangedRange = 8f;
    public float meleeStopDistance = 3f;
    public float rangedStopDistance = 8f;

    [Header("쿨다운")]
    public float meleeCooldown = 2f;
    public float rangedCooldown = 3f;
    private float nextRangedAvailableTime = 0f;

    [Header("돌진 패턴")]
    public float dashSpeed = 10f;
    public float dashCooldown = 5f;
    public int dashDamage = 25;
    public float dashKnockback = 12f;
    public float stunTime = 0.5f; // 플레이어 스턴 시간
    public float dashStunDuration = 1.0f; // 벽 충돌 후 행동 불가 시간
    
    private bool isStunned = false;
    public float stunDuration = 0.5f;
    public float backOffDistance = 1f;

    [Header("근접 공격")]
    public int attackDamage = 20;
    public float knockbackPower = 8f;
    public Transform hitboxCenter;
    public Vector2 hitboxSize = new Vector2(2f, 1f);
    public LayerMask targetLayer;

    [Header("원거리 공격")]
    public GameObject missilePrefab;
    public Transform firePoint;

    [Header("카메라 흔들림")]
    public Cinemachine.CinemachineImpulseSource impulseSource;

    public BossChase bossChase;
    private Transform target;

    private Animator anim;
    private bool isAttacking = false;
    private bool isDashing = false;
    private bool canDash = true;
    private bool hasHitPlayer = false;
    private Vector2 dashDir;
    private float dashFacingDirection; 
    private BossPattern lastPattern = BossPattern.None;

    private Rigidbody2D rb;

    // 대쉬 관련 코루틴 추적
    private Coroutine dashPrepCoroutine;
    private Coroutine dashCooldownCoroutine;
    public LayerMask wallLayer;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (bossChase == null) bossChase = GetComponent<BossChase>();

        target = PlayerController.TargetPoint;
    }

    void Update()
    {
        if (isStunned)
        {
            if (bossChase != null && bossChase.enabled)
                bossChase.enabled = false;
            return;
        }

        if (target == null || !target.gameObject.activeInHierarchy || isAttacking) return;

        // 다음 공격 패턴 결정
        DecideNextPattern();
        Debug.Log("DecideNextPattern called, nextPattern: " + nextPattern);

        float distance = Vector2.Distance(transform.position, target.position);

        switch (nextPattern)
        {
            case BossPattern.Melee:
                if (distance <= meleeStopDistance)
                {
                    StartCoroutine(DoMeleeAttack());
                    nextPattern = BossPattern.None;
                }
                break;

            case BossPattern.Ranged:
                if (distance <= rangedStopDistance)
                {
                    StartCoroutine(DoRangedAttack());
                    nextPattern = BossPattern.None;
                }
                break;

            case BossPattern.Dash:
                DashPrep();
                nextPattern = BossPattern.None;
                break;
        }
    }
    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.MovePosition(rb.position + dashDir * dashSpeed * Time.fixedDeltaTime);
            PerformDashHit();
        }
    }


    // =================== 애니메이션 이벤트 ===================
    public void StopChase()
    {
        if (bossChase != null)
            bossChase.enabled = false;
    }

    public void ResumeChase()
    {
        if (bossChase != null)
            bossChase.enabled = true;
    }

    // =================== 근접 공격 ===================
    IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(meleeCooldown);
        isAttacking = false;
    }

    public void PerformHit()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter.position, hitboxSize, 0f, targetLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(attackDamage, transform.position, knockbackPower);
                }
            }
        }
    }

    // =================== 원거리 공격 ===================
    IEnumerator DoRangedAttack()
    {
        isAttacking = true;
        anim.SetTrigger("RangedAttack");
        yield return new WaitForSeconds(rangedCooldown);
        isAttacking = false;
    }

    public void ShootMissile()
    {
        if (missilePrefab != null && firePoint != null && target != null)
        {
            GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
            Missile m = missile.GetComponent<Missile>();
            if (m != null)
                m.SetTarget(target); //  targetPoint로 자동 유도
        }
    }


    // =================== 돌진 공격 ===================
    public void DashPrep()
    {
        if (!canDash || isAttacking) return;
        dashPrepCoroutine = StartCoroutine(DashPrepCoroutine());
    }

    IEnumerator DashPrepCoroutine()
    {
        canDash = false;
        isAttacking = true;
        anim.SetTrigger("DashPrep");

        if (bossChase != null) bossChase.enabled = false;

        //  준비 시점에 바라본 방향 기억 (1: 오른쪽, -1: 왼쪽)
        dashFacingDirection = Mathf.Sign(transform.localScale.x);

        //  플레이어가 어느 쪽에 있든 현재 바라본 방향으로만 돌진
        dashDir = new Vector2(dashFacingDirection, 0f);

        yield return new WaitForSeconds(2f); // 준비 애니메이션 길이
        StartDash();
    }
    public void StartDash()
    {
        if (!isAttacking) return;

        isDashing = true;
        hasHitPlayer = false; //  새 돌진 시작 시 초기화
        anim.SetTrigger("Dash");
    }

    public void PerformDashHit()
    {
        if (!isDashing || hasHitPlayer) return; //  이미 맞췄으면 무시

        Collider2D hit = Physics2D.OverlapCircle(transform.position, 3f, targetLayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(dashDamage, transform.position, dashKnockback, stunTime * 2f);
                hasHitPlayer = true;  //  첫 번째 피격 후 중복 방지
               // StopDash();           //  피격 즉시 돌진 종료
            }
        }
    }

    private void StopDash()
    {
        isDashing = false;
        isAttacking = false;

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

        // 보스 이동/추격 중지
        if (bossChase != null)
            bossChase.enabled = false;

        // 스턴 유지 시간
        yield return new WaitForSeconds(dashStunDuration);

        // 스턴 해제
        isStunned = false;

        // 스턴이 끝난 후에만 추격 복구
        if (bossChase != null)
            bossChase.enabled = true;

        // 쿨다운 시작
        dashCooldownCoroutine = StartCoroutine(DashCooldownCoroutine());
    }

    private IEnumerator DashCooldownCoroutine()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        dashCooldownCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;
  
        if (((1 << collision.collider.gameObject.layer) & wallLayer) != 0)
        {
            rb.MovePosition(rb.position - dashDir * backOffDistance);
            StopDash();
        }
    }

    // =================== 패턴 결정 ===================
    void DecideNextPattern()
    {
        List<BossPattern> candidates = new List<BossPattern>();
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= meleeRange)
            candidates.Add(BossPattern.Melee);
        if (distance <= rangedRange && Time.time >= nextRangedAvailableTime)
            candidates.Add(BossPattern.Ranged);
        if (canDash)
            candidates.Add(BossPattern.Dash);

        if (candidates.Count == 0)
        {
            nextPattern = BossPattern.None;
            Debug.Log("BossPattern: None");
            return;
        }

        if (candidates.Count > 1 && candidates.Contains(lastPattern))
            candidates.Remove(lastPattern);

        int index = Random.Range(0, candidates.Count);
        nextPattern = candidates[index];
        lastPattern = nextPattern;

        if (nextPattern == BossPattern.Ranged)
            nextRangedAvailableTime = Time.time + rangedCooldown * 2f;

        Debug.Log("BossPattern: " + nextPattern);  // ← 여기서 패턴 로그 출력
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

        if (isDashing)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f); // OverlapCircle 반경 시각화
        }
    }
}
