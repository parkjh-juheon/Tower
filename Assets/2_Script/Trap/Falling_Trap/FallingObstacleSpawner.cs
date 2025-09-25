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

            List<float> usedPositions = new List<float>(); // 이번 스폰에서 사용된 X 좌표

            for (int i = 0; i < spawnCount; i++)
            {
                float spawnX;
                int safety = 0;

                // 최소 간격을 만족하는 X 좌표 찾기
                do
                {
                    spawnX = Random.Range(-spawnRangeX, spawnRangeX);
                    safety++;
                    if (safety > 20) break; // 무한 루프 방지
                }
                while (IsTooClose(spawnX, usedPositions));

                usedPositions.Add(spawnX);

                Vector3 spawnPos = spawnPoint.position;
                spawnPos.x += spawnX;

                GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

                // 낙하 속도 랜덤 적용
                FallingObstacle obstacle = obj.GetComponent<FallingObstacle>();
                if (obstacle != null)
                {
                    obstacle.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
                }
            }

            yield return new WaitForSeconds(interval);
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
