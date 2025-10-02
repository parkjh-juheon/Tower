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
        // 1) 플레이어 위치 초기화
        player.transform.position = startPosition;

        // 2) 플레이어 스탯 초기화
        player.SetAttackType(PlayerController.AttackType.Melee); // 기본 공격으로
        player.stats.attackDamage = 10;   // 기본값
        player.stats.moveSpeed = 12f;
        player.stats.maxJumpCount = 1;
        player.stats.jumpForce = 12f;
        player.stats.attackCooldown = 0.5f;
        playerHealth.UpdateMaxHP(100);    // 기본값



        // 3) Chunk 초기화
        chunkManager.ResetTower();

        // 4) 아이템 초기화
        foreach (var item in collectedItems)
        {
            if (item != null)
                Destroy(item);
        }
        collectedItems.Clear();

        Debug.Log("게임 초기화 완료!");
    }
}
