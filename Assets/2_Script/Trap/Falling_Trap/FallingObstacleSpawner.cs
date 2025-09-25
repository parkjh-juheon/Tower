using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingObstacleSpawner : MonoBehaviour
{
    [Header("장애물 설정")]
    public GameObject obstaclePrefab;
    public Transform spawnPoint;
    public float spawnRangeX = 5f;   // 좌우 랜덤 범위

    [Header("스폰 주기 (랜덤)")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    [Header("스폰 개수 (랜덤)")]
    public int minSpawnCount = 1;
    public int maxSpawnCount = 3;

    [Header("낙하 속도 (랜덤)")]
    public float minFallSpeed = 3f;
    public float maxFallSpeed = 7f;

    [Header("경고선 설정")]
    public GameObject warningLinePrefab; 
    public float warningDuration = 1.0f; // 경고선이 표시되는 시간

    [Header("겹침 방지")]
    public float minSpacing = 1.0f;   // X 좌표 간 최소 간격

    private void Start()
    {
        if (spawnPoint == null) spawnPoint = transform;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);

            List<float> usedPositions = new List<float>();

            for (int i = 0; i < spawnCount; i++)
            {
                float spawnX;
                int safety = 0;
                do
                {
                    spawnX = Random.Range(-spawnRangeX, spawnRangeX);
                    safety++;
                    if (safety > 20) break;
                }
                while (IsTooClose(spawnX, usedPositions));

                usedPositions.Add(spawnX);

                Vector3 spawnPos = spawnPoint.position;
                spawnPos.x += spawnX;

                StartCoroutine(ShowWarningAndSpawn(spawnPos));
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator ShowWarningAndSpawn(Vector3 spawnPos)
    {
        // 1. 경고선 생성
        GameObject warningLineObj = Instantiate(warningLinePrefab, spawnPos, Quaternion.identity);
        LineRenderer lineRenderer = warningLineObj.GetComponent<LineRenderer>();

        // Line Renderer의 끝 지점 설정 (아래쪽으로 15 유닛 떨어진 지점)
        lineRenderer.SetPosition(0, spawnPos);
        lineRenderer.SetPosition(1, new Vector3(spawnPos.x, spawnPos.y - 50f, spawnPos.z));

        // 2. 깜빡이는 효과를 위한 코루틴 시작 (BlinkEffect 코루틴의 참조를 저장)
        Coroutine blinkCoroutine = StartCoroutine(BlinkEffect(lineRenderer));

        // 3. 경고 시간만큼 대기
        yield return new WaitForSeconds(warningDuration);

        // 4. 장애물 생성
        GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        FallingObstacle obstacle = obj.GetComponent<FallingObstacle>();
        if (obstacle != null)
        {
            obstacle.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        }

        // 5. 경고선 파괴 전에 깜빡이 코루틴을 멈춥니다.
        StopCoroutine(blinkCoroutine);

        // 6. 경고선 파괴
        Destroy(warningLineObj);
    }

    // 경고선을 깜빡이게 만드는 코루틴
    private IEnumerator BlinkEffect(LineRenderer lineRenderer)
    {
        while (true)
        {
            // 0.2초마다 투명도를 조절하여 깜빡이는 효과 구현
            lineRenderer.enabled = !lineRenderer.enabled;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private bool IsTooClose(float newX, List<float> usedPositions)
    {
        foreach (float usedX in usedPositions)
        {
            if (Mathf.Abs(newX - usedX) < minSpacing)
                return true;
        }
        return false;
    }
}
