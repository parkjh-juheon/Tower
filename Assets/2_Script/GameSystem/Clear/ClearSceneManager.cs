using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearSceneManager : MonoBehaviour
{
    [Header("��ư ����")]
    public Button restartButton;
    public Button titleButton;

    [Header("Scene Loader ����")]
    public SceneLoader sceneLoader;

    [Header("��ư Ȱ��ȭ ��� �ð�")]
    public float delayTime = 5f;

    void Start()
    {
        // ó���� ��ư ��Ȱ��ȭ
        restartButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(false);

        // 5�� �� ��ư Ȱ��ȭ
        StartCoroutine(ActivateButtonsAfterDelay());
    }

    IEnumerator ActivateButtonsAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        restartButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true);

        // ��ư Ŭ�� �̺�Ʈ ����
        restartButton.onClick.AddListener(() => sceneLoader.ReloadCurrentScene());
        titleButton.onClick.AddListener(() => sceneLoader.LoadMainMenu());
    }
}
