using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [Header("�ɼ� �г�")]
    public GameObject optionPanel;

    [Header("�κ��丮 ĵ����")]
    public Canvas inventoryCanvas;

    [Header("���� ����")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("ȿ���� ����")]
    public AudioClip openSound;   // �г� ���� ��
    public AudioClip closeSound;  // �г� ���� ��
    public AudioClip buttonSound; // ��ư Ŭ�� ��

    private bool isPaused = false;
    private bool isTitleScene = false;

    void Start()
    {
        float bgmValue = Mathf.Pow(10, PlayerPrefs.GetFloat("BGMVolume", 0f) / 20);
        float sfxValue = Mathf.Pow(10, PlayerPrefs.GetFloat("SFXVolume", 0f) / 20);

        bgmSlider.value = bgmValue;
        sfxSlider.value = sfxValue;

        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);

        string sceneName = SceneManager.GetActiveScene().name;
        isTitleScene = (sceneName == "Title" || sceneName == "GameOver" || sceneName == "Clear");

        SetCursorState(isTitleScene);
    }

    void Update()
    {
        // Ÿ��Ʋ/���ӿ��� �������� Ŀ�� �׻� Ȱ��
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

        // �κ��丮 ���� �� ȿ����
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(!isActive ? openSound : closeSound);

        SetCursorState(!isActive);
    }

    void PauseGame()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (AudioManager.Instance != null && openSound != null)
            AudioManager.Instance.PlaySFX(openSound);

        SetCursorState(true);
    }

    public void ResumeGame()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (AudioManager.Instance != null && closeSound != null)
            AudioManager.Instance.PlaySFX(closeSound);

        SetCursorState(false);
    }

    private void SetCursorState(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isVisible;
    }

    public void OnButtonClick()
    {
        if (AudioManager.Instance != null && buttonSound != null)
            AudioManager.Instance.PlaySFX(buttonSound);
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null && buttonSound != null)
            AudioManager.Instance.PlaySFX(buttonSound);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
