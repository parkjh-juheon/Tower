using UnityEngine;

public class OptionManager : MonoBehaviour
{
    [Header("�ɼ� �г�")]
    public GameObject optionPanel;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        optionPanel.SetActive(true);   // �ɼ�â ����
        Time.timeScale = 0f;           // ���� ����
        isPaused = true;

        // Ŀ�� Ȱ��ȭ (�ɼ� ���� �����ϵ���)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        optionPanel.SetActive(false);  // �ɼ�â �ݱ�
        Time.timeScale = 1f;           // ���� �簳
        isPaused = false;

        // �ʿ�� Ŀ�� �ٽ� ��ױ�
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        // ���� ���� (���� ȯ��)
        Application.Quit();

#if UNITY_EDITOR
        // ����Ƽ �����Ϳ����� Play ��� ����
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
