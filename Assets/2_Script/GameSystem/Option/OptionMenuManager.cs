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
            Time.timeScale = 0f;  // 게임 일시정지
        else
            Time.timeScale = 1f;  // 게임 재개
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f; // 멈춘 상태에서 씬 전환 시 반드시 원래대로 돌려야 함

        // FadeManager를 사용해 씬 전환
        FadeManager.Instance.LoadScene("Title 1"); // "Title"은 실제 타이틀 씬 이름으로 변경
    }
}
