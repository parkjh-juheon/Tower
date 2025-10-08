using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [Header("옵션 패널")]
    public GameObject optionPanel;

    [Header("인벤토리 캔버스")]
    public Canvas inventoryCanvas; // InventoryCanvas 직접 참조

    private bool isPaused = false;

    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        float bgmValue = Mathf.Pow(10, PlayerPrefs.GetFloat("BGMVolume", 0f) / 20);
        float sfxValue = Mathf.Pow(10, PlayerPrefs.GetFloat("SFXVolume", 0f) / 20);

        bgmSlider.value = bgmValue;
        sfxSlider.value = sfxValue;

        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);

        // 현재 씬이 Title이면 커서 항상 활성화
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Title")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // 다른 씬에서는 기본적으로 잠금
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // Tab키로 인벤토리 캔버스 토글
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryCanvas();
        }
    }

    void ToggleInventoryCanvas()
    {
        if (inventoryCanvas == null) return;
        bool isActive = inventoryCanvas.gameObject.activeSelf;
        inventoryCanvas.gameObject.SetActive(!isActive);
    }

    void PauseGame()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        SetCursorState(true);
    }

    public void ResumeGame()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        StartCoroutine(ForceCursorLock());
    }

    private System.Collections.IEnumerator ForceCursorLock()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSecondsRealtime(0.05f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void SetCursorState(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isVisible;
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
