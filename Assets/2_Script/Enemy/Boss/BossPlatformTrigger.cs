using UnityEngine;

public class BossPlatformTrigger : MonoBehaviour
{
    public GameObject bossObject;      // ��Ȱ��ȭ ������ ����
    public GameObject bossUIObject;    // ��Ȱ��ȭ ������ ���� UI

    private bool activated = false;    // �� ���� Ȱ��ȭ

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("GroundChecker"))
        {
            if (bossObject != null)
                bossObject.SetActive(true);

            if (bossUIObject != null)
                bossUIObject.SetActive(true);

            activated = true;
        }
    }
}
