using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("UI")]
    public Image fadePanel;            // 검은 화면
    public GameObject loadingPanel;    // "Loading..." 패널
    public TextMeshProUGUI loadingText;
    [SerializeField] private float fadeDuration = 1f; // 인스펙터에서 조절 (기본 1초)
    [SerializeField] private float blackScreenDuration = 0.5f; // 검정 화면 지속 시간 (기본 0.5초)

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // 페이드 아웃
        yield return StartCoroutine(Fade(1f));

        // 로딩 UI 활성화 (페이드 아웃 완료 직후)
        loadingPanel.SetActive(true);

        // 검정 화면 지속
        yield return new WaitForSeconds(blackScreenDuration);

        // 점점점 효과 시작
        StartCoroutine(LoadingTextEffect());

        // 씬 비동기 로드
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            // 로딩이 끝났을 때 씬 전환
            if (async.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); // 약간 기다렸다가
                async.allowSceneActivation = true;
            }
            yield return null;
        }

        // 로딩 UI 숨기기
        loadingPanel.SetActive(false);

        // 페이드 인
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Color color = fadePanel.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadePanel.color = color;
            yield return null;
        }
        // 마지막 알파값 보정
        color.a = targetAlpha;
        fadePanel.color = color;
    }

    private IEnumerator LoadingTextEffect()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (loadingPanel.activeSelf)
        {
            loadingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // 0~3 반복
            yield return new WaitForSeconds(0.5f);
        }
    }
}
