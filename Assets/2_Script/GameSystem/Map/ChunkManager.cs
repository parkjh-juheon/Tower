using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameObject startChunkPrefab;
    [SerializeField] private List<GameObject> randomChunkPrefabs = new List<GameObject>();
    [SerializeField] private GameObject bossChunkPrefab;
    [SerializeField] private int chunksPerRound = 5;

    [Header("������ Ǯ")]
    public List<ItemData> itemPool; // ScriptableObject ������ ����Ʈ

    private List<GameObject> spawnedChunks = new List<GameObject>();

    private static HashSet<string> spawnedItemPositions = new HashSet<string>();

    private void Start()
    {
        BuildTower();
    }

    public void BuildTower()
    {
        ClearTower();

        // 1) StartChunk
        GameObject startChunk = Instantiate(startChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        PositionChunkAt(startChunk, Vector3.zero);
        spawnedChunks.Add(startChunk);

        // ������ ���� ����
        SpawnItemsInChunk(startChunk);

        Chunk startC = startChunk.GetComponent<Chunk>();
        Vector3 attachPoint = startC.topAnchor.position;

        // 2) ���� ûũ
        List<GameObject> pool = new List<GameObject>(randomChunkPrefabs);
        Shuffle(pool);

        for (int i = 0; i < chunksPerRound; i++)
        {
            GameObject prefab = pool[i % pool.Count];
            GameObject chunk = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            PositionChunkAt(chunk, attachPoint);
            spawnedChunks.Add(chunk);

            // ������ ���� ����
            SpawnItemsInChunk(chunk);

            Chunk c = chunk.GetComponent<Chunk>();
            attachPoint = c.topAnchor.position;
        }

        // 3) BossChunk
        GameObject boss = Instantiate(bossChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        PositionChunkAt(boss, attachPoint);
        spawnedChunks.Add(boss);

        // ������ ���� ����
        SpawnItemsInChunk(boss);

        // 3) BossChunk���� ������ ��, �̴ϸʿ� �� ���
        MiniMapController miniMap = FindAnyObjectByType<MiniMapController>();
        if (miniMap != null)
        {
            foreach (var chunk in spawnedChunks)
            {
                foreach (var t in chunk.GetComponentsInChildren<Transform>())
                {
                    if (t.CompareTag("Enemy"))
                        miniMap.RegisterEnemy(t);
                    else if (t.CompareTag("Ground"))
                        miniMap.RegisterGround(t);  // �� �޼��� �ʿ�
                }
            }
        }

    }

    private void PositionChunkAt(GameObject chunk, Vector3 targetPoint)
    {
        Chunk c = chunk.GetComponent<Chunk>();
        if (c == null || c.bottomAnchor == null)
        {
            return;
        }

        Vector3 offset = targetPoint - c.bottomAnchor.position;
        chunk.transform.position += offset;
    }

    private void SpawnItemsInChunk(GameObject chunkGO)
    {
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        if (chunk == null || chunk.itemSpawnPoints == null || chunk.itemSpawnPoints.Length == 0)
            return;

        foreach (Transform spawnPoint in chunk.itemSpawnPoints)
        {
            string key = $"{chunkGO.name}_{spawnPoint.position.x:F2}_{spawnPoint.position.y:F2}_{spawnPoint.position.z:F2}";

            // �̹� ������ ��ġ��� ��ŵ
            if (spawnedItemPositions.Contains(key))
                continue;

            if (Random.value < 1f)
            {
                ItemData itemData = GetRandomItemByRarity();
                GameObject itemObj = Instantiate(itemData.prefab, spawnPoint.position, Quaternion.identity, chunkGO.transform);

                StatItem statItem = itemObj.GetComponent<StatItem>();
                if (statItem != null)
                    statItem.data = itemData;

                MiniMapController miniMap = FindAnyObjectByType<MiniMapController>();
                if (miniMap != null)
                    miniMap.RegisterItem(itemObj.transform);

                spawnedItemPositions.Add(key);
            }
        }
    }

    private ItemData GetRandomItemByRarity()
    {
        // ��͵� Ȯ�� ���� (�� = 1.0f)
        float commonChance = 0.5f;
        float rareChance = 0.3f;
        float epicChance = 0.13f;

        float roll = Random.value;

        ItemRarity selectedRarity;
        if (roll < commonChance) selectedRarity = ItemRarity.Common;
        else if (roll < commonChance + rareChance) selectedRarity = ItemRarity.Rare;
        else if (roll < commonChance + rareChance + epicChance) selectedRarity = ItemRarity.Epic;
        else selectedRarity = ItemRarity.Legendary;

        // ���õ� ��͵��� �ش��ϴ� �����۸� ���͸�
        List<ItemData> candidates = itemPool.FindAll(item => item.rarity == selectedRarity);

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"[{selectedRarity}] ��͵��� �������� �����ϴ�. Common���� ��ü�մϴ�.");
            candidates = itemPool.FindAll(item => item.rarity == ItemRarity.Common);
        }

        return candidates[Random.Range(0, candidates.Count)];
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

    public void ResetTower()
    {
        ClearTower();
        BuildTower();
    }
}
