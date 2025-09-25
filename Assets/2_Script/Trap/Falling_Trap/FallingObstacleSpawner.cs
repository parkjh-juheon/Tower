using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingObstacleSpawner : MonoBehaviour
{
    [Header("��ֹ� ����")]
    public GameObject obstaclePrefab;
    public Transform spawnPoint;
    public float spawnRangeX = 5f;   // �¿� ���� ����

    [Header("���� �ֱ� (����)")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    [Header("���� ���� (����)")]
    public int minSpawnCount = 1;
    public int maxSpawnCount = 3;

    [Header("���� �ӵ� (����)")]
    public float minFallSpeed = 3f;
    public float maxFallSpeed = 7f;

    [Header("��� ����")]
    public GameObject warningLinePrefab; 
    public float warningDuration = 1.0f; // ����� ǥ�õǴ� �ð�

    [Header("��ħ ����")]
    public float minSpacing = 1.0f;   // X ��ǥ �� �ּ� ����

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
        // 1. ��� ����
        GameObject warningLineObj = Instantiate(warningLinePrefab, spawnPos, Quaternion.identity);
        LineRenderer lineRenderer = warningLineObj.GetComponent<LineRenderer>();

        // Line Renderer�� �� ���� ���� (�Ʒ������� 15 ���� ������ ����)
        lineRenderer.SetPosition(0, spawnPos);
        lineRenderer.SetPosition(1, new Vector3(spawnPos.x, spawnPos.y - 50f, spawnPos.z));

        // 2. �����̴� ȿ���� ���� �ڷ�ƾ ���� (BlinkEffect �ڷ�ƾ�� ������ ����)
        Coroutine blinkCoroutine = StartCoroutine(BlinkEffect(lineRenderer));

        // 3. ��� �ð���ŭ ���
        yield return new WaitForSeconds(warningDuration);

        // 4. ��ֹ� ����
        GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        FallingObstacle obstacle = obj.GetComponent<FallingObstacle>();
        if (obstacle != null)
        {
            obstacle.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        }

        // 5. ��� �ı� ���� ������ �ڷ�ƾ�� ����ϴ�.
        StopCoroutine(blinkCoroutine);

        // 6. ��� �ı�
        Destroy(warningLineObj);
    }

    // ����� �����̰� ����� �ڷ�ƾ
    private IEnumerator BlinkEffect(LineRenderer lineRenderer)
    {
        while (true)
        {
            // 0.2�ʸ��� ������ �����Ͽ� �����̴� ȿ�� ����
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
