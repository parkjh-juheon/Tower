using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [Header("World bounds (월드 좌표)")]
    public Vector2 worldMin; // 왼쪽 아래
    public Vector2 worldMax; // 오른쪽 위

    [Header("UI")]
    public RectTransform miniMapArea;      // 미니맵 패널
    public RectTransform playerIcon;       // 플레이어 아이콘
    public RectTransform enemyIconPrefab;  // 적 아이콘 프리팹
    public RectTransform itemIconPrefab;   // 아이템 아이콘 프리팹
    public RectTransform groundIconPrefab; // Ground 아이콘 프리팹

    [Header("Options")]
    public bool hideOffscreenIcons = true; // 미니맵 영역 밖 아이콘 숨김

    // 내부 리스트
    private Transform player;
    private List<Transform> enemies = new List<Transform>();
    private List<Transform> items = new List<Transform>();
    private List<Transform> grounds = new List<Transform>();

    private List<RectTransform> enemyIcons = new List<RectTransform>();
    private List<RectTransform> itemIcons = new List<RectTransform>();
    private List<RectTransform> groundIcons = new List<RectTransform>();

    public Vector2 viewSize = new Vector2(200, 200); // 플레이어 중심으로 보여줄 미니맵 영역


    void Awake()
    {
        if (miniMapArea == null)
            miniMapArea = GetComponent<RectTransform>();
    }

    void Start()
    {
        // 플레이어 찾기
        var pgo = GameObject.FindGameObjectWithTag("Player");
        if (pgo) player = pgo.transform;

        // 초기 오브젝트 등록
        RegisterAllObjectsWithTag("Enemy", enemies, enemyIcons, enemyIconPrefab);
        RegisterAllObjectsWithTag("Item", items, itemIcons, itemIconPrefab);
        RegisterAllObjectsWithTag("Ground", grounds, groundIcons, groundIconPrefab, true);
    }

    void Update()
    {
        if (player) UpdateIcon(playerIcon, player.position);

        UpdateIcons(enemies, enemyIcons);
        UpdateIcons(items, itemIcons);
        UpdateIcons(grounds, groundIcons); // Ground 위치 갱신
    }

    // --------------------
    // 일반 아이콘 갱신
    private void UpdateIcons(List<Transform> targets, List<RectTransform> icons)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
                UpdateIcon(icons[i], targets[i].position);
            else
                icons[i].gameObject.SetActive(false);
        }
    }

    // --------------------
    // 아이콘 위치 갱신
    private void UpdateIcon(RectTransform icon, Vector3 worldPos)
    {
        // 플레이어 중심 좌표 계산
        Vector2 offset = new Vector2(player.position.x, player.position.y);

        // world 범위를 플레이어 중심 영역으로 정규화
        float xNorm = Mathf.InverseLerp(offset.x - viewSize.x / 2, offset.x + viewSize.x / 2, worldPos.x);
        float yNorm = Mathf.InverseLerp(offset.y - viewSize.y / 2, offset.y + viewSize.y / 2, worldPos.y);

        // UI 좌표
        float px = Mathf.Lerp(0f, miniMapArea.rect.width, xNorm) - miniMapArea.rect.width * miniMapArea.pivot.x;
        float py = Mathf.Lerp(0f, miniMapArea.rect.height, yNorm) - miniMapArea.rect.height * miniMapArea.pivot.y;

        icon.anchoredPosition = new Vector2(px, py);

        // Offscreen 처리
        bool insideX = xNorm >= 0f && xNorm <= 1f;
        bool insideY = yNorm >= 0f && yNorm <= 1f;
        icon.gameObject.SetActive(hideOffscreenIcons ? (insideX && insideY) : true);

    }


    // --------------------
    // Ground 아이콘 길이까지 반영하여 등록
    private void RegisterAllObjectsWithTag(string tag, List<Transform> targetList, List<RectTransform> iconList, RectTransform prefab, bool isGround = false)
    {
        var gos = GameObject.FindGameObjectsWithTag(tag);
        foreach (var go in gos)
        {
            targetList.Add(go.transform);
            var icon = Instantiate(prefab, miniMapArea);

            if (isGround)
            {
                BoxCollider2D bc = go.GetComponent<BoxCollider2D>();
                if (bc != null)
                {
                    float worldWidth = bc.bounds.size.x; // World 폭
                    icon.localScale = Vector3.one;
                    icon.sizeDelta = new Vector2(
                        (worldWidth / (worldMax.x - worldMin.x)) * miniMapArea.rect.width,
                        icon.sizeDelta.y
                    );
                }
            }

            iconList.Add(icon);
        }
    }

    // --------------------
    // 동적 등록/제거
    public void RegisterEnemy(Transform enemy) => RegisterDynamic(enemy, enemies, enemyIcons, enemyIconPrefab);
    public void UnregisterEnemy(Transform enemy) => UnregisterDynamic(enemy, enemies, enemyIcons);

    public void RegisterItem(Transform item) => RegisterDynamic(item, items, itemIcons, itemIconPrefab);
    public void UnregisterItem(Transform item) => UnregisterDynamic(item, items, itemIcons);

    public void RegisterGround(Transform ground) => RegisterDynamic(ground, grounds, groundIcons, groundIconPrefab, true);
    public void UnregisterGround(Transform ground) => UnregisterDynamic(ground, grounds, groundIcons);

    private void RegisterDynamic(Transform obj, List<Transform> list, List<RectTransform> iconList, RectTransform prefab, bool isGround = false)
    {
        list.Add(obj);
        var icon = Instantiate(prefab, miniMapArea);

        if (isGround)
        {
            BoxCollider2D bc = obj.GetComponent<BoxCollider2D>();
            if (bc != null)
            {
                float worldWidth = bc.bounds.size.x;
                icon.localScale = Vector3.one;
                icon.sizeDelta = new Vector2(
                    (worldWidth / (worldMax.x - worldMin.x)) * miniMapArea.rect.width,
                    icon.sizeDelta.y
                );
            }
        }

        iconList.Add(icon);
    }

    private void UnregisterDynamic(Transform obj, List<Transform> list, List<RectTransform> iconList)
    {
        int idx = list.IndexOf(obj);
        if (idx >= 0)
        {
            Destroy(iconList[idx].gameObject);
            iconList.RemoveAt(idx);
            list.RemoveAt(idx);
        }
    }
}
