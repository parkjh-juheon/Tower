using UnityEngine;

public class BossPlatformTrigger : MonoBehaviour
{
    public GameObject bossObject;      // ��Ȱ��ȭ ������ ����
    public GameObject bossUIObject;    // ��Ȱ��ȭ ������ ���� UI

    private bool activated = false;    // �� ���� Ȱ��ȭ

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
