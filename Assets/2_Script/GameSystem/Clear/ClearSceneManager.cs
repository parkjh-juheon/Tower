using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearSceneManager : MonoBehaviour
{
    [Header("버튼 설정")]
    public Button restartButton;
    public Button titleButton;

    [Header("Scene Loader 참조")]
    public SceneLoader sceneLoader;

    [Header("버튼 활성화 대기 시간")]
    public float delayTime = 5f;

    void Start()
    {
        // 처음엔 버튼 비활성화
        restartButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(false);

        // 5초 후 버튼 활성화
        StartCoroutine(ActivateButtonsAfterDelay());
    }

    IEnumerator ActivateButtonsAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        restartButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true);

        // 버튼 클릭 이벤트 연결
        restartButton.onClick.AddListener(() => sceneLoader.ReloadCurrentScene());
        titleButton.onClick.AddListener(() => sceneLoader.LoadMainMenu());
    }
}
