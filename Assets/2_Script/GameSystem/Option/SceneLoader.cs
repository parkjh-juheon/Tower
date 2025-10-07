using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("페이드용 이미지 (검은색 이미지 추천)")]
    public Image fadeImage; // 전체화면 UI Image
    public float fadeDuration = 1f; // 페이드 시간 (초)

    private void Start()
    {
        // 씬이 시작될 때 자동으로 페이드 인
        if (fadeImage != null)
            StartCoroutine(FadeIn());
    }

    // 특정 씬으로 이동 (페이드 아웃 후 전환)
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    // 현재 씬 다시 불러오기
    public void ReloadCurrentScene()
    {
        StartCoroutine(FadeOutAndLoad(SceneManager.GetActiveScene().name));
    }

    // 메인 메뉴로 이동
    public void LoadMainMenu()
    {
        StartCoroutine(FadeOutAndLoad("Title"));
    }

    // 게임 종료
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- 페이드 연출 ---

    IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = 1f - (timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = timer / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // 씬 전환 (타임스케일 복구)
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}

