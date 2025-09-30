using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private enum BossPattern { None, Melee, Ranged, Dash }
    private BossPattern nextPattern = BossPattern.None;

    [Header("���� ����")]
    public float meleeRange = 3f;
    public float rangedRange = 8f;
    public float meleeStopDistance = 3f;
    public float rangedStopDistance = 8f;

    [Header("��ٿ�")]
    public float meleeCooldown = 2f;
    public float rangedCooldown = 3f;
    private float nextRangedAvailableTime = 0f;

    [Header("���� ����")]
    public float dashSpeed = 10f;
    public float dashCooldown = 5f;
    public int dashDamage = 25;
    public float dashKnockback = 12f;
    public float dashStunDuration = 1.0f; // �� �浹 �� �ൿ �Ұ� �ð�
    
    private bool isStunned = false;
    public float stunDuration = 0.5f;
    public float backOffDistance = 1f;

    [Header("���� ����")]
    public int attackDamage = 20;
    public float knockbackPower = 8f;
    public Transform hitboxCenter;
    public Vector2 hitboxSize = new Vector2(2f, 1f);
    public LayerMask targetLayer;

    [Header("���Ÿ� ����")]
    public GameObject missilePrefab;
    public Transform firePoint;

    [Header("ī�޶� ��鸲")]
    public Cinemachine.CinemachineImpulseSource impulseSource;

    public BossChase bossChase;
    public Transform player;

    private Animator anim;
    private bool isAttacking = false;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector2 dashDir;
    private BossPattern lastPattern = BossPattern.None;

    private Rigidbody2D rb;

    // �뽬 ���� �ڷ�ƾ ����
    private Coroutine dashPrepCoroutine;
    private Coroutine dashCooldownCoroutine;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (bossChase == null) bossChase = GetComponent<BossChase>();
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // ���� ���̸� �߰� ��Ȱ��ȭ + �ൿ �ߴ�
        if (isStunned)
        {
            if (bossChase != null && bossChase.enabled)
                bossChase.enabled = false;
            return;
        }

        // ���� �� �̵� ó��
        if (isDashing)
        {
            rb.MovePosition(rb.position + dashDir * dashSpeed * Time.fixedDeltaTime);
            return;
        }

        if (player == null || isAttacking) return;

        // ���� ���� ����
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
                DashPrep();
                nextPattern = BossPattern.None;
                break;
        }
    }


    // =================== �ִϸ��̼� �̺�Ʈ ===================
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

    // =================== ���� ���� ===================
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

    // =================== ���Ÿ� ���� ===================
    IEnumerator DoRangedAttack()
    {
        isAttacking = true;
        anim.SetTrigger("RangedAttack");
        yield return new WaitForSeconds(rangedCooldown);
        isAttacking = false;
    }

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

    // =================== ���� ���� ===================
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

        // �غ� �� �ٶ� �������� dashDir ����
        dashDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        yield return new WaitForSeconds(2f); // �غ� �ִϸ��̼� ����
        StartDash();
    }

    public void StartDash()
    {
        if (!isAttacking) return;

        isDashing = true;
        anim.SetTrigger("Dash");
    }

    public void PerformDashHit()
    {
        if (!isDashing) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, 1f, targetLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(dashDamage, transform.position, dashKnockback);
        }
    }

    private void StopDash()
    {
        // �뽬 ���� �ڷ�ƾ ����
        if (dashPrepCoroutine != null)
        {
            StopCoroutine(dashPrepCoroutine);
            dashPrepCoroutine = null;
        }
        if (dashCooldownCoroutine != null)
        {
            StopCoroutine(dashCooldownCoroutine);
            dashCooldownCoroutine = null;
        }

        isDashing = false;
        isAttacking = false;

        anim.ResetTrigger("Dash");
        anim.ResetTrigger("DashPrep");
        anim.SetTrigger("EndDash");

        //  ī�޶� ��鸲
        if (impulseSource != null)
            impulseSource.GenerateImpulse();

        StartCoroutine(DashStunCoroutine());
    }

    private IEnumerator DashStunCoroutine()
    {
        isStunned = true;

        // ���� �̵�/�߰� ����
        if (bossChase != null)
            bossChase.enabled = false;

        // ���� ���� �ð�
        yield return new WaitForSeconds(dashStunDuration);

        // ���� ����
        isStunned = false;

        // ������ ���� �Ŀ��� �߰� ����
        if (bossChase != null)
            bossChase.enabled = true;

        // ��ٿ� ����
        dashCooldownCoroutine = StartCoroutine(DashCooldownCoroutine());
    }

    private IEnumerator StunAndBackOff(Vector2 knockbackDir)
    {
        if (isStunned) yield break;
        isStunned = true;

        // ���� ����
        isDashing = false;
        if (bossChase != null) bossChase.enabled = false;

        // ��¦ �ڷ� �о��
        Vector3 backPos = transform.position - (Vector3)knockbackDir.normalized * backOffDistance;
        transform.position = backPos;

        // ���� ����
        yield return new WaitForSeconds(stunDuration);

        // ���� ���� �� Chase ��Ȱ��ȭ
        if (bossChase != null) bossChase.enabled = true;
        isStunned = false;
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

        if (collision.collider.CompareTag("Player"))
        {
            // �÷��̾� ������ + �˹�
            PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(dashDamage, transform.position, dashKnockback);
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            // �� �浹 �� �ڷ� ��¦ ƨ���
            Vector2 backDir = -dashDir; // ���� ���� ���� �ݴ�
            rb.MovePosition(rb.position + backDir * backOffDistance);

            StopDash(); // EndDash �ִϸ��̼� + ī�޶� ��鸲 ó��
            Debug.Log("Dash stopped and boss bounced back from wall.");
        }
    }


    // =================== ���� ���� ===================
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

        if (isDashing)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, dashDir * 2f);
        }
    }
}
