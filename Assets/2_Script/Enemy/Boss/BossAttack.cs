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
    public float stunTime = 0.5f; // �÷��̾� ���� �ð�
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

    // �뽬 ���� �ڷ�ƾ ����
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

        // ���� ���� ���� ����
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
        if (missilePrefab != null && firePoint != null && target != null)
        {
            GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
            Missile m = missile.GetComponent<Missile>();
            if (m != null)
                m.SetTarget(target); //  targetPoint�� �ڵ� ����
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

        //  �غ� ������ �ٶ� ���� ��� (1: ������, -1: ����)
        dashFacingDirection = Mathf.Sign(transform.localScale.x);

        //  �÷��̾ ��� �ʿ� �ֵ� ���� �ٶ� �������θ� ����
        dashDir = new Vector2(dashFacingDirection, 0f);

        yield return new WaitForSeconds(2f); // �غ� �ִϸ��̼� ����
        StartDash();
    }
    public void StartDash()
    {
        if (!isAttacking) return;

        isDashing = true;
        hasHitPlayer = false; //  �� ���� ���� �� �ʱ�ȭ
        anim.SetTrigger("Dash");
    }

    public void PerformDashHit()
    {
        if (!isDashing || hasHitPlayer) return; //  �̹� �������� ����

        Collider2D hit = Physics2D.OverlapCircle(transform.position, 3f, targetLayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(dashDamage, transform.position, dashKnockback, stunTime * 2f);
                hasHitPlayer = true;  //  ù ��° �ǰ� �� �ߺ� ����
               // StopDash();           //  �ǰ� ��� ���� ����
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

    // =================== ���� ���� ===================
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

        Debug.Log("BossPattern: " + nextPattern);  // �� ���⼭ ���� �α� ���
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
            Gizmos.DrawWireSphere(transform.position, 1f); // OverlapCircle �ݰ� �ð�ȭ
        }
    }
}
