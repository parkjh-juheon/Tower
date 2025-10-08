using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [Header("�ɼ� �г�")]
    public GameObject optionPanel;

    [Header("�κ��丮 ĵ����")]
    public Canvas inventoryCanvas; // InventoryCanvas ���� ����

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

        // ���� ���� Title�̸� Ŀ�� �׻� Ȱ��ȭ
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Title")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // �ٸ� �������� �⺻������ ���
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

        // TabŰ�� �κ��丮 ĵ���� ���
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
        // ���� ���� (���� ȯ��)
        Application.Quit();

#if UNITY_EDITOR
        // ����Ƽ �����Ϳ����� Play ��� ����
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
