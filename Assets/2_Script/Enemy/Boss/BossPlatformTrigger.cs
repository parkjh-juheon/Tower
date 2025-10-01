using UnityEngine;

public class BossPlatformTrigger : MonoBehaviour
{
    public GameObject bossObject;      // 비활성화 상태인 보스
    public GameObject bossUIObject;    // 비활성화 상태인 보스 UI

    private bool activated = false;    // 한 번만 활성화

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        Debug.Log("BossPlatformTrigger activated by " + other.name);

        if (other.CompareTag("Player"))
        {
            if (bossObject != null)
                bossObject.SetActive(true);
            Debug.Log("Boss object activated.");

            if (bossUIObject != null)
                bossUIObject.SetActive(true);
            Debug.Log("Boss UI object activated.");

            activated = true;
        }
    }
}
