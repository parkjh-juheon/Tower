using UnityEngine;

public class DeadlyPlatform : MonoBehaviour
{
    [Header("���� ���� ����")]
    public float deadlyFallThreshold = 5f; // �� ���� �̻����� �������� ���� �߻�

    private void Start()
    {
        // PlayerFallTracker ã��
        PlayerFallTracker player = FindAnyObjectByType<PlayerFallTracker>();
        if (player != null)
        {
            player.OnHighFallLanded += (Collider2D hitCollider) => HandleHighFallLand(player, hitCollider);
        }
    }

    private void HandleHighFallLand(PlayerFallTracker player, Collider2D hitCollider)
    {
        // ������ ������ �ڱ� �ڽ����� Ȯ��
        if (hitCollider.GetComponentInParent<DeadlyPlatform>() == this)
        {
            // �÷��̾ �ֱ� ���� ���� ������ ���� �۵�
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
