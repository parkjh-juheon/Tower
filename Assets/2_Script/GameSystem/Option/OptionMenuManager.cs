using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionMenuManager : MonoBehaviour
{
    public GameObject optionPanel;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOption();
        }
    }

    public void ToggleOption()
    {
        isPaused = !isPaused;
        optionPanel.SetActive(isPaused);

        if (isPaused)
            Time.timeScale = 0f;  // ���� �Ͻ�����
        else
            Time.timeScale = 1f;  // ���� �簳
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f; // ���� ���¿��� �� ��ȯ �� �ݵ�� ������� ������ ��

        // FadeManager�� ����� �� ��ȯ
        FadeManager.Instance.LoadScene("Title 1"); // "Title"�� ���� Ÿ��Ʋ �� �̸����� ����
    }
}
