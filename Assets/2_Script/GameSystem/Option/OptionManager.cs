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
    }

    public void ResumeGame()
    {
        optionPanel.SetActive(false);  // �ɼ�â �ݱ�
        Time.timeScale = 1f;           // ���� �簳
        isPaused = false;
    }

    public void QuitGame()
    {
        // ���� ���� ���� (���忡���� ����)
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ����� ���� ��� ���� ����
#endif
    }
}
