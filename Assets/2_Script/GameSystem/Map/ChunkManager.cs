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
        PositionChunkAt(startChunk, Vector3.zero); // bottomAnchor�� (0,0,0)�� ������
        spawnedChunks.Add(startChunk);

        Chunk startC = startChunk.GetComponent<Chunk>();

        Vector3 attachPoint = startC.topAnchor.position;

        // 2) ���� ûũ n��
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

    // ûũ�� Ư�� ������ bottomAnchor �������� ��ġ
    private void PositionChunkAt(GameObject chunk, Vector3 targetPoint)
    {
        Chunk c = chunk.GetComponent<Chunk>();
        if (c == null || c.bottomAnchor == null)
        {
            Debug.LogError($"[PositionChunkAt] {chunk.name}�� Chunk ������Ʈ�� bottomAnchor�� �����ϴ�!");
            return;
        }

        // bottomAnchor�� targetPoint�� ������ �̵���Ų��
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
