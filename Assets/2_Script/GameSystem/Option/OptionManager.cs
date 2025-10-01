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

        // 커서 활성화 (옵션 조작 가능하도록)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        optionPanel.SetActive(false);  // 옵션창 닫기
        Time.timeScale = 1f;           // 게임 재개
        isPaused = false;

        // 필요시 커서 다시 잠그기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        // 게임 종료 (빌드 환경)
        Application.Quit();

#if UNITY_EDITOR
        // 유니티 에디터에서는 Play 모드 중지
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
