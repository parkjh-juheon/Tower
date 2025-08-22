using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [Header("��ư")]
    public Button startButton;
    public Button quitButton;

    [Header("�̵��� �� �̸�")]
    public string gameSceneName = "MainScene"; // �ν����Ϳ��� ����

    void Start()
    {
        // ��ư �̺�Ʈ ���
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        //  FadeManager�� ���� ���̵�ƿ� + �ε� �� �� �̵�
        FadeManager.Instance.LoadScene(gameSceneName);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
