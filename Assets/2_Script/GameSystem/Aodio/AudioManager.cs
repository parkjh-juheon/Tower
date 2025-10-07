using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("����� �ͼ�")]
    public AudioMixer mixer;

    [Header("BGM, SFX �ҽ�")]
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

    // BGM ��ü ���� ����
    public void SetBGMVolume(float sliderValue)
    {
        float dB = Mathf.Log10(sliderValue) * 20;
        mixer.SetFloat("BGMVolume", dB);
        PlayerPrefs.SetFloat("BGMVolume", dB);
    }

    // SFX ��ü ���� ����
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
