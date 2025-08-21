using UnityEngine;

public class OptionManager : MonoBehaviour
{
    [Header("옵션 패널")]
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
        optionPanel.SetActive(true);   // 옵션창 열기
        Time.timeScale = 0f;           // 게임 정지
        isPaused = true;
    }

    public void ResumeGame()
    {
        optionPanel.SetActive(false);  // 옵션창 닫기
        Time.timeScale = 1f;           // 게임 재개
        isPaused = false;
    }

    public void QuitGame()
    {
        // 게임 종료 로직 (빌드에서만 동작)
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서는 종료 대신 실행 멈춤
#endif
    }
}
