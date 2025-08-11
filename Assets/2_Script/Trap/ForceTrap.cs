using UnityEngine;

public class ForceTrap : MonoBehaviour
{
    [Header("트리거 범위")]
    public float triggerRadius = 3f;

    [Header("힘 설정")]
    public float forceStrength = 10f;
    public bool isPulling = true;

    [Header("발동 시간 설정")]
    public float forceDuration = 0.5f;
    public float forceCooldown = 2f;

    [Header("이펙트 설정")]
    public GameObject effectObject; // 이펙트 오브젝트
    public bool useDifferentEffects = false;
    public GameObject pullEffect; // 끌어당김 전용
    public GameObject pushEffect; // 밀어내기 전용

    private Transform player;
    private Rigidbody2D playerRb;
    private bool isActive = false;
    private float forceTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        if (effectObject != null) effectObject.SetActive(false);
        if (useDifferentEffects)
        {
            if (pullEffect != null) pullEffect.SetActive(false);
            if (pushEffect != null) pushEffect.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null)
        {
            cooldownTimer += Time.deltaTime;

            float distance = Vector2.Distance(transform.position, player.position);

            if (!isActive && cooldownTimer >= forceCooldown && distance <= triggerRadius)
            {
                StartForce();
            }

            if (isActive)
            {
                ApplyForce();
                forceTimer += Time.deltaTime;
                if (forceTimer >= forceDuration)
                {
                    StopForce();
                }
            }
        }
    }

    void StartForce()
    {
        isActive = true;
        forceTimer = 0f;
        ShowEffect(true);
    }

    void StopForce()
    {
        isActive = false;
        cooldownTimer = 0f;
        ShowEffect(false);
    }

    void ShowEffect(bool state)
    {
        if (useDifferentEffects)
        {
            if (isPulling && pullEffect != null) pullEffect.SetActive(state);
            if (!isPulling && pushEffect != null) pushEffect.SetActive(state);
        }
        else
        {
            if (effectObject != null) effectObject.SetActive(state);
        }
    }

    void ApplyForce()
    {
        if (playerRb == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        if (!isPulling)
            direction *= -1;

        playerRb.AddForce(direction * forceStrength, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerRb = other.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            playerRb = null;
            StopForce();
        }

        void StopForce()
        {
            isActive = false;
            cooldownTimer = 0f;
            ShowEffect(false);

            // 밀기/당기기 끝난 뒤에만 참조 해제
            player = null;
            playerRb = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
