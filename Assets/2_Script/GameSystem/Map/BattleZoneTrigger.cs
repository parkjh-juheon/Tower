using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BattleZoneTrigger : MonoBehaviour
{
    [Header("���� ���� ����")]
    public GameObject[] walls;              // ���� �� �÷��̾� �̵� ���ѿ� ��
    public string enemyTag = "Enemy";       // �� �±�
    public float checkInterval = 1f;        // ���� �� üũ �ֱ�(��)

    private bool battleStarted = false;
    private bool battleEnded = false;
    private Collider2D battleZoneCollider;

    private void Awake()
    {
        battleZoneCollider = GetComponent<Collider2D>();

        // Ʈ���� Collider���� ��
        battleZoneCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (battleStarted) return;

        if (other.CompareTag("GroundChecker"))
        {
            StartBattle();
        }
    }

    void StartBattle()
    {
        battleStarted = true;

        // �� Ȱ��ȭ
        foreach (GameObject wall in walls)
        {
            if (wall != null)
                wall.SetActive(true);
        }

        // �ֱ������� ���� �� Ȯ��
        StartCoroutine(CheckEnemiesCoroutine());
    }

    IEnumerator CheckEnemiesCoroutine()
    {
        while (!battleEnded)
        {
            yield return new WaitForSeconds(checkInterval);

            // ���� ���� ������ ���� ��������
            Collider2D[] colliders = Physics2D.OverlapBoxAll(
                battleZoneCollider.bounds.center,
                battleZoneCollider.bounds.size,
                0f
            );

            bool enemiesExist = false;

            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag(enemyTag))
                {
                    enemiesExist = true;
                    break;
                }
            }

            if (!enemiesExist)
            {
                EndBattle();
            }
        }
    }

    void EndBattle()
    {
        battleEnded = true;

        foreach (GameObject wall in walls)
        {
            if (wall != null)
                wall.SetActive(false);
        }

        Debug.Log("[BattleZone] ���� ���� - �� ��Ȱ��ȭ��");
    }

    // ����׿�: ������ ���� ���� �ڽ� �ð�ȭ
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
