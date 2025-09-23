using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameObject startChunkPrefab;
    [SerializeField] private List<GameObject> randomChunkPrefabs = new List<GameObject>();
    [SerializeField] private GameObject bossChunkPrefab;
    [SerializeField] private int chunksPerRound = 5;

    private List<GameObject> spawnedChunks = new List<GameObject>();

    private void Start()
    {
        BuildTower();
    }

    public void BuildTower()
    {
        ClearTower();

        // 1) StartChunk (fixed)
        GameObject startChunk = Instantiate(startChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        PositionChunkAt(startChunk, Vector3.zero); // bottomAnchor가 (0,0,0)에 오도록
        spawnedChunks.Add(startChunk);

        Chunk startC = startChunk.GetComponent<Chunk>();

        Vector3 attachPoint = startC.topAnchor.position;

        // 2) 랜덤 청크 n개
        List<GameObject> pool = new List<GameObject>(randomChunkPrefabs);
        Shuffle(pool);

        for (int i = 0; i < chunksPerRound; i++)
        {
            GameObject prefab = pool[i % pool.Count];
            GameObject chunk = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            PositionChunkAt(chunk, attachPoint);
            spawnedChunks.Add(chunk);

            Chunk c = chunk.GetComponent<Chunk>();

            attachPoint = c.topAnchor.position;
        }

        // 3) BossChunk (fixed)
        GameObject boss = Instantiate(bossChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        PositionChunkAt(boss, attachPoint);
        spawnedChunks.Add(boss);

        Chunk bossC = boss.GetComponent<Chunk>();
    }

    // 청크를 특정 지점에 bottomAnchor 기준으로 배치
    private void PositionChunkAt(GameObject chunk, Vector3 targetPoint)
    {
        Chunk c = chunk.GetComponent<Chunk>();
        if (c == null || c.bottomAnchor == null)
        {
            Debug.LogError($"[PositionChunkAt] {chunk.name}에 Chunk 컴포넌트나 bottomAnchor가 없습니다!");
            return;
        }

        // bottomAnchor가 targetPoint에 오도록 이동시킨다
        Vector3 offset = targetPoint - c.bottomAnchor.position;
        chunk.transform.position += offset;
    }

    private void ClearTower()
    {
        foreach (var go in spawnedChunks)
            if (go != null) Destroy(go);
        spawnedChunks.Clear();
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}
