using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance; // 싱글톤

    [Header("UI 컴포넌트")]
    public Image fadeImage;       // 화면 덮는 검은색 이미지
    public GameObject loadingUI;  // 로딩 화면 (회전 아이콘 같은 거)

    [Header("설정값")]
    public float fadeDuration = 1f; // 페이드 시간

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // 1. 페이드 아웃
        yield return StartCoroutine(Fade(1));

        // 2. 로딩 UI 켜기
        if (loadingUI != null) loadingUI.SetActive(true);

        // 3. 씬 비동기 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; // 100% 찰 때까지 대기

        while (op.progress < 0.9f)
        {
            yield return null; // 로딩중
        }

        // 로딩 끝 (잠깐 딜레이 가능)
        yield return new WaitForSeconds(1f);

        op.allowSceneActivation = true; // 씬 전환 실행

        // 4. 씬 로드 완료 후 로딩 UI 끄기
        if (loadingUI != null) loadingUI.SetActive(false);

        // 5. 페이드 인
        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}
