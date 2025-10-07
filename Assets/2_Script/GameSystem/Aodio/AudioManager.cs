using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("오디오 믹서")]
    public AudioMixer mixer;

    [Header("BGM, SFX 소스")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private void Awake()
    {
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

    // BGM 전체 볼륨 제어
    public void SetBGMVolume(float sliderValue)
    {
        float dB = Mathf.Log10(sliderValue) * 20;
        mixer.SetFloat("BGMVolume", dB);
        PlayerPrefs.SetFloat("BGMVolume", dB);
    }

    // SFX 전체 볼륨 제어
    public void SetSFXVolume(float sliderValue)
    {
        float dB = Mathf.Log10(sliderValue) * 20;
        mixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", dB);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}
