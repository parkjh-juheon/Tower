using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BattleZoneTrigger : MonoBehaviour
{
    [Header("전투 구간 설정")]
    public GameObject[] walls;              // 전투 중 플레이어 이동 제한용 벽
    public string enemyTag = "Enemy";       // 적 태그
    public float checkInterval = 1f;        // 남은 적 체크 주기(초)

    private bool battleStarted = false;
    private bool battleEnded = false;
    private Collider2D battleZoneCollider;

    private void Awake()
    {
        battleZoneCollider = GetComponent<Collider2D>();

        // 트리거 Collider여야 함
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

        // 벽 활성화
        foreach (GameObject wall in walls)
        {
            if (wall != null)
                wall.SetActive(true);
        }

        // 주기적으로 남은 적 확인
        StartCoroutine(CheckEnemiesCoroutine());
    }

    IEnumerator CheckEnemiesCoroutine()
    {
        while (!battleEnded)
        {
            yield return new WaitForSeconds(checkInterval);

            // 전투 구간 안쪽의 적만 가져오기
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

        Debug.Log("[BattleZone] 전투 종료 - 벽 비활성화됨");
    }

    // 디버그용: 씬에서 전투 구간 박스 시각화
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
