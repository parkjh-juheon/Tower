using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Anchors (assign in inspector)")]
    public Transform bottomAnchor;
    public Transform topAnchor;

    [Header("아이템 스폰 위치")]
    public Transform[] itemSpawnPoints;  // 청크 내에서 아이템이 배치될 위치들

    // 유틸리티: 청크 높이 (월드 좌표 기준)
    public float Height => topAnchor.position.y - bottomAnchor.position.y;
}
