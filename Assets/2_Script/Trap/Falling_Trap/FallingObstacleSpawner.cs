using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingObstacleSpawner : MonoBehaviour
{
    [Header("��ֹ� ����")]
    public GameObject[] obstaclePrefabs;
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

        // === ������ ���� �ڵ� ���߱� ===
        float maxDistance = 100f; // �ִ� Ž�� �Ÿ�
        RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, maxDistance, LayerMask.GetMask("RealGround"));
        float endY = hit.collider != null ? hit.point.y : spawnPos.y - 50f;

        lineRenderer.SetPosition(0, spawnPos);
        lineRenderer.SetPosition(1, new Vector3(spawnPos.x, endY, spawnPos.z));

        // 2. �����̴� ȿ�� (Fade In/Out)
        Coroutine fadeCoroutine = StartCoroutine(FadeEffect(lineRenderer));

        // 3. ��� �ð���ŭ ���
        yield return new WaitForSeconds(warningDuration);

        // 4. ��ֹ� ����
        int prefabIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject obj = Instantiate(obstaclePrefabs[prefabIndex], spawnPos, Quaternion.identity);
        FallingObstacle obstacle = obj.GetComponent<FallingObstacle>();
        if (obstacle != null)
        {
            obstacle.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        }

        // 5. �ڷ�ƾ ���� & ��� �ı�
        StopCoroutine(fadeCoroutine);
        Destroy(warningLineObj);
    }

    // ����� Fade In/Out ��Ű�� �ڷ�ƾ
    // ����� Fade In/Out ��Ű�� �ڷ�ƾ
    private IEnumerator FadeEffect(LineRenderer lineRenderer)
    {
        Color baseStart = lineRenderer.startColor;
        Color baseEnd = lineRenderer.endColor;

        float alpha = 1f;
        bool fadingOut = true;

        while (true)
        {
            if (fadingOut)
                alpha -= Time.deltaTime * 2f; // 0.5�ʿ� ������ �����
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
