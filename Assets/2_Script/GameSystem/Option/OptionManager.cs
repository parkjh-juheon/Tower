using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [Header("옵션 패널")]
    public GameObject optionPanel;

    [Header("인벤토리 캔버스")]
    public Canvas inventoryCanvas;

    private bool isPaused = false;
    private bool isTitleScene = false;

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

        isTitleScene = SceneManager.GetActiveScene().name == "Title";

        if (isTitleScene)
        {
            SetCursorState(true);
        }
        else
        {
            SetCursorState(false);
        }
    }

    void Update()
    {
        if (isTitleScene)
        {
            if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
                SetCursorState(true);
            return;
        }

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
        SetCursorState(false);
    }

    private void SetCursorState(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isVisible;
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
