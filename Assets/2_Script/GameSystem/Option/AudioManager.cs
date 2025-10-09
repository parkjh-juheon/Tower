using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("오디오 믹서")]
    public AudioMixer mixer;

    [Header("BGM, SFX 소스")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("씬별 BGM")]
    public AudioClip titleBGM;
    public AudioClip stageBGM;
    public AudioClip bossBGM;
    public AudioClip gameoverBGM;
    public AudioClip clearBGM;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 시 자동 BGM 변경
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneBGM(scene.name);
    }

    // BGM 전체 볼륨 제어
    public void SetBGMVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mixer.SetFloat("BGMVolume", dB);
        PlayerPrefs.SetFloat("BGMVolume", dB);
    }

    // SFX 전체 볼륨 제어
    public void SetSFXVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", dB);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySceneBGM(string sceneName)
    {
        switch (sceneName)
        {
            case "Title":
                PlayBGM(titleBGM);
                break;
            case "Main1":
            case "Mani2":
                PlayBGM(stageBGM);
                break;
            case "Boss":
                PlayBGM(bossBGM);
                break;
            case "Clear":
                PlayBGM(clearBGM);
                break;
            case "GameOver":
                PlayBGM(gameoverBGM);
                break;
            default:
                bgmSource.Stop();
                break;
        }
    }
}
