using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmAudioSource;  // Nhạc nền
    public AudioSource vfxAudioSource;  // Âm thanh hiệu ứng (VFX: obstacle, heal...)

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;   // File nhạc nền

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
            return;
        }

        // Tự động phát nhạc nền nếu được gán
        if (bgmAudioSource != null && backgroundMusic != null)
        {
            bgmAudioSource.clip = backgroundMusic;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (vfxAudioSource != null && clip != null)
        {
            vfxAudioSource.PlayOneShot(clip);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudioSource != null && clip != null)
        {
            bgmAudioSource.clip = clip;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }
    }
}
