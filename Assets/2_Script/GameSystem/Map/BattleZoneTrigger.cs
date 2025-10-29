using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class BattleZoneTrigger : MonoBehaviour
{
    [Header("전투 구간 설정")]
    public GameObject[] walls;
    public string enemyTag = "Enemy";
    public float checkInterval = 1f;

    [Header("아이템 드랍 설정")]
    public List<ItemData> itemPool;
    public int dropCount = 1;
    public float dropRadius = 1.5f;

    private bool battleStarted = false;
    private bool battleEnded = false;
    private Collider2D battleZoneCollider;

    // 마지막 적이 죽은 위치
    private Vector3 lastEnemyPosition;

    private void Awake()
    {
        battleZoneCollider = GetComponent<Collider2D>();
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

        foreach (GameObject wall in walls)
        {
            if (wall != null)
                wall.SetActive(true);
        }

        StartCoroutine(CheckEnemiesCoroutine());
    }

    IEnumerator CheckEnemiesCoroutine()
    {
        while (!battleEnded)
        {
            yield return new WaitForSeconds(checkInterval);

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
        if (battleEnded) return;
        battleEnded = true;

        foreach (GameObject wall in walls)
        {
            if (wall != null)
                wall.SetActive(false);
        }
        //  전투 종료 시 아이템 드랍
        DropItems();
    }

    void DropItems()
    {
        if (itemPool == null || itemPool.Count == 0)
        {
            Debug.LogWarning("[BattleZone] 드랍할 아이템 풀 없음!");
            return;
        }

        //  마지막 적의 위치를 기준으로 드랍 (없으면 전투구역 중앙)
        Vector3 center = (lastEnemyPosition != Vector3.zero)
            ? lastEnemyPosition
            : battleZoneCollider.bounds.center;

        for (int i = 0; i < dropCount; i++)
        {
            ItemData itemData = itemPool[Random.Range(0, itemPool.Count)];
            Vector3 dropPos = center + (Vector3)Random.insideUnitCircle * dropRadius;

            GameObject itemObj = Instantiate(itemData.prefab, dropPos, Quaternion.identity);
            StatItem statItem = itemObj.GetComponent<StatItem>();
            if (statItem != null)
                statItem.data = itemData;

            Debug.Log($"[BattleZone] 아이템 드랍됨: {itemData.itemName} at {dropPos}");
        }
    }

    //  외부(적)에서 호출할 수 있는 메서드
    public void ReportEnemyDeath(Vector3 deathPosition)
    {
        lastEnemyPosition = deathPosition;
    }

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
