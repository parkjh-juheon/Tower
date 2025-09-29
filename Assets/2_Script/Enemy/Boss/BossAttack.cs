using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{

    private enum BossPattern { None, Melee, Ranged }
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



    // 애니메이션 이벤트에서 호출됨
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

    // 공격 시작 시 BossChase 비활성화
    public void StopChase()
    {
        if (bossChase != null) bossChase.enabled = false;
    }

    // 공격 종료 시 BossChase 활성화
    public void ResumeChase()
    {
        if (bossChase != null) bossChase.enabled = true;
    }

    // 원거리 공격에서 미사일 발사
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
        {
            candidates.Add(BossPattern.Melee);
        }
        if (distance <= rangedRange && Time.time >= nextRangedAvailableTime)
        {
            candidates.Add(BossPattern.Ranged);
        }

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

        // 원거리 패턴 선택 시 쿨다운 적용
        if (nextPattern == BossPattern.Ranged)
            nextRangedAvailableTime = Time.time + rangedCooldown * 2f; // 쿨다운 2배
    }



    void OnDrawGizmosSelected()
    {
        if (hitboxCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(hitboxCenter.position, hitboxSize);
        }

        if (player != null)
        {
            // 근접 사거리 (파란색)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, meleeRange);

            // 원거리 사거리 (초록색)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangedRange);
        }
        else
        {
            // 플레이어가 없어도 중심은 보스 위치
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, meleeRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, rangedRange);
        }
    }

}
