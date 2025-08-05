using UnityEngine;

public class ForceTrap : MonoBehaviour
{
    [Header("Ʈ���� ����")]
    public float triggerRadius = 3f;

    [Header("�� ����")]
    public float forceStrength = 10f;
    public bool isPulling = true; // true = ������, false = �о

    [Header("�ߵ� �ð� ����")]
    public float forceDuration = 0.5f; // �� ���� �ð�
    public float forceCooldown = 2f;   // ���� �ߵ����� ��� �ð�

    private Transform player;
    private Rigidbody2D playerRb;
    private bool isActive = false;
    private float forceTimer = 0f;
    private float cooldownTimer = 0f;

    void Update()
    {
        if (player != null)
        {
            cooldownTimer += Time.deltaTime;

            float distance = Vector2.Distance(transform.position, player.position);

            // ���� �ȿ� �ְ�, ��ٿ��� ���� ���¿����� �ߵ�
            if (!isActive && cooldownTimer >= forceCooldown && distance <= triggerRadius)
            {
                StartForce();
            }

            // ���� ����Ǵ� ��
            if (isActive)
            {
                ApplyForce();
                forceTimer += Time.deltaTime;
                if (forceTimer >= forceDuration)
                {
                    isActive = false;
                    cooldownTimer = 0f; // ��ٿ� ����
                }
            }
        }
    }

    void StartForce()
    {
        isActive = true;
        forceTimer = 0f;
    }

    void ApplyForce()
    {
        if (playerRb == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        if (!isPulling)
            direction *= -1; // �о��� ���� ����

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
            isActive = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
