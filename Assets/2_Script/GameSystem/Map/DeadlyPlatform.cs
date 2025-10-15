using UnityEngine;

public class DeadlyPlatform : MonoBehaviour
{
    [Header("낙하 감지 기준")]
    public float deadlyFallThreshold = 5f; // 이 높이 이상으로 떨어져야 리셋 발생

    private void Start()
    {
        // PlayerFallTracker 찾기
        PlayerFallTracker player = FindAnyObjectByType<PlayerFallTracker>();
        if (player != null)
        {
            player.OnHighFallLanded += (Collider2D hitCollider) => HandleHighFallLand(player, hitCollider);
        }
    }

    private void HandleHighFallLand(PlayerFallTracker player, Collider2D hitCollider)
    {
        // 착지한 발판이 자기 자신인지 확인
        if (hitCollider.GetComponentInParent<DeadlyPlatform>() == this)
        {
            // 플레이어가 최근 높은 낙하 상태일 때만 작동
            if (player.hasRecentHighFall)
            {
                SceneLoader loader = FindAnyObjectByType<SceneLoader>();
                if (loader != null)
                    loader.ReloadCurrentScene();
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene(
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                    );
            }
        }
    }
}
