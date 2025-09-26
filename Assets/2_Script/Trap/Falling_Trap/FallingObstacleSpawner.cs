using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingObstacleSpawner : MonoBehaviour
{
    [Header("장애물 설정")]
    public GameObject[] obstaclePrefabs;
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

        // === 땅까지 길이 자동 맞추기 ===
        float maxDistance = 100f; // 최대 탐지 거리
        RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, maxDistance, LayerMask.GetMask("RealGround"));
        float endY = hit.collider != null ? hit.point.y : spawnPos.y - 50f;

        lineRenderer.SetPosition(0, spawnPos);
        lineRenderer.SetPosition(1, new Vector3(spawnPos.x, endY, spawnPos.z));

        // 2. 깜빡이는 효과 (Fade In/Out)
        Coroutine fadeCoroutine = StartCoroutine(FadeEffect(lineRenderer));

        // 3. 경고 시간만큼 대기
        yield return new WaitForSeconds(warningDuration);

        // 4. 장애물 생성
        int prefabIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject obj = Instantiate(obstaclePrefabs[prefabIndex], spawnPos, Quaternion.identity);
        FallingObstacle obstacle = obj.GetComponent<FallingObstacle>();
        if (obstacle != null)
        {
            obstacle.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        }

        // 5. 코루틴 정지 & 경고선 파괴
        StopCoroutine(fadeCoroutine);
        Destroy(warningLineObj);
    }

    // 경고선을 Fade In/Out 시키는 코루틴
    // 경고선을 Fade In/Out 시키는 코루틴
    private IEnumerator FadeEffect(LineRenderer lineRenderer)
    {
        Color baseStart = lineRenderer.startColor;
        Color baseEnd = lineRenderer.endColor;

        float alpha = 1f;
        bool fadingOut = true;

        while (true)
        {
            if (fadingOut)
                alpha -= Time.deltaTime * 2f; // 0.5초에 완전히 사라짐
            else
                alpha += Time.deltaTime * 2f;

            alpha = Mathf.Clamp01(alpha);

            Color newStart = new Color(baseStart.r, baseStart.g, baseStart.b, alpha);
            Color newEnd = new Color(baseEnd.r, baseEnd.g, baseEnd.b, alpha);

            lineRenderer.startColor = newStart;
            lineRenderer.endColor = newEnd;

            if (alpha <= 0f) fadingOut = false;
            if (alpha >= 1f) fadingOut = true;

            yield return null;
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
