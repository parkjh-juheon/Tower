using UnityEngine;

public class BossPlatformTrigger : MonoBehaviour
{
    public GameObject bossObject;      // 비활성화 상태인 보스
    public GameObject bossUIObject;    // 비활성화 상태인 보스 UI

    private bool activated = false;    // 한 번만 활성화

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            if (bossObject != null)
                bossObject.SetActive(true);

            if (bossUIObject != null)
                bossUIObject.SetActive(true);

            activated = true;
        }
    }
}
