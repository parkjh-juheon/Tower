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
        spawnedChunks.Add(startChunk);
        Chunk startC = startChunk.GetComponent<Chunk>();
        Vector3 attachPoint = startC.topAnchor.position;

        // 2) 랜덤 청크 n개 (순서 섞기 원하면 Shuffle 사용)
        List<GameObject> pool = new List<GameObject>(randomChunkPrefabs);
        Shuffle(pool);

        for (int i = 0; i < chunksPerRound; i++)
        {
            GameObject prefab = pool[i % pool.Count];
            GameObject chunk = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            PositionChunkAt(chunk, attachPoint);
            spawnedChunks.Add(chunk);
            attachPoint = chunk.GetComponent<Chunk>().topAnchor.position;
        }

        // 3) BossChunk (fixed)
        GameObject boss = Instantiate(bossChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        PositionChunkAt(boss, attachPoint);
        spawnedChunks.Add(boss);
    }

    private void PositionChunkAt(GameObject chunkObj, Vector3 attachPosition)
    {
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        if (chunk == null || chunk.bottomAnchor == null)
        {
            Debug.LogError("Chunk or bottomAnchor missing on " + chunkObj.name);
            return;
        }

        Vector3 bottomWorld = chunk.bottomAnchor.position; // 현재 월드 위치
        Vector3 delta = attachPosition - bottomWorld;
        chunkObj.transform.position += delta;
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
