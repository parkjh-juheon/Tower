using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameObject startChunkPrefab;
    [SerializeField] private List<GameObject> randomChunkPrefabs = new List<GameObject>();
    [SerializeField] private GameObject bossChunkPrefab;
    [SerializeField] private int chunksPerRound = 5;
    [SerializeField] private GameObject statItemPrefab;
    [SerializeField] private StatItemData[] itemDatabase; // 생성 가능한 아이템 목록

    private List<GameObject> spawnedChunks = new List<GameObject>();

    private void Start()
    {
        BuildTower();
    }


    private void SpawnItemsInChunk(Chunk chunk)
    {
        foreach (var point in chunk.itemSpawnPoints)
        {
            if (Random.value < 0.5f) // 50% 확률로 스폰
            {
                GameObject go = Instantiate(statItemPrefab, point.position, Quaternion.identity);
                StatItem item = go.GetComponent<StatItem>();
                item.data = itemDatabase[Random.Range(0, itemDatabase.Length)];
            }
        }
    }

    public void BuildTower()
    {
        ClearTower();

        // 1) StartChunk (fixed)
        GameObject startChunk = Instantiate(startChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        PositionChunkAt(startChunk, Vector3.zero);
        spawnedChunks.Add(startChunk);

        Chunk startC = startChunk.GetComponent<Chunk>();
        SpawnItemsInChunk(startC); // 아이템 스폰 호출

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
            SpawnItemsInChunk(c); //  랜덤 청크에도 아이템 생성

            attachPoint = c.topAnchor.position;
        }

        // 3) BossChunk (fixed)
        GameObject boss = Instantiate(bossChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        PositionChunkAt(boss, attachPoint);
        spawnedChunks.Add(boss);

        Chunk bossC = boss.GetComponent<Chunk>();
        SpawnItemsInChunk(bossC); // 보스 청크에도 아이템 생성 (원한다면)
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
