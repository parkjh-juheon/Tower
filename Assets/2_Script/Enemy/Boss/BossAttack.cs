using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private enum BossPattern { None, Melee, Ranged, Dash }
    private BossPattern nextPattern = BossPattern.None;

    public float meleeStopDistance = 3f;  // 근접 공격 사정거리
    public float rangedStopDistance = 8f; // 원거리 공격 사정거리

    public BossChase bossChase;
    public Transform player;

    public float meleeRange = 3f;      // 근접 공격 범위
    public float rangedRange = 8f;     // 원거리 공격 범위

    public float meleeCooldown = 2f;        // 근거리 공격 쿨다운
    public float rangedCooldown = 3f;       // 원거리 공격 쿨다운
    private float nextRangedAvailableTime = 20f;

    public float dashSpeed = 10f;        // 돌진 속도
    public float dashDuration = 1f;      // 돌진 유지 시간
    public float dashCooldown = 5f;      // 돌진 패턴 쿨다운
    public int dashDamage = 25;          // 돌진 피해
    public float dashKnockback = 12f;    // 돌진 넉백

    private bool canDash = true;
    private bool isDashing = false;

    public GameObject missilePrefab;
    public Transform firePoint;

    public int attackDamage = 20;
    public float knockbackPower = 8f;

    public Transform hitboxCenter;
    public Vector2 hitboxSize = new Vector2(2f, 1f);
    public LayerMask targetLayer;

    private Animator anim;
    private bool isAttacking = false;

    private BossPattern lastPattern = BossPattern.None;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (bossChase == null) bossChase = GetComponent<BossChase>();
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        // 패턴 결정
        DecideNextPattern();

        switch (nextPattern)
        {
            case BossPattern.Melee:
                if (Vector2.Distance(transform.position, player.position) <= meleeStopDistance)
                {
                    StartCoroutine(DoMeleeAttack());
                    nextPattern = BossPattern.None;
                }
                break;

            case BossPattern.Ranged:
                if (Vector2.Distance(transform.position, player.position) <= rangedStopDistance)
                {
                    StartCoroutine(DoRangedAttack());
                    nextPattern = BossPattern.None;
                }
                break;

            case BossPattern.Dash:
                StartCoroutine(DoDashAttack());
                nextPattern = BossPattern.None;
                break;
        }
    }

    IEnumerator DoMeleeAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack"); // 애니메이션 이벤트에서 BossChase 제어
        yield return new WaitForSeconds(meleeCooldown);
        isAttacking = false;
    }

    IEnumerator DoRangedAttack()
    {
        isAttacking = true;
        anim.SetTrigger("RangedAttack");  // 이벤트에서 ShootMissile 호출
        yield return new WaitForSeconds(rangedCooldown);
        isAttacking = false;
    }

    IEnumerator DoDashAttack()
    {
        if (!canDash) yield break;
        canDash = false;
        isAttacking = true;

        // 준비 동작
        anim.SetTrigger("DashPrep");
        if (bossChase != null) bossChase.enabled = false;
        yield return new WaitForSeconds(0.5f); // 준비 시간

        // 돌진 시작
        Vector2 dashDir = (player.position - transform.position).normalized;
        float elapsed = 0f;
        isDashing = true;
        anim.SetTrigger("Dash");

        while (elapsed < dashDuration)
        {
            transform.position += (Vector3)(dashDir * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        // 돌진 종료
        yield return new WaitForSeconds(0.5f); // 후딜
        if (bossChase != null) bossChase.enabled = true;

        isAttacking = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // 돌진 충돌 판정
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.collider.CompareTag("Player"))
        {
            PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(dashDamage, transform.position, dashKnockback);
            }
        }
    }

    // 근접 공격 히트박스 (애니메이션 이벤트에서 호출)
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

    public void StopChase() { if (bossChase != null) bossChase.enabled = false; }
    public void ResumeChase() { if (bossChase != null) bossChase.enabled = true; }

    // 원거리 공격에서 미사일 발사 (애니메이션 이벤트에서 호출)
    public void ShootMissile()
    {
        if (missilePrefab != null && firePoint != null && player != null)
        {
            GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
            Missile m = missile.GetComponent<Missile>();
            if (m != null)
                m.SetTargetDirection((player.position - firePoint.position).normalized);
        }
    }

    void DecideNextPattern()
    {
        List<BossPattern> candidates = new List<BossPattern>();
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= meleeRange)
            candidates.Add(BossPattern.Melee);
        if (distance <= rangedRange && Time.time >= nextRangedAvailableTime)
            candidates.Add(BossPattern.Ranged);
        if (canDash)
            candidates.Add(BossPattern.Dash);

        if (candidates.Count == 0)
        {
            nextPattern = BossPattern.None;
            return;
        }

        if (candidates.Count > 1 && candidates.Contains(lastPattern))
            candidates.Remove(lastPattern);

        int index = Random.Range(0, candidates.Count);
        nextPattern = candidates[index];
        lastPattern = nextPattern;

        if (nextPattern == BossPattern.Ranged)
            nextRangedAvailableTime = Time.time + rangedCooldown * 2f;
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
