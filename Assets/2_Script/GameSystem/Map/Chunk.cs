using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Anchors (assign in inspector)")]
    public Transform bottomAnchor;
    public Transform topAnchor;

    [Header("������ ���� ��ġ")]
    public Transform[] itemSpawnPoints;  // ûũ ������ �������� ��ġ�� ��ġ��

    // ��ƿ��Ƽ: ûũ ���� (���� ��ǥ ����)
    public float Height => topAnchor.position.y - bottomAnchor.position.y;
}
