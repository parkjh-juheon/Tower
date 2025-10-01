using UnityEngine;

public class ResetZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 게임 초기화 호출
            GameManager.Instance.ResetProgress();
        }
    }
}
