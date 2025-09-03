using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [Header("버튼")]
    public Button startButton;
    public Button quitButton;

    [Header("이동할 씬 이름")]
    public string gameSceneName = "MainScene"; // 인스펙터에서 지정

    void Start()
    {
        // 버튼 이벤트 등록
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        //  FadeManager를 통해 페이드아웃 + 로딩 → 씬 이동
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
