using UnityEngine;

public class ResetZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� �ʱ�ȭ ȣ��
            GameManager.Instance.ResetProgress();
        }
    }
}
