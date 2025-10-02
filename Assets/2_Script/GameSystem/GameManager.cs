using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerController player;
    public ChunkManager chunkManager;
    public PlayerHealth playerHealth;

    private List<GameObject> collectedItems = new List<GameObject>();
    private Vector3 startPosition;

    private void Awake()
    {
        Instance = this;
        startPosition = player.transform.position;
    }

    public void RegisterItem(GameObject item)
    {
        collectedItems.Add(item);
    }

    public void ResetProgress()
    {
        // 1) �÷��̾� ��ġ �ʱ�ȭ
        player.transform.position = startPosition;

        // 2) �÷��̾� ���� �ʱ�ȭ
        player.SetAttackType(PlayerController.AttackType.Melee); // �⺻ ��������
        player.stats.attackDamage = 10;   // �⺻��
        player.stats.moveSpeed = 12f;
        player.stats.maxJumpCount = 1;
        player.stats.jumpForce = 12f;
        player.stats.attackCooldown = 0.5f;
        playerHealth.UpdateMaxHP(100);    // �⺻��



        // 3) Chunk �ʱ�ȭ
        chunkManager.ResetTower();

        // 4) ������ �ʱ�ȭ
        foreach (var item in collectedItems)
        {
            if (item != null)
                Destroy(item);
        }
        collectedItems.Clear();

        Debug.Log("���� �ʱ�ȭ �Ϸ�!");
    }
}
