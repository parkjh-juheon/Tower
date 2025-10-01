using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Anchors (assign in inspector)")]
    public Transform bottomAnchor;
    public Transform topAnchor;

    // 유틸리티: 청크 높이 (월드 좌표 기준)
    public float Height => topAnchor.position.y - bottomAnchor.position.y;
}
