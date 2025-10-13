using UnityEngine;

public class ClearTrigger : MonoBehaviour
{
    private SceneLoader sceneLoader;

    void Start()
    {
        // �� �δ� ���� (Hierarchy �� SceneLoader�� �־�� ��)
        sceneLoader = FindAnyObjectByType<SceneLoader>();
    }

    void Update()
    {
        // C Ű�� ������ Clear ������ �̵�
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("Clear");
            }
            else
            {
                Debug.LogWarning("SceneLoader�� �������� �ʽ��ϴ�! ���� SceneLoader ������Ʈ�� �߰��ϼ���.");
            }
        }
    }
}
