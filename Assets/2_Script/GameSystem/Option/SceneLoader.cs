using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("���̵�� �̹��� (������ �̹��� ��õ)")]
    public Image fadeImage; // ��üȭ�� UI Image
    public float fadeDuration = 1f; // ���̵� �ð� (��)

    private void Start()
    {
        // ���� ���۵� �� �ڵ����� ���̵� ��
        if (fadeImage != null)
            StartCoroutine(FadeIn());
    }

    // Ư�� ������ �̵� (���̵� �ƿ� �� ��ȯ)
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    // ���� �� �ٽ� �ҷ�����
    public void ReloadCurrentScene()
    {
        StartCoroutine(FadeOutAndLoad(SceneManager.GetActiveScene().name));
    }

    // ���� �޴��� �̵�
    public void LoadMainMenu()
    {
        StartCoroutine(FadeOutAndLoad("Title"));
    }

    // ���� ����
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- ���̵� ���� ---

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

        // �� ��ȯ (Ÿ�ӽ����� ����)
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}

