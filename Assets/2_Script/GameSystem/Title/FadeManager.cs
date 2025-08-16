using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance; // �̱���

    [Header("UI ������Ʈ")]
    public Image fadeImage;       // ȭ�� ���� ������ �̹���
    public GameObject loadingUI;  // �ε� ȭ�� (ȸ�� ������ ���� ��)

    [Header("������")]
    public float fadeDuration = 1f; // ���̵� �ð�

    private void Awake()
    {
        // �̱��� ����
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
        // 1. ���̵� �ƿ�
        yield return StartCoroutine(Fade(1));

        // 2. �ε� UI �ѱ�
        if (loadingUI != null) loadingUI.SetActive(true);

        // 3. �� �񵿱� �ε�
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; // 100% �� ������ ���

        while (op.progress < 0.9f)
        {
            yield return null; // �ε���
        }

        // �ε� �� (��� ������ ����)
        yield return new WaitForSeconds(1f);

        op.allowSceneActivation = true; // �� ��ȯ ����

        // 4. �� �ε� �Ϸ� �� �ε� UI ����
        if (loadingUI != null) loadingUI.SetActive(false);

        // 5. ���̵� ��
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
