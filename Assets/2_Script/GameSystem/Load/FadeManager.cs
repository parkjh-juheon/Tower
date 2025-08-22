using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("UI")]
    public Image fadePanel;            // ���� ȭ��
    public GameObject loadingPanel;    // "Loading..." �г�
    public TextMeshProUGUI loadingText;
    [SerializeField] private float fadeDuration = 1f; // �ν����Ϳ��� ���� (�⺻ 1��)
    [SerializeField] private float blackScreenDuration = 0.5f; // ���� ȭ�� ���� �ð� (�⺻ 0.5��)

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
        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(1f));

        // �ε� UI Ȱ��ȭ (���̵� �ƿ� �Ϸ� ����)
        loadingPanel.SetActive(true);

        // ���� ȭ�� ����
        yield return new WaitForSeconds(blackScreenDuration);

        // ������ ȿ�� ����
        StartCoroutine(LoadingTextEffect());

        // �� �񵿱� �ε�
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            // �ε��� ������ �� �� ��ȯ
            if (async.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); // �ణ ��ٷȴٰ�
                async.allowSceneActivation = true;
            }
            yield return null;
        }

        // �ε� UI �����
        loadingPanel.SetActive(false);

        // ���̵� ��
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
        // ������ ���İ� ����
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
            dotCount = (dotCount + 1) % 4; // 0~3 �ݺ�
            yield return new WaitForSeconds(0.5f);
        }
    }
}
