using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class CharacterStats
    {
        // 공통 스탯
        public int maxHP;
        public float moveSpeed;
        public float dashDistance;
        public float jumpForce;
        public int maxJumpCount;
        public float attackDamage;
        public float attackCooldown;

        // 근접 전용
        public float knockbackPower;   // 넉백
        public float meleeRange;       // 공격 범위

        // 원거리 전용
        public float bulletSize;       // 총알 크기
        public float bulletLifeTime;   // 지속 시간
        public float bulletSpeed;      // 탄속
    }

    public enum AttackType { Melee, Ranged, RapidFire }
    public AttackType attackType = AttackType.Melee;
    public CharacterStats stats;

    [Header("공격 타입별 애니메이터")]
    public RuntimeAnimatorController meleeAnimator;
    public RuntimeAnimatorController rangedAnimator;
    public RuntimeAnimatorController rapidFireAnimator;

    [Header("공격 설정")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float baseAttackCooldown = 0.5f; // 애니메이터 속도 보정용

    [Header("원거리 공격 설정")]
    public GameObject bulletPrefab;
    public GameObject rapidFireBulletPrefab;
    public Transform firePoint;

    [Header("탄약 시스템")]
    public int maxAmmo = 6;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public Image ammoReloadFill;   //  도넛 모양 재장전 표시

    [Header("구르기 설정")]
    [SerializeField] private float rollForce = 6f;
    [SerializeField] private float rollDuration = 0.5f;

    [Header("점프 설정")]
    [SerializeField] private int maxJumpCount = 1;
    private int baseMaxJumpCount;
    private int currentJumpCount = 0;
    private int groundJumpCount = 0;

    [Header("Ground Check 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("공중 공격 설정")]
    [SerializeField] private float airAttackFallSpeed = 20f;
    [SerializeField] public Vector2 airAttackBoxSize = new Vector2(1.5f, 2f);
    [SerializeField] public float airAttackOffsetX = 1f;

    private float lastAttackTime = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = true;
    private bool wasGrounded = false;
    private bool isRolling = false;
    private bool isAirAttacking = false;
    private float rollTimer = 0f;
    private int facingDirection = 1;

    public bool canControl = true;
    public bool airAttackHasHit = false; // 공중 공격 판정용 플래그

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseMaxJumpCount = stats.maxJumpCount;
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    private void Start()
    {
        SetAttackType(attackType);
    }


    private void Update()
    {
        CheckGrounded();
        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);

        HandleMovement();
        HandleJump();
        HandleRoll();
        HandleAttack();

        // Update() 안에서
        if (isAirAttacking && isGrounded)
        {
            PerformAirAttack(); // 착지하자마자 판정 (애니 이벤트 대신)
            EndAirAttack();
        }

        if (isAirAttacking) AirAttackHitCheck();

        UpdateAmmoUI();
    }

    public void SetAttackType(AttackType newType)
    {
        attackType = newType;

        switch (attackType)
        {
            case AttackType.Melee:
                animator.runtimeAnimatorController = meleeAnimator;
                break;

            case AttackType.Ranged:
                animator.runtimeAnimatorController = rangedAnimator;
                maxAmmo = 6;
                currentAmmo = maxAmmo;
                stats.attackCooldown = 0.5f;
                break;

            case AttackType.RapidFire:
                animator.runtimeAnimatorController = rapidFireAnimator;
                maxAmmo = 100;
                currentAmmo = maxAmmo;
                stats.bulletLifeTime = 0.5f;
                stats.attackCooldown = 0.01f;
                break;
        }

        UpdateAmmoUI();
    }

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            currentJumpCount = 0;
            groundJumpCount = 0;
            maxJumpCount = baseMaxJumpCount;
        }

        animator.SetBool("isGrounded", isGrounded);
    }

    private void HandleAttack()
    {
        if (!canControl || isReloading) return;

        if (attackType == AttackType.Melee
            && Input.GetKeyDown(KeyCode.X)
            && Time.time >= lastAttackTime + stats.attackCooldown)
        {
            HandleMeleeAttack();
            lastAttackTime = Time.time;
        }
        else if ((attackType == AttackType.Ranged || attackType == AttackType.RapidFire)
            && Input.GetKey(KeyCode.X)
            && Time.time >= lastAttackTime + stats.attackCooldown)
        {
            if (currentAmmo > 0)
            {
                HandleRangedAttack();
                currentAmmo--;
                UpdateAmmoUI();

                if (currentAmmo <= 0)
                    StartCoroutine(Reload());
            }
            else
            {
                Debug.Log("탄약이 없습니다! 재장전 필요");
            }
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        float elapsed = 0f;
        ammoReloadFill.gameObject.SetActive(true);

        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / reloadTime);
            ammoReloadFill.fillAmount = progress;
            yield return null;
        }

        currentAmmo = maxAmmo;
        isReloading = false;
        ammoReloadFill.fillAmount = 0f;
        ammoReloadFill.gameObject.SetActive(false);
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText == null) return;
        ammoText.gameObject.SetActive(attackType == AttackType.Ranged || attackType == AttackType.RapidFire);

        if (attackType == AttackType.Ranged || attackType == AttackType.RapidFire)
            ammoText.text = $"({currentAmmo} / {maxAmmo})";
    }

    private int currentAttackIndex = 0;

    private void HandleMeleeAttack()
    {
        float speedRatio = baseAttackCooldown / stats.attackCooldown;
        animator.speed = speedRatio;

        if (!isGrounded) StartAirAttack();
        else
        {
            currentAttackIndex = (currentAttackIndex % 3) + 1;
            animator?.SetTrigger($"Attack{currentAttackIndex}");

            Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * stats.meleeRange * 0.5f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, stats.meleeRange, enemyLayer);

            foreach (var hit in hits)
            {
                // 일반 적
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)stats.attackDamage, transform.position, stats.knockbackPower);
                }

                // 보스
                BossHealth boss = hit.GetComponent<BossHealth>();
                if (boss != null)
                {
                    boss.TakeDamage((int)stats.attackDamage);
                }
            }

        }
        StartCoroutine(ResetAnimatorSpeed());
    }

    private void HandleRangedAttack()
    {
        GameObject prefab = (attackType == AttackType.RapidFire && rapidFireBulletPrefab != null)
            ? rapidFireBulletPrefab
            : bulletPrefab;

        if (prefab == null || firePoint == null) return;

        animator?.SetTrigger("Shoot");

        GameObject bulletObj = Instantiate(prefab, firePoint.position, firePoint.rotation);
        if (bulletObj.TryGetComponent(out P_Bullet bullet))
        {
            bullet.Init(
                (int)stats.attackDamage,
                stats.bulletSpeed,
                stats.bulletSize,
                stats.bulletLifeTime,
                facingDirection
            );
        }
    }

    private void HandleMovement()
    {
        if (!canControl || isRolling) return;
        float inputX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputX * stats.moveSpeed, rb.linearVelocity.y);

        if (inputX > 0 && facingDirection != 1)
        {
            facingDirection = 1; spriteRenderer.flipX = true; FlipFirePoint();
        }
        else if (inputX < 0 && facingDirection != -1)
        {
            facingDirection = -1; spriteRenderer.flipX = false; FlipFirePoint();
        }
        animator?.SetFloat("Speed", Mathf.Abs(inputX));
    }

    private void FlipFirePoint()
    {
        if (firePoint != null)
        {
            Vector3 localPos = firePoint.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * facingDirection;
            firePoint.localPosition = localPos;
            firePoint.localRotation = (facingDirection == 1) ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        }
    }

    private void HandleJump()
    {
        if (!canControl || isRolling) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, stats.jumpForce);
                groundJumpCount++;
                currentJumpCount = 0;
                animator?.SetTrigger("Jump");
            }
            else if (currentJumpCount < maxJumpCount)
            {
                if (groundJumpCount == 0 && currentJumpCount == 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, stats.jumpForce);
                    currentJumpCount = 1;
                    maxJumpCount = baseMaxJumpCount + 1;
                    animator?.SetTrigger("Jump");
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, stats.jumpForce);
                    currentJumpCount++;
                    animator?.SetTrigger("Jump");
                }
            }
        }
    }

    private void HandleRoll()
    {
        if (!canControl) return;
        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            if (rollTimer >= rollDuration) { isRolling = false; animator?.SetBool("Roll", false); }
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isRolling = true; rollTimer = 0f;
            rb.linearVelocity = new Vector2(facingDirection * rollForce, rb.linearVelocity.y);
            animator?.SetBool("Roll", true);
        }
    }

    private void StartAirAttack()
    {
        if (!isGrounded)
        {
            isAirAttacking = true;
            canControl = false;
            airAttackHasHit = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -airAttackFallSpeed);

            animator.SetTrigger("Air");
            Debug.Log($"[{Time.time:F2}] [PlayerController] AirAttack 시작 (코드)");
        }
    }


    // 애니 이벤트 또는 호출용으로 수정
    public void PerformAirAttack()
    {
        if (airAttackHasHit) return; // 이미 맞았다면 중복 방지
        airAttackHasHit = true;

        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, airAttackBoxSize, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            // 일반 적
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                int damage = (int)(stats.attackDamage * 2);
                float knockback = stats.knockbackPower * 1.5f;
                enemy.TakeDamage(damage, transform.position, knockback);
            }

            // 보스
            BossHealth boss = hit.GetComponent<BossHealth>();
            if (boss != null)
            {
                int damage = (int)(stats.attackDamage * 2); // 2배 데미지
                boss.TakeDamage(damage);
            }
        }

    }


    private void AirAttackHitCheck()
    {
        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, airAttackBoxSize, 0f, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            // 일반 적
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage((int)stats.attackDamage, transform.position, stats.knockbackPower);

            // 보스
            BossHealth boss = hit.GetComponent<BossHealth>();
            if (boss != null)
                boss.TakeDamage((int)stats.attackDamage);
        }

    }

    private void EndAirAttack()
    {
        isAirAttacking = false; 
        canControl = true;
    }

    private IEnumerator ResetAnimatorSpeed()
    {
        yield return new WaitForSeconds(0.1f);
        animator.speed = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * stats.meleeRange * 0.5f;
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackPosition, stats.meleeRange);

        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);
        Gizmos.color = Color.blue; Gizmos.DrawWireCube(center, airAttackBoxSize);

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow; Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
